using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    // Make AudioManager accessible from anywhere
    public static AudioManager Instance; 

    // Accesses Sound Script
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        PlayMusic("MainTheme");
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        
        // Check if Sound file can be found
        if (s == null)
        {
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x=> x.name == name);

        if (s == null)
        {
        }
        else
        {
            // Play Sound effect once
            sfxSource.PlayOneShot(s.clip);
        }
    }
}
