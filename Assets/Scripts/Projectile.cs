using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  float speed = 10;

  public float Speed { set { speed = value; } }

  void Update()
  {
    Debug.Log($"{speed}");
    transform.Translate(Vector3.forward * Time.deltaTime * speed);
  }
}
