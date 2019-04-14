using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptedAnimation;
using System.Threading.Tasks;

#pragma warning disable 4014
namespace Animation
{
    public class DialogueBodyAnim : ScriptAnimation
    {
        public async override Task Appear()
        {
            SetCurrentAsOrigin();
            TweenPosition(OriginPos + new Vector3(0, 30, 0), OriginPos, 0.3f);
            await TweenCanvasGroup(0, 1, 0.3f);
        }

        public async override Task Disappear()
        {
            TweenPosition(OriginPos, OriginPos - new Vector3(0, 30, 0), 0.3f);
            await TweenCanvasGroup(1, 0, 0.3f);
            SetOriginAsCurrent();
        }
    }
}