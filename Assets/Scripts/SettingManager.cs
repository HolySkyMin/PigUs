using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public GameObject FullscreenPanel;
    public Slider CPSslider;
    public Toggle SkipToggle, FullscreenToggle;
    public Text CPStext;

    public void LoadConfig()
    {
        CPSslider.value = GameManager.Instance.Config.CPS;
        SkipToggle.isOn = GameManager.Instance.Config.SkipTutorial;
        FullscreenToggle.isOn = GameManager.Instance.Config.AllowFullscreen;

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            FullscreenPanel.SetActive(false);
        CPSChanging();
    }

    public void CPSChanging()
    {
        CPStext.text = CPSslider.value.ToString();
    }

    public void SaveChange()
    {
        GameManager.Instance.Config.CPS = (int)CPSslider.value;
        GameManager.Instance.Config.SkipTutorial = SkipToggle.isOn;
        GameManager.Instance.Config.AllowFullscreen = FullscreenToggle.isOn;
        GameManager.Instance.SaveConfig();
    }

    public void ResetGameProgress()
    {
        GameManager.Instance.Config.EndingChecklist = new Dictionary<int, bool>()
        {
            { 1, false },
            { 2, false },
            { 3, false },
            { 4, false },
            { 5, false },
            { 6, false },
            { 7, false },
            { 8, false },
            { 9, false },
            { 10, false },
            { 11, false },
            { 12, false },
            { 13, false },
            { 14, false },
            { 15, false },
            { 16, false },
            { 17, false },
            { 18, false },
            { 19, false },
            { 20, false }
        };
        GameManager.Instance.SaveConfig();
    }
}
