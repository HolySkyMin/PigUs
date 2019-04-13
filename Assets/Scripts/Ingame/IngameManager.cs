using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

namespace Ingame
{
    public class IngameManager : MonoBehaviour
    {
        public static IngameManager Instance { get; private set; }

        public int Food { get { return Variables[0]; } set { Variables[0] = value; } }
        public int Health { get { return Variables[1]; } set { Variables[1] = value; } }
        public int Favority { get { return Variables[2]; } set { Variables[2] = value; } }
        public int DayCount { get; private set; }
        public int FavorityPhase { get; set; } // 0: normal, 1: human, 2: pig, 3: tutorial

        public int[] Variables; // 0: 포만감, 1: 건강, 2: 친밀도
        public Text DayText;
        public Slider FoodSlider, HealthSlider, FavoritySlider;

        private DialogueGroup eventDialog;
        private List<SelectionLog> dayLog;
        private Dictionary<int, List<DialogueGroup>> dayEvents;

        private void Awake()
        {
            Instance = this;

            LoadData();
            Initialize();
        }

        public void LoadData()
        {
            DayCount = GameManager.Instance.Save.DayCount;
            Variables = GameManager.Instance.Save.Variables.Clone() as int[];
            dayLog = new List<SelectionLog>();
            for (int i = 0; i < GameManager.Instance.Save.DayLog.Count; i++)
                dayLog.Add(GameManager.Instance.Save.DayLog[i]);
        }

        public void SaveData()
        {
            GameManager.Instance.Save.DayCount = DayCount;
            GameManager.Instance.Save.Variables = Variables.Clone() as int[];
            var tmpCount = GameManager.Instance.Save.DayLog.Count;
            for (int i = 0; i < dayLog.Count; i++)
            {
                if (i >= tmpCount)
                    GameManager.Instance.Save.DayLog.Add(dayLog[i]);
            }
            GameManager.Instance.SaveToFile();
        }
        private void Start()
        {
            UpdateRoutine();
        }

        public void Initialize()
        {
            dayEvents = new Dictionary<int, List<DialogueGroup>>();
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; ; j++)
                {
                    var asset = Resources.Load<TextAsset>($"DayEvents/Event{i}/Event{i}_{j}");
                    if (asset == null)
                        break;
                    else
                    {
                        var dialog = JsonMapper.ToObject<DialogueGroup>(asset.text);
                        if (!dayEvents.ContainsKey(i))
                            dayEvents.Add(i, new List<DialogueGroup>());
                        dayEvents[i].Add(dialog);
                    }
                }
            }

            //if (Favority < 30)
            //    FavorityPhase = 1;
            //else if (Favority > 70)
            //    FavorityPhase = 2;
            //else
            //    FavorityPhase = 0;

            //if (DayCount == 0)
            //    FavorityPhase = 3;
        }

        public async void UpdateRoutine()
        {
            // 현재는 7일 돌리고 무조건 넘어감.
            for(int i = DayCount; i < 7; i++)
            {
                DayCount++;
                eventDialog = dayEvents[FavorityPhase][Random.Range(0, dayEvents[FavorityPhase].Count)];
                await RunDayDialogue();
                SaveData();
            }
            SceneChanger.Instance.ChangeScene("ResultScene");
        }

        private void Update()
        {
            DayText.text = $"DAY {DayCount}";
            FoodSlider.value = Food;
            HealthSlider.value = Health;
            FavoritySlider.value = Favority;
        }

        public async Task RunDayDialogue()
        {
            for(int i = 0; i < eventDialog.Length; i++)
            {
                var dialog = eventDialog[i];
                if (dialog.Type == 0)
                    await DialogueManager.Instance.ShowDialogue(dialog.Talker, dialog.Context);
                else if (dialog.Type == 1) // 대전제: 선택은 한 차례만 한다.
                {
                    var res = await DialogueManager.Instance.ShowInteraction(new[] { dialog.Selects[0].Context, dialog.Selects[1].Context });
                    for(int j = 0; j < Variables.Length; j++)
                    {
                        if (dialog.Selects[res].VariableType[j] > 0)
                            Variables[j] += dialog.Selects[res].VariableDelta[j];
                    }
                    dayLog.Add(new SelectionLog() { Day = DayCount, SelectedContext = dialog.Selects[res].Context, DeltaValue = dialog.Selects[res].VariableDelta.Clone() as int[] });
                    for(int j = 0; j < dialog.Selects[res].Length; j++)
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
        }
    }
}
