using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using RichTextSubstringHelper;

namespace Ingame
{
    public class DialogueDisplayer : MonoBehaviour
    {
        public bool IsFullyAnimated { get; private set; }

        public Image CaseImage;
        public Text Talker, Context;
        public GameObject EndIndicator;

        public async Task DisplayContext(string talker, string context)
        {
            IsFullyAnimated = false;

            Talker.text = talker;
            await TextAnimating(context);
        }

        IEnumerator TextAnimating(string context)
        {
            int rtMaxCount = context.RichTextLength();
            int rtCount = 0;

            while(!IsFullyAnimated)
            {
                if (DialogueManager.Instance.Receiver.ReceivedClick)
                {
                    Context.text = context;
                    IsFullyAnimated = true;
                    DialogueManager.Instance.Receiver.ReceivedClick = false;
                }
                else
                {
                    Context.text = context.RichTextSubString(rtCount);
                    if (rtCount >= rtMaxCount)
                        IsFullyAnimated = true;
                    else
                    {
                        rtCount++;
                        yield return new WaitForSeconds(1f / 59);
                    }
                }
            }
            EndIndicator.SetActive(true);
            yield return new WaitUntil(() => DialogueManager.Instance.Receiver.ReceivedClick);
            DialogueManager.Instance.Receiver.ReceivedClick = false;
            EndIndicator.SetActive(false);
        }
    }
}