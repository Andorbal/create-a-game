using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
  public Transform tilePrefab;
  public Transform obstaclePrefab;
  public Transform navmeshFloor;
  public Transform navmeshMaskPrefab;
  public Vector2 mapSize;
  public Vector2 maxMapsize;

  [Range(0, 1)]
  public float outlinePercent;

  [Range(0, 1)]
  public float obstaclePercent;

  public float tileSize;

  public int seed;
  Coord mapCenter;

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
    mapCenter = new Coord((int)mapSize.x / 2, (int)mapSize.y / 2);

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
        newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
      }
    }

    bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

    int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
    int currentObstacleCount = 0;

    for (int i = 0; i < obstacleCount; i += 1)
    {
      var randomCoord = GetRandomCoord();
      obstacleMap[randomCoord.x, randomCoord.y] = true;
      currentObstacleCount += 1;

      if (randomCoord != mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
      {
        var obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y).WithY(0.5f);
        var newObstacle = Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity, mapHolder);
        newObstacle.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
      }
      else
      {
        obstacleMap[randomCoord.x, randomCoord.y] = false;
        currentObstacleCount -= 1;
      }
    }

    // Create navmesh mask
    var maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (mapSize.x + maxMapsize.x) / 4 * tileSize, Quaternion.identity, mapHolder.transform);
    maskLeft.localScale = new Vector3((maxMapsize.x - mapSize.x) / 2, 1, mapSize.y) * tileSize;

    var maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (mapSize.x + maxMapsize.x) / 4 * tileSize, Quaternion.identity, mapHolder.transform);
    maskRight.localScale = new Vector3((maxMapsize.x - mapSize.x) / 2, 1, mapSize.y) * tileSize;

    var maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (mapSize.y + maxMapsize.y) / 4 * tileSize, Quaternion.identity, mapHolder.transform);
    maskTop.localScale = new Vector3(maxMapsize.x, 1, (maxMapsize.y - mapSize.y) / 2) * tileSize;

    var maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (mapSize.y + maxMapsize.y) / 4 * tileSize, Quaternion.identity, mapHolder.transform);
    maskBottom.localScale = new Vector3(maxMapsize.x, 1, (maxMapsize.y - mapSize.y) / 2) * tileSize;


    navmeshFloor.localScale = new Vector3(maxMapsize.x, maxMapsize.y) * tileSize;
  }

  bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
  {
    var mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
    var queue = new Queue<Coord>();
    queue.Enqueue(mapCenter);
    mapFlags[mapCenter.x, mapCenter.y] = true;

    int accessibleTileCount = 1;

    // Flood fill algorithm
    while (queue.Count > 0)
    {
      var tile = queue.Dequeue();

      for (int x = -1; x <= 1; x += 1)
      {
        for (int y = -1; y <= 1; y += 1)
        {
          var neighborX = tile.x + x;
          var neighborY = tile.y + y;

          if (x == 0 || y == 0)
          {
            if (neighborX >= 0 && neighborX < obstacleMap.GetLength(0) &&
              neighborY >= 0 && neighborY < obstacleMap.GetLength(1))
            {
              if (!mapFlags[neighborX, neighborY] && !obstacleMap[neighborX, neighborY])
              {
                mapFlags[neighborX, neighborY] = true;
                queue.Enqueue(new Coord(neighborX, neighborY));
                accessibleTileCount += 1;
              }
            }
          }
        }
      }
    }

    int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);
    return targetAccessibleTileCount == accessibleTileCount;
  }

  Vector3 CoordToPosition(int x, int y)
  {
    return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y) * tileSize;
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
