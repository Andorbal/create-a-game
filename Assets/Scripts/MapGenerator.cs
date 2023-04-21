using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
  public Transform tilePrefab;
  public Transform obstaclePrefab;
  public Vector2 mapSize;

  [Range(0, 1)]
  public float outlinePercent;

  public int seed;

  List<Coord> allTileCoords;
  Queue<Coord> shuffledTileCoords;

  void Start()
  {
    GenerateMap();
  }

  public void GenerateMap()
  {
    allTileCoords = new List<Coord>();

    for (int x = 0; x < mapSize.x; x += 1)
    {
      for (int y = 0; y < mapSize.y; y += 1)
      {
        allTileCoords.Add(new Coord(x, y));
      }
    }
    shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));

    string holderName = "Generated Map";
    if (transform.Find(holderName))
    {
      DestroyImmediate(transform.Find(holderName).gameObject);
    }

    var mapHolder = new GameObject(holderName).transform;
    mapHolder.parent = transform;

    for (int x = 0; x < mapSize.x; x += 1)
    {
      for (int y = 0; y < mapSize.y; y += 1)
      {
        var tilePosition = CoordToPosition(x, y);
        var newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90), mapHolder);
        newTile.localScale = Vector3.one * (1 - outlinePercent);
      }
    }

    int obstacleCount = 10;
    for (int i = 0; i < obstacleCount; i += 1)
    {
      var randomCoord = GetRandomCoord();
      var obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y).WithY(0.5f);
      var newObstacle = Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity, mapHolder);
    }
  }

  Vector3 CoordToPosition(int x, int y)
  {
    return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
  }

  public Coord GetRandomCoord()
  {
    var randomCoord = shuffledTileCoords.Dequeue();
    shuffledTileCoords.Enqueue(randomCoord);
    return randomCoord;
  }

  [Serializable]
  public record Coord(int x, int y);
}
