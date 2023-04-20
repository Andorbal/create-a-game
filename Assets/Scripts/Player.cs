using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : MonoBehaviour
{
  public float moveSpeed = 5;

  Camera viewCamera;
  PlayerController controller;
  GunController gunController;

  void Start()
  {
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
    var groundPlane = new Plane(Vector3.up, Vector3.zero);

    if (groundPlane.Raycast(ray, out float rayDistance))
    {
      var point = ray.GetPoint(rayDistance);
      //Debug.DrawLine(ray.origin, point, Color.red);
      controller.LookAt(point);
    }

    // Weapon input
    if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
    {
      gunController.Shoot();
    }
  }
}
