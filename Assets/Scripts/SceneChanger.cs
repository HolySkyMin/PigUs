using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }

    public Animator ChangeAnim;

    private void Awake()
    {
        Instance = this;
    }

    public async void ChangeScene(string sceneName, float duration = 0.34f)
    {
        ChangeAnim.Play("SceneFadeIn");
        await new WaitForSeconds(duration);
        await SceneManager.LoadSceneAsync(sceneName);
    }
}
