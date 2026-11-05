using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public AudioSource walkingSound;
    public AudioSource jumpSound;
    public AudioSource swordSound;
    public AudioSource deathSound;
    public AudioSource BGSound;
    public AudioSource glassSound;
    public AudioSource ShieldSound;
    public AudioSource voiceSound;
    public AudioSource bowSound;
    private void Awake()
    {
        if(Instance!=null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlaySound(AudioSource soundToPlay)
    {
        if (!soundToPlay.isPlaying)
        {
            soundToPlay.Play();
        }
    }
    public void PlayVoiceSound(AudioClip clip)
    {
        voiceSound.clip = clip;
        if (!voiceSound.isPlaying)
        {
            voiceSound.Play();
        }
        else
        {
            voiceSound.Stop();
            voiceSound.Play();
        }
    }

}
