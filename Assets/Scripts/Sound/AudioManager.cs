using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Make AudioManager accessible from anywhere
    public static AudioManager Instance; 

    // Accesses Sound Script
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource, ambienceSource;

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
        PlayMusic("MainTheme", "Ambience");
    }

    public void PlayMusic(string name, string ambient)
    {
        // Need a different audio soure for any clips that loop, such as music and ambience.

        // Find song with matching name in Sounds file
        Sound song = Array.Find(musicSounds, x => x.name == name);
        Sound ambience = Array.Find(musicSounds, x => x.name == ambient);

        // Check if song is found
        if (song == null)
        {
        }
        else
        {
            musicSource.clip = song.clip;
            musicSource.Play();
            musicSource.loop = true;
        }
        if (ambience == null)
        {
        }
        else
        {
            ambienceSource.clip = ambience.clip;
            ambienceSource.Play();
            ambienceSource.loop = true;
        }
    }

    public void PlaySFX(string name)
    {
        // Find song with matching name in Sounds file
        Sound s = Array.Find(sfxSounds, x=> x.name == name);

        // Check if song is found
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
