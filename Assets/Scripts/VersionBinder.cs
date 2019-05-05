using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionBinder : MonoBehaviour
{
    public Text VersionText;

    private void Start()
    {
        VersionText.text = Application.version;
    }
}
