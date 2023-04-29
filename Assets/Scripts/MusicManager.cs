using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
  public AudioClip mainTheme;
  public AudioClip menuTheme;

  void Start()
  {
    AudioManager.Instance.PlayMusic(menuTheme, 2);
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      AudioManager.Instance.PlayMusic(mainTheme, 3);
    }
  }
}
