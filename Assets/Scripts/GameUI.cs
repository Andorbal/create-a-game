using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
  public Image fadePlane;
  public GameObject gameOverUI;
  public RectTransform newWaveBanner;
  public TMP_Text newWaveTitle;
  public TMP_Text newWaveEnemyCount;

  Spawner spawner;

  string[] numbers = { "One", "Two", "Three", "Foun", "Five" };

  void Awake()
  {
    spawner = FindObjectOfType<Spawner>();
    spawner.OnNewWave += OnNewWave;
  }

  void Start()
  {
    FindObjectOfType<Player>().OnDeath += OnGameOver;
  }

  void OnNewWave(int waveNumber)
  {
    newWaveTitle.text = $"- Wave {numbers[waveNumber - 1]} -";
    var enemyCount = spawner.waves[waveNumber - 1].infinite ?
      "Infinite" :
      spawner.waves[waveNumber - 1].enemyCount.ToString();
    newWaveEnemyCount.text = $"Enemies: {enemyCount}";

    StopCoroutine(nameof(AnimateNewWaveBanner));
    StartCoroutine(nameof(AnimateNewWaveBanner));
  }

  void OnGameOver()
  {
    StartCoroutine(Fade(Color.clear, Color.black, 1));
    gameOverUI.SetActive(true);
  }

  IEnumerator AnimateNewWaveBanner()
  {
    var delayTime = 1.5f;
    var speed = 3f;
    var animationPercent = 0f;
    var dir = 1;
    var endDelayTime = Time.time + 1 / speed + delayTime;

    while (animationPercent >= 0)
    {
      animationPercent += Time.deltaTime * speed * dir;

      if (animationPercent >= 1)
      {
        animationPercent = 1;
        if (Time.time > endDelayTime)
        {
          dir = -1;
        }
      }

      newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-170, 30, animationPercent);
      yield return null;
    }
  }

  IEnumerator Fade(Color from, Color to, float time)
  {
    var speed = 1 / time;
    var percent = 0f;

    while (percent <= 1)
    {
      percent += Time.deltaTime * speed;
      fadePlane.color = Color.Lerp(from, to, percent);
      yield return null;
    }
  }

  public void StartNewGame()
  {
    SceneManager.LoadScene("Game");
  }
}
