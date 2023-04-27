using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
  public Transform weaponHold;
  public Gun startingGun;

  Gun equippedGun;

  void Start()
  {
    if (startingGun)
    {
      EquipGun(startingGun);
    }
  }

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
}
