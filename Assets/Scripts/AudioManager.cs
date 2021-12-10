using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
  AudioSource audioSource;
  public const float DEFAULT_VOLUME = .5f;
  public const string VOLUME_LEVEL_KEY = "VolumeLevel";

  // Start is called before the first frame update
  void Start()
  {
    audioSource = GetComponent<AudioSource>();

    float volume = PlayerPrefs.GetFloat(VOLUME_LEVEL_KEY, DEFAULT_VOLUME);
    audioSource.volume = volume;

    DontDestroyOnLoad(gameObject);
  }

  // Update is called once per frame
  void Update()
  {
    
  }

  public void AdjustVolume(float volumeLevel)
  {
    audioSource.volume = volumeLevel;
    PlayerPrefs.SetFloat(VOLUME_LEVEL_KEY, volumeLevel);
  }
}
