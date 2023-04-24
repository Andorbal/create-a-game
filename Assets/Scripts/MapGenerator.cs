using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
  public Map[] maps;
  [Min(0)]
  public int mapIndex;

  public Transform tilePrefab;
  public Transform obstaclePrefab;
  public Transform navmeshFloor;
  public Transform navmeshMaskPrefab;
  public Vector2 maxMapsize;

  [Range(0, 1)]
  public float outlinePercent;

  public float tileSize;

  Map currentMap;
  List<Coord> allTileCoords;
  Queue<Coord> shuffledTileCoords;
  Queue<Coord> shuffledOpenTileCoords;
  Transform[,] tileMap;

  void Start()
  {
    GenerateMap();
  }

  public void GenerateMap()
  {
    currentMap = maps[mapIndex];
    tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
    System.Random rand = new(currentMap.seed);
    GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, 0.05f, currentMap.mapSize.y * tileSize);

    // Generating coords
    allTileCoords = new List<Coord>();

    for (int x = 0; x < currentMap.mapSize.x; x += 1)
    {
      for (int y = 0; y < currentMap.mapSize.y; y += 1)
      {
        allTileCoords.Add(new Coord(x, y));
      }
    }
    shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));

    // Create map holder object
    string holderName = "Generated Map";
    if (transform.Find(holderName))
    {
      DestroyImmediate(transform.Find(holderName).gameObject);
    }

    var mapHolder = new GameObject(holderName).transform;
    mapHolder.parent = transform;

    // Spawning tiles
    for (int x = 0; x < currentMap.mapSize.x; x += 1)
    {
      for (int y = 0; y < currentMap.mapSize.y; y += 1)
      {
        var tilePosition = CoordToPosition(x, y);
        var newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90), mapHolder);
        newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
        tileMap[x, y] = newTile;
      }
    }

    // Spawning obstacles
    bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

    int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
    int currentObstacleCount = 0;
    var allOpenCoords = new List<Coord>(allTileCoords);

    for (int i = 0; i < obstacleCount; i += 1)
    {
      var randomCoord = GetRandomCoord();
      obstacleMap[randomCoord.x, randomCoord.y] = true;
      currentObstacleCount += 1;

      if (randomCoord != currentMap.mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
      {
        float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)rand.NextDouble());
        var obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y).WithY(obstacleHeight / 2);
        var newObstacle = Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity, mapHolder);
        newObstacle.localScale = (Vector3.one * (1 - outlinePercent) * tileSize).WithY(obstacleHeight);

        var obstacleRenderer = newObstacle.GetComponent<Renderer>();
        var obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
        var colorPercent = randomCoord.y / (float)currentMap.mapSize.y;
        obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
        obstacleRenderer.sharedMaterial = obstacleMaterial;

        allOpenCoords.Remove(randomCoord);
      }
      else
      {
        obstacleMap[randomCoord.x, randomCoord.y] = false;
        currentObstacleCount -= 1;
      }
    }
    shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));

    // Create navmesh mask
    var maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapsize.x) / 4f * tileSize, Quaternion.identity, mapHolder.transform);
    maskLeft.localScale = new Vector3((maxMapsize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

    var maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapsize.x) / 4f * tileSize, Quaternion.identity, mapHolder.transform);
    maskRight.localScale = new Vector3((maxMapsize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

    var maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapsize.y) / 4f * tileSize, Quaternion.identity, mapHolder.transform);
    maskTop.localScale = new Vector3(maxMapsize.x, 1, (maxMapsize.y - currentMap.mapSize.y) / 2f) * tileSize;

    var maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapsize.y) / 4f * tileSize, Quaternion.identity, mapHolder.transform);
    maskBottom.localScale = new Vector3(maxMapsize.x, 1, (maxMapsize.y - currentMap.mapSize.y) / 2f) * tileSize;

    navmeshFloor.localScale = new Vector3(maxMapsize.x, maxMapsize.y) * tileSize;
  }

  bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
  {
    var mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
    var queue = new Queue<Coord>();
    queue.Enqueue(currentMap.mapCenter);
    mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

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

    int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
    return targetAccessibleTileCount == accessibleTileCount;
  }

  Vector3 CoordToPosition(int x, int y)
  {
    return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
  }

  public Transform GetTileFromPosition(Vector3 position)
  {
    int x = Mathf.RoundToInt(position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
    int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);

    x = Mathf.Clamp(x, 0, tileMap.GetLength(0) - 1);
    y = Mathf.Clamp(y, 0, tileMap.GetLength(1) - 1);

    return tileMap[x, y];
  }

  public Coord GetRandomCoord()
  {
    var randomCoord = shuffledTileCoords.Dequeue();
    shuffledTileCoords.Enqueue(randomCoord);
    return randomCoord;
  }

  public Transform GetRandomOpenTile()
  {
    var randomCoord = shuffledOpenTileCoords.Dequeue();
    shuffledOpenTileCoords.Enqueue(randomCoord);
    return tileMap[randomCoord.x, randomCoord.y];
  }

  [Serializable]
  public class Coord
  {
    public int x;
    public int y;

    public Coord(int x, int y)
    {
      this.x = x;
      this.y = y;
    }

    public override bool Equals(object obj) =>
    obj is Coord coord &&
             x == coord.x &&
             y == coord.y;

    public override int GetHashCode() =>
      HashCode.Combine(x, y);

    public static bool operator ==(Coord left, Coord right) =>
      left?.x == right?.x && left?.y == right?.y;
    public static bool operator !=(Coord left, Coord right) =>
      !(left == right);
  }

  [Serializable]
  public class Map
  {
    public Coord mapSize;
    [Range(0, 1)]
    public float obstaclePercent;
    public int seed;
    public float minObstacleHeight;
    public float maxObstacleHeight;
    public Color foregroundColor;
    public Color backgroundColor;

    public Coord mapCenter
    {
      get => new Coord(mapSize.x / 2, mapSize.y / 2);
    }
  }
}
