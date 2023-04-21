using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
  public Transform tilePrefab;
  public Vector2 mapSize;

  [Range(0, 1)]
  public float outlinePercent;

  void Start()
  {
    GenerateMap();
  }

  public void GenerateMap()
  {
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
        var tilePosition = new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
        var newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90), mapHolder);
        newTile.localScale = Vector3.one * (1 - outlinePercent);
      }
    }
  }
}
