using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ingame
{
    public class PhaseShower : MonoBehaviour
    {
        public Graphic TargetGraphic;

        void Update()
        {
            switch(IngameManager.Instance.Data.CurrentStoryType)
            {
                case 0:
                    TargetGraphic.color = Color.white;
                    break;
                case 1:
                    TargetGraphic.color = Color.cyan;
                    break;
                case 2:
                    TargetGraphic.color = Color.yellow;
                    break;
                default:
                    TargetGraphic.color = Color.red;
                    break;
            }
        }
    }
}