using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Title
{
    public class TitleManager : MonoBehaviour
    {
        public Button NewGameBtn, LoadGameBtn;
        public Toggle TutorialSkipper;

        private void Start()
        {
            if (GameManager.SaveFileExists)
                LoadGameBtn.interactable = true;
            else
                LoadGameBtn.interactable = false;

            SoundManager.Instance.PlayBgm("Pig, Us");
        }

        public void StartGame()
        {
            GameManager.Instance.CreateData(TutorialSkipper.isOn);
            SceneChanger.Instance.ChangeScene("DayScene");
        }

        public void LoadGame()
        {
            GameManager.Instance.LoadToData();
            SceneChanger.Instance.ChangeScene("DayScene");
        }
    }
}