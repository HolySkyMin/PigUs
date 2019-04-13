using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ingame
{
    public class DialogueInteractButton : MonoBehaviour
    {
        public int Index;
        public Text Context;

        public void Set(string context)
        {
            Context.text = context;
        }

        public void Clicked()
        {
            DialogueManager.Instance.Interactor.SetResult(Index);
        }
    }
}