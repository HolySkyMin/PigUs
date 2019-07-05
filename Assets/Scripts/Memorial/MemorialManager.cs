using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Memorial
{
    public class MemorialManager : MonoBehaviour
    {
        public MemorialEndingBtn[] Endings;
        public Image EndingImage;
        public Text EndingIndex, EndingTitle, EndingRule, EndingDesc;

        private int lastSelectedIndex = 0;

        private void Start()
        {
            LoadEndingInfo();
        }

        public void LoadEndingInfo()
        {
            foreach(var ending in GameManager.Instance.Config.EndingChecklist)
            {
                if (ending.Value == true)
                    Endings[ending.Key].Activate();
            }
        }

        public void ShowEnding(int index)
        {
            if (lastSelectedIndex > 0)
                Endings[lastSelectedIndex].SelectedBorder.SetActive(false);
            Endings[index].SelectedBorder.SetActive(true);
            EndingImage.sprite = Endings[index].EndingSprite;
            EndingIndex.text = $"결말 #{index}";
            EndingTitle.text = Endings[index].EndingTitle;
            EndingRule.text = Endings[index].EndingRule;
            EndingDesc.text = Endings[index].EndingDescription;
            lastSelectedIndex = index;
        }

        public void GoBack()
        {
            SceneChanger.Instance.ChangeScene("TitleScene");
        }
    }
}