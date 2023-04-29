using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
  public Transform weaponHold;
  public Gun[] allGuns;

  Gun equippedGun;

  void Start()
  {
  }

  public void EquipGun(int weaponIndex) =>
    EquipGun(allGuns[weaponIndex]);

  public void EquipGun(Gun gunToEquip)
  {
    if (equippedGun)
    {
      Destroy(equippedGun);
    }

    equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation, weaponHold.transform);
  }

  public void OnTriggerHold()
  {
    equippedGun?.OnTriggerHold();
  }

  public void OnTriggerRelease() => equippedGun?.OnTriggerRelease();
  public float GunHeight { get => weaponHold.position.y; }

  public void Aim(Vector3 aimPoint)
  {
    equippedGun?.Aim(aimPoint);
  }

  public void Reload() => equippedGun?.Reload();
}
