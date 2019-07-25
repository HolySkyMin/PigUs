using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Title
{
    public class TitleManager : MonoBehaviour
    {
        public Button NewGameBtn, LoadGameBtn, Phase2LoadBtn;
        public Toggle TutorialSkipper;

        private void Start()
        {
            if (GameManager.SaveFileExists || GameManager.Phase2Exists)
                LoadGameBtn.interactable = true;
            else
                LoadGameBtn.interactable = false;
            if (GameManager.Phase2Exists)
                Phase2LoadBtn.interactable = true;
            else
                Phase2LoadBtn.interactable = false;

            SoundManager.Instance.PlayBgm("Pig, Us");
        }

        public void StartGame()
        {
            GameManager.Instance.CreateData(GameManager.Instance.Config.SkipTutorial);
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
                SceneChanger.Instance.ChangeScene("DayScene");
            else
                SceneChanger.Instance.ChangeScene("MDayScene");
        }

        public void LoadGame(bool isPhase2)
        {
            if (isPhase2)
                GameManager.Instance.LoadPhase2();
            else
                GameManager.Instance.LoadToData();
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
                SceneChanger.Instance.ChangeScene("DayScene");
            else
                SceneChanger.Instance.ChangeScene("MDayScene");
        }

        public void ToggleFullscreen()
        {
            if (Screen.fullScreen)
                Screen.SetResolution(1600, 900, false);
            else
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }

        public void MoveScene(string sceneName)
        {
            SceneChanger.Instance.ChangeScene(sceneName);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}