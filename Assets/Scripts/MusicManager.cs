using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
  public AudioClip mainTheme;
  public AudioClip menuTheme;

  string sceneName;

  void Start()
  {
    OnLevelWasLoaded(0);
  }

  void OnLevelWasLoaded(int sceneIndex)
  {
    var newSceneName = SceneManager.GetActiveScene().name;

    if (newSceneName != sceneName)
    {
      sceneName = newSceneName;
      Invoke(nameof(PlayMusic), 0.2f);
    }
  }

  void PlayMusic()
  {
    AudioClip clipToPlay = null;
    if (sceneName == "Menu")
    {
      clipToPlay = menuTheme;
    }
    else if (sceneName == "Game")
    {
      clipToPlay = mainTheme;
    }

    if (clipToPlay != null)
    {
      AudioManager.Instance.PlayMusic(clipToPlay, 2);
    }
  }
}
