using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Ingame
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        public DialogueDisplayer Displayer;
        public DialogueClickReceiver Receiver;
        public DialogueInteractor Interactor;

        private void Awake()
        {
            Instance = this;
        }

        public void ShowImage(string imageKey)
        {
            Displayer.CaseImage.sprite = Resources.Load<Sprite>($"Images/{imageKey}");
        }

        public async Task ShowDialogue(string talker, string context)
        {
            Receiver.AllowedToReceive = true;
            await Displayer.DisplayContext(talker, context);
            Receiver.AllowedToReceive = false;
        }

        public async Task<int> ShowInteraction(string[] sels)
        {
            Receiver.AllowedToReceive = false;
            Interactor.gameObject.SetActive(true);
            Interactor.Show(sels);
            await new WaitUntil(() => Interactor.HasResult);
            Interactor.gameObject.SetActive(false);
            return Interactor.Result;
        }

        public void CleanDialogue()
        {
            Displayer.Talker.text = "";
            Displayer.Context.text = "";
        }
    }
}