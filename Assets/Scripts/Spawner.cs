using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
  [Serializable]
  public class Wave
  {
    public int enemyCount;
    public float timeBetweenSpawns;
  }

  public Wave[] waves;
  public Enemy enemy;

  Wave currentWave;
  int currentWaveNumber;
  int enemiesRemainingToSpawn;
  int enemiesRemainingAlive;
  float nextSpawnTime;

  void Start()
  {
    NextWave();
  }

  void Update()
  {
    if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
    {
      enemiesRemainingToSpawn -= 1;
      nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

      var spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity);
      spawnedEnemy.OnDeath += OnEnemyDeath;
    }
  }

  void NextWave()
  {
    currentWaveNumber += 1;

    print($"Wave {currentWaveNumber}");
    if (currentWaveNumber - 1 < waves.Length)
    {
      currentWave = waves[currentWaveNumber - 1];

      enemiesRemainingAlive = currentWave.enemyCount;
      enemiesRemainingToSpawn = currentWave.enemyCount;
    }
  }

  void OnEnemyDeath()
  {
    enemiesRemainingAlive -= 1;

    if (enemiesRemainingAlive <= 0)
    {
      NextWave();
    }
  }
}
