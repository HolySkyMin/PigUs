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

        private void Awake()
        {
            for(int i = 0; i < GameManager.Instance.Save.DayLog.Count; i++)
            {
                var log = GameManager.Instance.Save.DayLog[i];
                var newObj = Instantiate(LogElement);
                newObj.GetComponent<ResultLogElement>().Set(log.Day, log.SelectedContext, log.DeltaValue);
                newObj.transform.SetParent(LogParent);
                newObj.transform.localScale = Vector3.one;
                newObj.SetActive(true);
            }
            LayoutRebuilder.MarkLayoutForRebuild(LogParent);
            LayoutRebuilder.ForceRebuildLayoutImmediate(LogParent);
        }
        
        public void GoToTitle()
        {
            SceneChanger.Instance.ChangeScene("TitleScene");
        }
    }
}