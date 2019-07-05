using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Memorial
{
    public class MemorialEndingBtn : MonoBehaviour
    {
        public GameObject UnlockedImage;
        public GameObject SelectedBorder;
        public Sprite EndingSprite;
        public string EndingTitle;
        public string EndingRule;
        [TextArea]
        public string EndingDescription;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Activate()
        {
            UnlockedImage.SetActive(true);
            gameObject.GetComponent<Button>().interactable = true;
        }
    }
}