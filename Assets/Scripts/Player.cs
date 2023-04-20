using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
  public float moveSpeed = 5;

  Camera viewCamera;
  PlayerController controller;

    void Start()
    {
        controller = GetComponent<PlayerController>();
        viewCamera = Camera.main;
    }

    void Update()
    {
      var moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));        
      var moveVelocity = moveInput.normalized * moveSpeed;
      controller.Move(moveVelocity);

      var ray = viewCamera.ScreenPointToRay(Input.mousePosition);
      var groundPlane = new Plane(Vector3.up, Vector3.zero);
      
      if (groundPlane.Raycast(ray, out float rayDistance)) {
        var point = ray.GetPoint(rayDistance);
        //Debug.DrawLine(ray.origin, point, Color.red);
        controller.LookAt(point);
      }
    }
}
