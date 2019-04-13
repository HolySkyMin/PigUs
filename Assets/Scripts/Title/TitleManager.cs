using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Title
{
    public class TitleManager : MonoBehaviour
    {
        public Button NewGameBtn, LoadGameBtn;

        private void Start()
        {
            if (GameManager.SaveFileExists)
                LoadGameBtn.interactable = true;
            else
                LoadGameBtn.interactable = false;
        }

        public void StartGame()
        {
            GameManager.Instance.CreateData();
            SceneChanger.Instance.ChangeScene("DayScene");
        }

        public void LoadGame()
        {
            GameManager.Instance.LoadToData();
            SceneChanger.Instance.ChangeScene("DayScene");
        }
    }
}