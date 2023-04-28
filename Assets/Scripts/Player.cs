using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
  public float moveSpeed = 5;
  public Crosshairs crosshairs;

  Camera viewCamera;
  PlayerController controller;
  GunController gunController;

  protected override void Start()
  {
    base.Start();

    controller = GetComponent<PlayerController>();
    gunController = GetComponent<GunController>();
    viewCamera = Camera.main;
  }

  void Update()
  {
    // Movement input
    var moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    var moveVelocity = moveInput.normalized * moveSpeed;
    controller.Move(moveVelocity);

    // Look input
    var ray = viewCamera.ScreenPointToRay(Input.mousePosition);
    var groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);

    if (groundPlane.Raycast(ray, out float rayDistance))
    {
      var point = ray.GetPoint(rayDistance);
      controller.LookAt(point);
      crosshairs.transform.position = point;
      crosshairs.DetectTargets(ray);

      if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 2.25)
      {
        gunController.Aim(point);
      }
    }

    // Weapon input
    if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
    {
      gunController.OnTriggerHold();
    }

    if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
    {
      gunController.OnTriggerRelease();
    }

    if (Input.GetKeyDown(KeyCode.R))
    {
      gunController.Reload();
    }
  }
}
