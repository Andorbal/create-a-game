using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
  public enum FireMode
  {
    Auto,
    Burst,
    Single
  }

  public FireMode fireMode;
  public Transform[] projectileSpawn;
  public Projectile projectile;
  public float msBetweenShots = 0.100f;
  public float muzzleVelocity = 35;
  public int burstCount;

  public Transform shell;
  public Transform shellEjection;

  MuzzleFlash muzzleFlash;
  float nextShotTime;
  bool triggerReleasedSinceLastShot;
  int shotsRemainingInBurst;

  void Start()
  {
    shotsRemainingInBurst = burstCount;
    muzzleFlash = GetComponent<MuzzleFlash>();
  }

  void Shoot()
  {
    if (Time.time > nextShotTime)
    {
      if (fireMode == FireMode.Burst)
      {
        if (shotsRemainingInBurst == 0)
        {
          return;
        }
        shotsRemainingInBurst -= 1;
      }
      else if (fireMode == FireMode.Single)
      {
        if (!triggerReleasedSinceLastShot)
        {
          return;
        }
      }

      for (int i = 0; i < projectileSpawn.Length; i += 1)
      {
        nextShotTime = Time.time + msBetweenShots;
        var newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation);
        newProjectile.Speed = muzzleVelocity;
      }

      Instantiate(shell, shellEjection.position, shellEjection.rotation);
      muzzleFlash.Activate();
    }
  }

  public void OnTriggerHold()
  {
    Shoot();
    triggerReleasedSinceLastShot = false;
  }

  public void OnTriggerRelease()
  {
    triggerReleasedSinceLastShot = true;
    shotsRemainingInBurst = burstCount;
  }
}
