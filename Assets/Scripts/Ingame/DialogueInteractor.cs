using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ingame
{
    public class DialogueInteractor : MonoBehaviour
    {
        public int Result { get; private set; }
        public bool HasResult { get; private set; }

        public DialogueInteractButton[] Buttons;

        public void Show(string[] contexts)
        {
            Result = -1;
            HasResult = false;

            for (int i = 0; i < contexts.Length; i++)
                Buttons[i].Set(contexts[i]);
        }

        public void SetResult(int index)
        {
            Result = index;
            HasResult = true;
        }
    }
}