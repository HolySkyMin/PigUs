using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class ScreenScaleHelper : MonoBehaviour
{
    private void Awake()
    {
        var curState = Screen.currentResolution;
        if (curState.width / (float)curState.height >= 16f / 9)
            GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
        else
            GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
    }
}
