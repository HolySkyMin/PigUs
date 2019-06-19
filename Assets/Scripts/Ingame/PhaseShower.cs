using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Ingame
{
    public class PhaseShower : MonoBehaviour
    {
        public int StoryType;

        private float clock, originZ;

        private void Start()
        {
            originZ = gameObject.transform.localEulerAngles.z;
        }

        private void Update()
        {
            if (IngameManager.Instance.Data.StoryPhase == 0 && IngameManager.Instance.Data.Phase1Progress[StoryType - 1] == 5)
            {
                gameObject.transform.localEulerAngles = new Vector3(0, 0, originZ + 10 * Mathf.Sin(clock));
                clock += 3 * Mathf.PI * Time.deltaTime;
                if (clock > 2 * Mathf.PI)
                    clock -= 2 * Mathf.PI;
            }
            else
            {
                clock = 0;
                gameObject.transform.localEulerAngles = new Vector3(0, 0, originZ);
            }

            if (IngameManager.Instance.Data.StoryPhase > -5 && IngameManager.Instance.Data.StoryPhase != 0 && IngameManager.Instance.Data.StoryPhase != StoryType)
                gameObject.SetActive(false);
        }
    }
}