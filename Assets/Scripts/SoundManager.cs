using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource BgmPlayer, SePlayer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void PlayBgm(string audioKey)
    {
        BgmPlayer.clip = Resources.Load<AudioClip>($"Sounds/{audioKey}");
        BgmPlayer.Play();
    }

    public void PlaySe(string audioKey)
    {
        SePlayer.PlayOneShot(Resources.Load<AudioClip>($"Sounds/{audioKey}"));
        //SePlayer.PlayOneShot((AudioClip)(await Resources.LoadAsync($"Sounds/{audioKey}")));
    }
}
