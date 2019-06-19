using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Result
{
    public class ResultManager : MonoBehaviour
    {
        public GameObject LogElement;
        public RectTransform LogParent;
        public Text DescText;
        public Button Phase2Btn;

        private void Awake()
        {
            for(int i = 0; i < GameManager.Instance.Save.DayLog.Count; i++)
            {
                var log = GameManager.Instance.Save.DayLog[i];
                var newObj = Instantiate(LogElement);
                newObj.GetComponent<ResultLogElement>().Set(log.Phase, log.Day, log.SelectedContext, log.DeltaValue);
                newObj.transform.SetParent(LogParent);
                newObj.transform.localScale = Vector3.one;
                newObj.SetActive(true);
            }
            LayoutRebuilder.MarkLayoutForRebuild(LogParent);
            LayoutRebuilder.ForceRebuildLayoutImmediate(LogParent);

            if (GameManager.Instance.Save.IsGameOver)
                DescText.text = "아무래도 좋은 결과를 얻지는 못한 것 같네요.\n\n돌아간다면, 아직 기회는 있을지도 모릅니다.";
            else
            {
                if (GameManager.Instance.Save.CurrentStoryType == 1)
                    DescText.text = "구구는 인간과 함께하는 길을 골라\n끝까지 살아남았습니다.\n그 결과가 나중에 어떻게 다가오든 말이죠...\n\n혹시 다른 선택을 원하시나요?";
                else
                    DescText.text = "구구는 돼지들의 반란의 선봉에 서서\n돼지로서의 자유를 되찾았습니다.\n그 결과가 나중에 어떻게 다가오든 말이죠...\n\n혹시 다른 선택을 원하시나요?";
            }

            if (GameManager.Phase2Exists)
                Phase2Btn.interactable = true;
            else
                Phase2Btn.interactable = false;
        }

        private void Start()
        {
            SoundManager.Instance.PlayBgm("Game Over");
        }

        public void RetryPhase2()
        {
            GameManager.Instance.LoadPhase2();
            SceneChanger.Instance.ChangeScene("DayScene");
        }

        public void RetryLastDay()
        {
            GameManager.Instance.LoadToData();
            SceneChanger.Instance.ChangeScene("DayScene");
        }

        public void GoToTitle()
        {
            SceneChanger.Instance.ChangeScene("TitleScene");
        }
    }
}