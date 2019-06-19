using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using ScriptedAnimation;

namespace Ingame
{
    public class DebugIngameManager : MonoBehaviour
    {
        public int Food { get { return Variables[0]; } set { Variables[0] = value; } }
        public int Health { get { return Variables[1]; } set { Variables[1] = value; } }
        public int HumanFavority { get { return Variables[2]; } set { Variables[2] = value; } }
        public int PigFavority { get { return Variables[3]; } set { Variables[3] = value; } }

        public Slider FoodSlider, HealthSlider, HumanSlider, PigSlider;
        public ScriptAnimation DialogAnimation;
        public GameObject SettingPanel;
        public InputField PathInput;
        public Text RunResult;

        private int[] Variables = new[] { 50, 50, 0, 0 };

        private void Update()
        {
            FoodSlider.value = Food;
            HealthSlider.value = Health;
            HumanSlider.value = HumanFavority;
            PigSlider.value = PigFavority;
        }

        public async void PlayDialogue()
        {
            Variables = new[] { 50, 50, 0, 0 };

            var asset = Resources.Load<TextAsset>($"DayEvents/{PathInput.text}");
            if(asset == null)
                RunResult.text = "파일이 없습니다.";
            else
            {
                DialogueGroup dialog;
                try { dialog = JsonMapper.ToObject<DialogueGroup>(asset.text); }
                catch { RunResult.text = "파일 불러오기 종 오류가 발생했습니다."; return; }
                SettingPanel.SetActive(false);
                var result = await RunDayDialogue(dialog, true, true);
                RunResult.text = "성공적으로 플레이를 마쳤습니다. 결과: " + result.ToString();
                SoundManager.Instance.StopBgm();
                SettingPanel.SetActive(true);
            }
        }

        public void RunIntegrityTest()
        {
            var dirInfo = new DirectoryInfo(Application.dataPath + "/Resources/DayEvents");
            var dirs = dirInfo.GetDirectories();

            Debug.Log("Starting integrity test...");
            int errCnt = 0, fileCnt = 0;
            for(int i = 0; i < dirs.Length; i++)
            {
                var files = dirs[i].GetFiles();
                for(int j = 0; j < files.Length; j++)
                {
                    if (files[j].Name.EndsWith(".meta"))
                        continue;
                    var realname = files[j].Name.Substring(0, files[j].Name.Length - 5);
                    var asset = Resources.Load<TextAsset>($"DayEvents/{dirs[i].Name}/{realname}");
                    try { JsonMapper.ToObject<DialogueGroup>(asset.text); }
                    catch { Debug.LogWarning($"- Error occured at {dirs[i].Name}/{realname}"); errCnt++; }
                    fileCnt++;
                }
            }
            Debug.Log($"Integrity test finished. {fileCnt} files, {errCnt} error(s).");
        }

        public async Task<bool> RunDayDialogue(DialogueGroup dialogGroup, bool playAppear, bool playDisappear)
        {
            if (playAppear)
                await DialogAnimation.Appear();
            bool selectRes = false;
            for (int i = 0; i < dialogGroup.Length; i++)
            {
                var dialog = dialogGroup[i];
                if (dialog.Type == 0)
                    await DialogueManager.Instance.ShowDialogue(dialog.Talker, dialog.Context, dialog.Unskippable);
                else if (dialog.Type == 1) // 대전제: 선택은 한 차례만 한다.
                {
                    var res = await DialogueManager.Instance.ShowInteraction(new[] { dialog.Selects[0].Context, dialog.Selects[1].Context });
                    selectRes = dialog.Selects[res].IsTrigger;
                    for (int j = 0; j < Variables.Length; j++)
                    {
                        if (j < 2)
                            Variables[j] += (int)(dialog.Selects[res].VariableDelta[j] * 1.5f);
                        else
                            Variables[j] += dialog.Selects[res].VariableDelta[j];
                        if (Variables[j] > 100)
                            Variables[j] = 100;
                        if (Variables[j] < 0)
                            Variables[j] = 0;

                        // 예외 케이스
                        if (dialog.Selects[res].VariableDelta[0] == 6 && dialog.Selects[res].VariableDelta[1] == 6)
                        {
                            Variables[0] = 50;
                            Variables[1] = 50;
                        }
                    }
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
            if (playDisappear)
                await DialogAnimation.Disappear();
            return selectRes;
        }
    }
}
