using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Check if there's already an instance of UIManager
        if (Instance != null && Instance != this)
        {
            // If there is another instance, destroy this one
            Destroy(gameObject);
        }
        else
        {
            // Set the instance to this UIManager and mark it as persistent
            Instance = this;
        }
    }

    public AudioSource audioSource;

    public void Start()
    {
        UpdateMusicState();
    }

    public void UpdateMusicState()
    {
        int musicOn = PlayerPrefs.GetInt("music", 1);
        audioSource.volume = musicOn == 1 ? 1f : 0;
    }
}