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
        public GameObject EndIndicator, UnskipIndicator;

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
            int mynum = IngameManager.MasterGameNum;

            while(!IsFullyAnimated)
            {
                if (mynum != IngameManager.MasterGameNum)
                    yield break;

                if (DialogueManager.Instance.Receiver.ReceivedClick)
                {
                    if (mynum != IngameManager.MasterGameNum)
                        yield break;
                    Context.text = context;
                    IsFullyAnimated = true;
                    DialogueManager.Instance.Receiver.ReceivedClick = false;
                }
                else
                {
                    if (mynum != IngameManager.MasterGameNum)
                        yield break;
                    if(Context != null)
                        Context.text = context.RichTextSubString(rtCount);
                    if (rtCount >= rtMaxCount)
                    {
                        IsFullyAnimated = true;
                        DialogueManager.Instance.Receiver.AllowedToReceive = true;
                    }
                    else
                    {
                        rtCount++;
                        yield return new WaitForSeconds(1f / GameManager.Instance.Config.CPS);
                        if (mynum != IngameManager.MasterGameNum)
                            yield break;
                    }
                }
            }
            if (mynum != IngameManager.MasterGameNum)
                yield break;
            EndIndicator.SetActive(true);
            yield return new WaitUntil(() => (DialogueManager.Instance.Receiver.ReceivedClick || mynum != IngameManager.MasterGameNum));
            if (mynum != IngameManager.MasterGameNum)
                yield break;
            DialogueManager.Instance.Receiver.ReceivedClick = false;
            EndIndicator.SetActive(false);
            UnskipIndicator.SetActive(false);
        }
    }
}