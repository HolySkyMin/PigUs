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

        private List<DialogueGroup> dayStoryNeutral, dayNormalReuse;
        private Dictionary<int, List<DialogueGroup>> dayEvents, storyHuman1, storyPig1;

        private void Awake()
        {
            Instance = this;

            LoadData();
            Initialize();
        }

        public void LoadData()
        {
            Data = GameManager.Instance.Save;

            // 랜덤 이벤트들을 불러옵니다.
            LoadRandomBag();

            // 인간 스토리 (1차) 를 불러옵니다.
            storyHuman1 = new Dictionary<int, List<DialogueGroup>>();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; ; j++)
                {
                    var asset = Resources.Load<TextAsset>($"DayEvents/Human1/{i}_{j}");
                    if (asset == null)
                        break;
                    else
                    {
                        if (!storyHuman1.ContainsKey(i))
                            storyHuman1.Add(i, new List<DialogueGroup>());
                        var dialog = JsonMapper.ToObject<DialogueGroup>(asset.text);
                        storyHuman1[i].Add(dialog);
                    }
                }
            }

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
        }

        public void LoadRandomBag()
        {
            if (Data.DayCount > 0)
                return;
            if (Data.RandomQueue.Count > 0)
                return;

            for(int i = 0; i < 30; i++)
            {
                var bag = new List<DialogueGroup>();
                for(int j = 0; ; j++)
                {
                    var asset = Resources.Load<TextAsset>($"DayEvents/Random/{i}_{j}");
                    if(asset == null)
                    {
                        if (j == 0)
                            i = ((i + 10) / 10) * 10 - 1; // 10*n+a에서, 다음 반복 때 10*(n+1)이 되도록 합니다.
                        else if (i / 10 == 1)
                        {
                            var shuffleQueue = new Queue<DialogueGroup>();
                            while(bag.Count > 0)
                            {
                                int rnd = Random.Range(0, bag.Count);
                                shuffleQueue.Enqueue(bag[rnd]);
                                bag.RemoveAt(rnd);
                            }
                            while (shuffleQueue.Count > 0)
                                Data.RandomQueue.Enqueue(shuffleQueue.Dequeue());
                        }
                        else
                            Data.RandomQueue.Enqueue(bag);
                        break;
                    }
                    else
                        bag.Add(JsonMapper.ToObject<DialogueGroup>(asset.text));
                }
            }
        }

        public void LoadRandomGameoverBag()
        {
            Data.RandomQueue.Clear();
            for(int i = 0; ; i++)
            {
                var asset = Resources.Load<TextAsset>($"DayEvents/GameOver/Phase1_WrongSelect_Bag{i}");
                if (asset == null)
                    break;
                Data.RandomQueue.Enqueue(JsonMapper.ToObject<DialogueGroup>(asset.text));
            }
        }

        public void SaveData()
        {
            GameManager.Instance.SaveToFile();
        }

        private void Start()
        {
            UpdateRoutine();
        }

        public void Initialize()
        {
            Data.BlendRandomNormal = true;
        }

        public async void UpdateRoutine()
        {
            // 조건에 맞을 시 튜토리얼을 실행합니다.
            if (Data.DayCount == 0 && !Data.SkipTutorial)
            {
                SoundManager.Instance.PlayBgm("Tutorial");
                await RunTutorial();
                Data.SkipTutorial = true;
                SaveData();
                await new WaitForSeconds(0.5f);
            }

            for(int i = Data.DayCount; ; i++)
            {
                // 우선 일수를 늘립니다.
                Data.DayCount++;
                Data.CurrentStoryType = 0;

                // 이후의 값과 비교하기 위해 현재 값들을 저장해 둡니다.
                var befHumanProg = HumanFavority / 3;
                var befPigProg = PigFavority / 3;
                var befPhase = Data.StoryPhase;

                // 조건에 따라 다이얼로그를 실행합니다.
                if(Data.StoryPhase == 0)
                {
                    SoundManager.Instance.PlayBgm("Choice");
                    if (Data.RandomQueue.Count < 1)
                    {
                        ShowGameOver("DayEvents/GameOver/General_Sold", true);
                        break;
                    }
                    if (Data.DayCount == Data.Phase1StoryDay)
                    {
                        if(Data.StoryQueue.Count > 0)
                        {
                            var stories = Data.StoryQueue.Dequeue();
                            Data.CurrentStoryType = Data.StoryTypeQueue.Dequeue();
                            bool selectRes = false;
                            for (int j = 0; j < stories.Count; j++)
                                selectRes = await RunDayDialogue(stories[j], j == 0 ? true : false, j == stories.Count - 1 && !Data.IsHumanStackFull && !Data.IsPigStackFull ? true : false);

                            if (Data.IsHumanStackFull)
                            {
                                if (selectRes)
                                    await SetPhase2(1);
                                else
                                    await SetPhase1GameOver(1);
                            }
                            if (Data.IsPigStackFull)
                            {
                                if (selectRes)
                                    await SetPhase2(2);
                                else
                                    await SetPhase1GameOver(2);
                            }
                        }
                        else
                            await RunDayRandomEvent();
                        Data.Phase1StoryDay += Random.Range(3, 5);
                    }
                    else
                        await RunDayRandomEvent();
                }
                else if(Data.StoryPhase == 1 || Data.StoryPhase == 2)
                {
                    Data.CurrentStoryType = Data.StoryPhase;

                    var stories = Data.StoryQueue.Dequeue();
                    bool selectRes = false;
                    for (int j = 0; j < stories.Count; j++)
                    {
                        selectRes = await RunDayDialogue(stories[j], j == 0 ? true : false, j == stories.Count - 1 ? true : false);
                        if (Data.StoryQueue.Count < 1 && j == stories.Count - 1)
                        {
                            ShowGameOver($"DayEvents/{(Data.StoryPhase == 1 ? "Human" : "Pig")}2/Ending{(selectRes ? "1" : "2")}", true);
                            GameManager.Instance.DeleteFile("save");
                            return;
                        }
                        else if (selectRes)
                        {
                            ShowGameOver($"DayEvents/GameOver/{stories[j].ToGameOver}", j == stories.Count - 1 ? true : false);
                            return;
                        }
                    }
                }
                else if(Data.StoryPhase == -1)
                {
                    SoundManager.Instance.PlayBgm("Pig, Us (Twisted ver.)");
                    Data.CurrentStoryType = -1;
                    Data.BlendRandomNormal = false;
                    if (Data.RandomQueue.Count < 1)
                    {
                        ShowGameOver("DayEvents/GameOver/Phase1_WrongSelect", true);
                        break;
                    }
                    else
                        await RunDayRandomEvent();
                }
                else if(Data.StoryPhase == -9)
                {
                    SoundManager.Instance.PlayBgm("Death");
                    Data.CurrentStoryType = -1;
                    ShowGameOver("DayEvents/GameOver/General_Death", true);
                    break;
                }
                else if(Data.StoryPhase == -10)
                {
                    SoundManager.Instance.PlayBgm("Death");
                    Data.CurrentStoryType = -1;
                    ShowGameOver("DayEvents/GameOver/General_Sold", true);
                    break;
                }
                else
                    return;

                // 값 변화를 비교하고 변경 사항을 적용합니다. (Only for Phase 1)
                if ((HumanFavority / 3) > befHumanProg && HumanFavority < 18)
                {
                    for (int j = befHumanProg; j < HumanFavority / 3; j++)
                    {
                        var stories = new List<DialogueGroup>();
                        for (int k = 0; k < storyHuman1[j].Count; k++)
                            stories.Add(storyHuman1[j][k]);
                        Data.StoryQueue.Enqueue(stories);
                        Data.StoryTypeQueue.Enqueue(1);
                    }
                }
                if ((PigFavority / 3) > befPigProg && PigFavority < 18 && !Data.IsHumanStackFull)
                {
                    for (int j = befPigProg; j < PigFavority / 3; j++)
                    {
                        var stories = new List<DialogueGroup>();
                        for (int k = 0; k < storyPig1[j].Count; k++)
                            stories.Add(storyPig1[j][k]);
                        Data.StoryQueue.Enqueue(stories);
                        Data.StoryTypeQueue.Enqueue(2);
                    }
                }
                if (HumanFavority >= 15)
                    Data.IsHumanStackFull = true;
                if (PigFavority >= 15)
                    Data.IsPigStackFull = true;
                if (Food <= 0 || Health <= 0)
                    Data.StoryPhase = -9;
                if (Food >= 100 || Health >= 100)
                    Data.StoryPhase = -10;
                if (befPhase != Data.StoryPhase)
                    Data.PhaseChangedDate = Data.DayCount;

                // 마지막으로 현재 상태를 저장합니다.
                SaveData();
            }
        }

        public async Task RunDayRandomEvent()
        {
            var dialog = Data.RandomQueue.Dequeue();
            if (dialog is DialogueGroup)
                await RunDayDialogue(dialog as DialogueGroup, true, true);
            else if (dialog is List<DialogueGroup>)
            {
                var cnt = (dialog as List<DialogueGroup>).Count;
                for (int i = 0; i < cnt; i++)
                    await RunDayDialogue((dialog as List<DialogueGroup>)[i], i == 0 ? true : false, i == cnt - 1 ? true : false);
            }
            else
                Debug.LogError("올바르지 않은 다이얼로그 큐 타입입니다.");
        }

        public async Task SetPhase2(int type)
        {
            var transDiag = new List<DialogueGroup>();
            string dialPath = $"DayEvents/Transition/";
            for(int i = 0; ; i++)
            {
                var asset = Resources.Load<TextAsset>(dialPath + $"{(type == 1 ? "Human_" : "Pig_")}{i}");
                if (asset == null)
                    break;
                transDiag.Add(JsonMapper.ToObject<DialogueGroup>(asset.text));
            }
            for (int i = 0; i < transDiag.Count; i++)
                await RunDayDialogue(transDiag[i], false, i == transDiag.Count - 1 ? true : false);

            Data.StoryQueue.Clear();
            Data.StoryPhase = type;
            for(int i = 0; ; i++)
            {
                var dialogList = new List<DialogueGroup>();
                for(int j = 0; ; j++)
                {
                    var asset = Resources.Load<TextAsset>($"DayEvents/{(type == 1 ? "Human" : "Pig")}2/{i}_{j}");
                    if (asset == null)
                        break;
                    dialogList.Add(JsonMapper.ToObject<DialogueGroup>(asset.text));
                }
                if (dialogList.Count < 1)
                    break;
                else
                    Data.StoryQueue.Enqueue(dialogList);
            }

            GameManager.Instance.SavePhase2();
        }

        public async Task SetPhase1GameOver(int type)
        {
            Data.StoryPhase = -1;
            var dialog = new DialogueGroup();
            if(type == 1)
                dialog = JsonMapper.ToObject<DialogueGroup>(Resources.Load<TextAsset>("DayEvents/GameOver/Phase1_Human").text);
            else
                dialog = JsonMapper.ToObject<DialogueGroup>(Resources.Load<TextAsset>("DayEvents/GameOver/Phase1_Pig").text);
            await RunDayDialogue(dialog, false, true);
            LoadRandomGameoverBag();
        }

        public async void ShowGameOver(string dialogPath, bool appear)
        {
            var dialog = JsonMapper.ToObject<DialogueGroup>(Resources.Load<TextAsset>(dialogPath).text);
            if(dialog != null)
                await RunDayDialogue(dialog, appear, true);
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
                await RunDayDialogue(tutorial[i], i > 0 ? false : true, i < tutorial.Count - 1 ? false : true, false);
        }

        public async Task<bool> RunDayDialogue(DialogueGroup dialogGroup, bool playAppear, bool playDisappear, bool writeLog = true)
        {
            if(playAppear)
                await DialogAnimation.Appear();
            bool selectRes = false;
            for(int i = 0; i < dialogGroup.Length; i++)
            {
                var dialog = dialogGroup[i];
                if (dialog.Type == 0)
                    await DialogueManager.Instance.ShowDialogue(dialog.Talker, dialog.Context, dialog.Unskippable);
                else if (dialog.Type == 1) // 대전제: 선택은 한 차례만 한다.
                {
                    var res = await DialogueManager.Instance.ShowInteraction(new[] { dialog.Selects[0].Context, dialog.Selects[1].Context });
                    selectRes = dialog.Selects[res].IsTrigger;
                    for (int j = 0; j < Data.Variables.Length; j++)
                    {
                        if(j < 2)
                            Data.Variables[j] += (int)(dialog.Selects[res].VariableDelta[j] * 1.5f);
                        else
                            Data.Variables[j] += dialog.Selects[res].VariableDelta[j];
                        if (Data.Variables[j] > 100)
                            Data.Variables[j] = 100;
                        if (Data.Variables[j] < 0)
                            Data.Variables[j] = 0;

                        // 예외 케이스
                        if(dialog.Selects[res].VariableDelta[0] == 6 && dialog.Selects[res].VariableDelta[1] == 6)
                        {
                            Data.Variables[0] = 50;
                            Data.Variables[1] = 50;
                        }
                    }
                    if (writeLog)
                        Data.DayLog.Add(new SelectionLog(Data.CurrentStoryType, Data.DayCount, dialog.Selects[res].Context, dialog.Selects[res].VariableDelta));
                    for (int j = 0; j < dialog.Selects[res].Length; j++)
                    {
                        var afterDialog = dialog.Selects[res][j];
                        if (afterDialog.Type == 0)
                            await DialogueManager.Instance.ShowDialogue(afterDialog.Talker, afterDialog.Context, afterDialog.Unskippable);
                        else if (afterDialog.Type == 2)
                            DialogueManager.Instance.ShowImage(afterDialog.ImageKey);
                        else if (afterDialog.Type == 3)
                            SoundManager.Instance.PlayBgm(afterDialog.BgmKey);
                        else if (afterDialog.Type == 4)
                            SoundManager.Instance.PlaySe(afterDialog.SEKey);
                    }
                }
                else if (dialog.Type == 2)
                    DialogueManager.Instance.ShowImage(dialog.ImageKey);
                else if (dialog.Type == 3)
                    SoundManager.Instance.PlayBgm(dialog.BgmKey);
                else if (dialog.Type == 4)
                    SoundManager.Instance.PlaySe(dialog.SEKey);
            }
            DialogueManager.Instance.CleanDialogue();
            if(playDisappear)
                await DialogAnimation.Disappear();
            return selectRes;
        }
    }
}
