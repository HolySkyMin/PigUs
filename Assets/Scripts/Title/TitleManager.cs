using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Title
{
    public class TitleManager : MonoBehaviour
    {
        public Button NewGameBtn, LoadGameBtn;
        public Toggle TutorialSkipper, Phase1Skipper;

        private void Start()
        {
            if (GameManager.SaveFileExists || GameManager.Phase2Exists)
                LoadGameBtn.interactable = true;
            else
                LoadGameBtn.interactable = false;
            if (GameManager.Phase2Exists)
                Phase1Skipper.interactable = true;
            else
                Phase1Skipper.interactable = false;

            SoundManager.Instance.PlayBgm("Pig, Us");
        }

        public void StartGame()
        {
            GameManager.Instance.CreateData(TutorialSkipper.isOn);
            SceneChanger.Instance.ChangeScene("DayScene");
        }

        public void LoadGame()
        {
            if (Phase1Skipper.isOn || !GameManager.SaveFileExists)
                GameManager.Instance.LoadPhase2();
            else
                GameManager.Instance.LoadToData();
            SceneChanger.Instance.ChangeScene("DayScene");
        }

        public void ToggleFullscreen()
        {
            if (Screen.fullScreen)
                Screen.SetResolution(1600, 900, false);
            else
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
    }
}