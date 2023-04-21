using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  public LayerMask collisionMask;
  float speed = 10;
  float damage = 1;

  float lifetime = 3;
  float skinWidth = .1f;

  public float Speed { set { speed = value; } }

  void Start()
  {
    Destroy(gameObject, lifetime);

    // Handle when the bullet spawns within an enemy
    var initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
    if (initialCollisions.Length > 0)
    {
      OnHitObject(initialCollisions[0]);
    }
  }

  void Update()
  {
    var moveDistance = Time.deltaTime * speed;
    CheckCollisions(moveDistance);
    transform.Translate(Vector3.forward * moveDistance);
  }

  void CheckCollisions(float moveDistance)
  {
    Ray ray = new(transform.position, transform.forward);
    if (Physics.Raycast(ray, out RaycastHit hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
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

  void OnHitObject(Collider c)
  {
    var damageableObject = c.GetComponent<IDamageable>();
    damageableObject?.TakeDamage(damage);

    GameObject.Destroy(gameObject);
  }
}
