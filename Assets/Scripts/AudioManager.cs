using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
  [Range(0, 1)]
  public float masterVolumePercent;
  [Range(0, 1)]
  public float sfxVolumePercent;
  [Range(0, 1)]
  public float musicVolumePercent;

  AudioSource[] musicSources;
  int activeMusicSourceIndex;
  Transform audioListenerTransform;
  Transform playerTransform;

  public static AudioManager Instance { get; private set; }

  public void PlayMusic(AudioClip clip, float fadeDuration = 1)
  {
    activeMusicSourceIndex = 1 - activeMusicSourceIndex;
    musicSources[activeMusicSourceIndex].clip = clip;
    musicSources[activeMusicSourceIndex].Play();

    StartCoroutine(nameof(AnimateMusicCrossfade), fadeDuration);
  }

  public void PlaySound(AudioClip clip, Vector3 position)
  {
    if (clip != null)
    {
      AudioSource.PlayClipAtPoint(clip, position, sfxVolumePercent * masterVolumePercent);
    }
  }

  void Awake()
  {
    audioListenerTransform = FindObjectOfType<AudioListener>().transform;
    playerTransform = FindObjectOfType<Player>().transform;

    musicSources = new AudioSource[2];
    for (int i = 0; i < musicSources.Length; i += 1)
    {
      var newMusicSource = new GameObject($"Music source {i + 1}");
      musicSources[i] = newMusicSource.AddComponent<AudioSource>();
      newMusicSource.transform.parent = transform;
    }

    Instance = this;
  }

  void Start()
  {

  }

  void Update()
  {
    if (playerTransform != null)
    {
      audioListenerTransform.position = playerTransform.position;
    }
  }

  IEnumerator AnimateMusicCrossfade(float duration)
  {
    float percent = 0;

    while (percent < 1)
    {
      percent += Time.deltaTime * 1 / duration;
      var maxVolume = musicVolumePercent * masterVolumePercent;
      musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, maxVolume, percent);
      musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(maxVolume, 0, percent);

      yield return null;
    }
  }
}
