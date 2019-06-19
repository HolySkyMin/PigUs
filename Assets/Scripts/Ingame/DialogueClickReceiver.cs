using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ingame
{
    public class DialogueClickReceiver : MonoBehaviour, IPointerDownHandler
    {
        public bool ReceivedClick { get; set; }
        public bool AllowedToReceive { get; set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(AllowedToReceive)
                ReceivedClick = true;
        }

        private void Update()
        {
            if (AllowedToReceive && Input.GetKeyDown(KeyCode.Space))
                ReceivedClick = true;
        }
    }
}