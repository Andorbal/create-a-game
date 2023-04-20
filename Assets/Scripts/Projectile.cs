using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  public LayerMask collisionMask;
  float speed = 10;
  float damage = 1;

  public float Speed { set { speed = value; } }

  void Update()
  {
    var moveDistance = Time.deltaTime * speed;
    CheckCollisions(moveDistance);
    transform.Translate(Vector3.forward * moveDistance);
  }

  void CheckCollisions(float moveDistance)
  {
    Ray ray = new(transform.position, transform.forward);
    if (Physics.Raycast(ray, out RaycastHit hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
    {
      OnHitObject(hit);
    }
  }

  void OnHitObject(RaycastHit hit)
  {
    print(hit.collider.gameObject.name);
    var damageableObject = hit.collider.GetComponent<IDamageable>();
    damageableObject?.TakeHit(damage, hit);

    GameObject.Destroy(gameObject);
  }
}
