using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using ScriptedAnimation;

namespace Ingame
{
    public class IngameManager : MonoBehaviour
    {
        public static IngameManager Instance { get; private set; }

        public int Food { get { return Data.Variables[0]; } set { Data.Variables[0] = value; } }
        public int Health { get { return Data.Variables[1]; } set { Data.Variables[1] = value; } }
        public int HumanFavority { get { return Data.Variables[2]; } set { Data.Variables[2] = value; } }
        public int PigFavority { get { return Data.Variables[3]; } set { Data.Variables[3] = value; } }
        public SaveData Data { get; set; }

        public Text DayText;
        public Slider FoodSlider, HealthSlider, FavoritySlider;
        public ScriptAnimation DialogAnimation;

        private bool isHumanStackFull = false, isPigStackFull = false;
        private List<DialogueGroup> dayRandGeneral, dayStoryNeutral;
        private Dictionary<int, List<DialogueGroup>> dayEvents, storyHuman1, storyPig1, storyPig2;

        private void Awake()
        {
            Instance = this;

            LoadData();
            Initialize();
        }

        public void LoadData()
        {
            Data = GameManager.Instance.Save;
        }

        public void SaveData()
        {
            GameManager.Instance.SaveToFile();
        }

        private void Start()
        {
            SoundManager.Instance.PlayBgm("Choice");
            UpdateRoutine();
        }

        public void Initialize()
        {
            // 랜덤 일반 이벤트들을 불러옵니다.
            dayRandGeneral = new List<DialogueGroup>();
            for(int i = 0; ; i++)
            {
                var asset = Resources.Load<TextAsset>($"DayEvents/Random/Normal_{i}");
                if (asset == null)
                    break;
                else
                {
                    var dialog = JsonMapper.ToObject<DialogueGroup>(asset.text);
                    dayRandGeneral.Add(dialog);
                }
            }

            // 랜덤 친밀도 이벤트들을 불러옵니다.
            dayStoryNeutral = new List<DialogueGroup>();
            for (int i = 0; ; i++)
            {
                var asset = Resources.Load<TextAsset>($"DayEvents/Random/Story_{i}");
                if (asset == null)
                    break;
                else
                {
                    var dialog = JsonMapper.ToObject<DialogueGroup>(asset.text);
                    dayStoryNeutral.Add(dialog);
                }
            }

            // 인간 스토리 (1차) 를 불러옵니다.
            //storyHuman1 = new Dictionary<int, List<DialogueGroup>>();
            //for(int i = 0; i < 5; i++)
            //{
            //    for(int j = 0; ; j++)
            //    {
            //        var asset = Resources.Load<TextAsset>($"DayEvents/Human1/{i}_{j}");
            //        if (asset == null)
            //            break;
            //        else
            //        {
            //            if (!storyHuman1.ContainsKey(i))
            //                storyHuman1.Add(i, new List<DialogueGroup>());
            //            var dialog = JsonMapper.ToObject<DialogueGroup>(asset.text);
            //            storyHuman1[i].Add(dialog);
            //        }
            //    }
            //}

            // 동물 스토리 (1차) 를 불러옵니다.
            storyPig1 = new Dictionary<int, List<DialogueGroup>>();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; ; j++)
                {
                    var asset = Resources.Load<TextAsset>($"DayEvents/Pig1/{i}_{j}");
                    if (asset == null)
                        break;
                    else
                    {
                        if (!storyPig1.ContainsKey(i))
                            storyPig1.Add(i, new List<DialogueGroup>());
                        var dialog = JsonMapper.ToObject<DialogueGroup>(asset.text);
                        storyPig1[i].Add(dialog);
                    }
                }
            }

            // 동물 스토리 (2차) 를 불러옵니다.
            storyPig2 = new Dictionary<int, List<DialogueGroup>>();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; ; j++)
                {
                    var asset = Resources.Load<TextAsset>($"DayEvents/Pig2/{i}_{j}");
                    if (asset == null)
                        break;
                    else
                    {
                        if (!storyPig2.ContainsKey(i))
                            storyPig2.Add(i, new List<DialogueGroup>());
                        var dialog = JsonMapper.ToObject<DialogueGroup>(asset.text);
                        storyPig2[i].Add(dialog);
                    }
                }
            }
        }

        public async void UpdateRoutine()
        {
            // If first run and set to be ran, run the tutorial.
            if (Data.DayCount == 0 && !Data.SkipTutorial)
            {
                await RunTutorial();
                Data.SkipTutorial = true;
                SaveData();
                await new WaitForSeconds(0.5f);
            }

            CheckQueue();
            for(int i = Data.DayCount; ; i++)
            {
                // 우선 일수를 늘립니다.
                Data.DayCount++;

                // 이후의 값과 비교하기 위해 현재 값들을 저장해 둡니다.
                var befHumanProg = HumanFavority / 3;
                var befPigProg = PigFavority / 3;
                var befPhase = Data.StoryPhase;
                
                // 조건에 따라 다이얼로그를 실행합니다.
                if(Data.StoryPhase == 0)
                {
                    if (Data.DayCount == Data.Phase1StoryDay)
                    {
                        if(Data.StoryQueue.Count > 0)
                        {
                            var stories = Data.StoryQueue.Dequeue();
                            bool selectRes = false;
                            for (int j = 0; j < stories.Count; j++)
                                selectRes = await RunDayDialogue(stories[j]);

                            if (isHumanStackFull)
                            {
                                if (selectRes)
                                    SetPhase2(1);
                                else
                                    Data.StoryPhase = -1;
                            }
                            if (isPigStackFull)
                            {
                                if (selectRes)
                                    SetPhase2(2);
                                else
                                    Data.StoryPhase = -1;
                            }
                        }
                        else
                            await RunDayDialogue(GetDayRandomEvent(50));
                        Data.Phase1StoryDay += Random.Range(3, 5);
                    }
                    else
                        await RunDayDialogue(GetDayRandomEvent(50));
                }
                else if(Data.StoryPhase == 1)
                {
                    Debug.Log("Succesfully entered human phase 2.");
                    break;
                }
                else if(Data.StoryPhase == 2)
                {
                    if(Data.StoryQueue.Count < 1)
                    {
                        // Pig Ending. Will move to ending cutscene.
                        SceneChanger.Instance.ChangeScene("ResultScene");
                        break;
                    }
                    else
                    {
                        var stories = Data.StoryQueue.Dequeue();
                        bool selectRes = false;
                        for (int j = 0; j < stories.Count; j++)
                        {
                            selectRes = await RunDayDialogue(stories[j]);
                            if (selectRes)
                                break;
                        }
                        
                        // Trigger가 True이면 게임 오버가 되어 씬을 이동합니다.
                        if (selectRes)
                        {
                            ShowGameOver($"DayEvents/GameOver/Phase2_Pig{storyPig2.Count - Data.StoryQueue.Count}");
                            break;
                        }
                    }
                }
                else if(Data.StoryPhase == -1)
                {
                    if(Data.DayCount > Data.PhaseChangedDate + 12)
                    {
                        ShowGameOver("DayEvents/GameOver/Phase1_WrongSelect");
                        break;
                    }
                    else
                        await RunDayDialogue(GetDayRandomEvent(0));
                }
                else if(Data.StoryPhase == -9)
                {
                    ShowGameOver("DayEvents/GameOver/General_Death");
                    break;
                }
                else
                    return;
                CheckQueue();

                // 값 변화를 비교하고 변경 사항을 적용합니다.
                //if((HumanFavority / 3) > befHumanProg && HumanFavority < 18)
                //{
                //    for(int j = befHumanProg; j < HumanFavority / 3; j++)
                //    {
                //        var stories = new List<DialogueGroup>();
                //        for (int k = 0; k < storyHuman1[j].Count; k++)
                //            stories.Add(storyHuman1[j][k]);
                //        dayStoryQueue.Enqueue(stories);
                //    }
                //}
                if((PigFavority / 3) > befPigProg && PigFavority < 18 && !isHumanStackFull)
                {
                    for (int j = befPigProg; j < PigFavority / 3; j++)
                    {
                        var stories = new List<DialogueGroup>();
                        for (int k = 0; k < storyPig1[j].Count; k++)
                            stories.Add(storyPig1[j][k]);
                        Data.StoryQueue.Enqueue(stories);
                    }
                }
                if (HumanFavority >= 15)
                    isHumanStackFull = true;
                if (PigFavority >= 15)
                    isPigStackFull = true;
                if (Food <= 0 || Health <= 0)
                    Data.StoryPhase = -9;
                if (befPhase != Data.StoryPhase)
                    Data.PhaseChangedDate = Data.DayCount;

                // 마지막으로 현재 상태를 저장합니다.
                SaveData();
            }
        }

        public DialogueGroup GetDayRandomEvent(int storyPerc)
        {
            int rnd = Random.Range(0, 100);
            if (rnd < storyPerc)
                return Data.RandomStoryBag.Dequeue();
            else
                return Data.RandomGeneralBag.Dequeue();
            
        }

        public void CheckQueue()
        {
            if(Data.RandomGeneralBag.Count < 1)
            {
                var shuffler = new List<DialogueGroup>();
                foreach (var item in dayRandGeneral)
                    shuffler.Add(item);
                while(shuffler.Count > 0)
                {
                    int idx = Random.Range(0, shuffler.Count);
                    Data.RandomGeneralBag.Enqueue(shuffler[idx]);
                    shuffler.RemoveAt(idx);
                }
            }
            if(Data.RandomStoryBag.Count < 1)
            {
                var shuffler = new List<DialogueGroup>();
                foreach (var item in dayStoryNeutral)
                    shuffler.Add(item);
                while(shuffler.Count > 0)
                {
                    int idx = Random.Range(0, shuffler.Count);
                    Data.RandomStoryBag.Enqueue(shuffler[idx]);
                    shuffler.RemoveAt(idx);
                }
            }
        }

        public void SetPhase2(int type)
        {
            Data.StoryQueue.Clear();

            Data.StoryPhase = type;
            if(type == 2)
            {
                foreach (KeyValuePair<int, List<DialogueGroup>> day in storyPig2)
                    Data.StoryQueue.Enqueue(day.Value);
            }
        }

        public async void ShowGameOver(string dialogPath)
        {
            var dialog = JsonMapper.ToObject<DialogueGroup>(Resources.Load<TextAsset>(dialogPath).text);
            if(dialog != null)
                await RunDayDialogue(dialog);
            SceneChanger.Instance.ChangeScene("ResultScene");
        }

        private void Update()
        {
            DayText.text = $"DAY {Data.DayCount}";
            FoodSlider.value = Food;
            HealthSlider.value = Health;
        }

        public async Task RunTutorial()
        {
            // 튜토리얼을 불러옵니다.
            var tutorial = new List<DialogueGroup>();
            for(int i = 0; ; i++)
            {
                var asset = Resources.Load<TextAsset>($"DayEvents/Tutorial/Tutorial_{i}");
                if (asset == null)
                    break;
                else
                {
                    var dialog = JsonMapper.ToObject<DialogueGroup>(asset.text);
                    tutorial.Add(dialog);
                }
            }

            // 튜토리얼을 플레이합니다.
            for(int i = 0; i < tutorial.Count; i++)
                await RunDayDialogue(tutorial[i], false);
        }

        public async Task<bool> RunDayDialogue(DialogueGroup dialogGroup, bool writeLog = true)
        {
            await DialogAnimation.Appear();
            bool selectRes = false;
            for(int i = 0; i < dialogGroup.Length; i++)
            {
                var dialog = dialogGroup[i];
                if (dialog.Type == 0)
                    await DialogueManager.Instance.ShowDialogue(dialog.Talker, dialog.Context);
                else if (dialog.Type == 1) // 대전제: 선택은 한 차례만 한다.
                {
                    var res = await DialogueManager.Instance.ShowInteraction(new[] { dialog.Selects[0].Context, dialog.Selects[1].Context });
                    selectRes = dialog.Selects[res].IsTrigger;
                    for(int j = 0; j < Data.Variables.Length; j++)
                    {
                        if (dialog.Selects[res].VariableType[j] > 0)
                        {
                            Data.Variables[j] += dialog.Selects[res].VariableDelta[j];
                            if (Data.Variables[j] > 100)
                                Data.Variables[j] = 100;
                            if (Data.Variables[j] < 0)
                                Data.Variables[j] = 0;
                        }
                    }
                    if (writeLog)
                        Data.DayLog.Add(new SelectionLog(Data.StoryPhase, Data.DayCount, dialog.Selects[res].Context, dialog.Selects[res].VariableDelta));
                    for (int j = 0; j < dialog.Selects[res].Length; j++)
                    {
                        var afterDialog = dialog.Selects[res][j];
                        if (afterDialog.Type == 0)
                            await DialogueManager.Instance.ShowDialogue(afterDialog.Talker, afterDialog.Context);
                        else if(afterDialog.Type == 2)
                            DialogueManager.Instance.ShowImage(afterDialog.ImageKey);
                    }
                }
                else if (dialog.Type == 2)
                    DialogueManager.Instance.ShowImage(dialog.ImageKey);
            }
            DialogueManager.Instance.CleanDialogue();
            await DialogAnimation.Disappear();
            return selectRes;
        }
    }
}
