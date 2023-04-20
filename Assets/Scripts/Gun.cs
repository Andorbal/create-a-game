using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
  public Transform muzzle;
  public Projectile projectile;
  public float msBetweenShots = 0.100f;
  public float muzzleVelocity = 35;

  float nextShotTime;

  public void Shoot()
  {
    if (Time.time > nextShotTime)
    {
      nextShotTime = Time.time + msBetweenShots;
      var newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
      newProjectile.Speed = muzzleVelocity;
    }
  }
}
