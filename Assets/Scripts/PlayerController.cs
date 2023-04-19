using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
  Vector3 velocity;
  Rigidbody myRigidbody;

  public void Start() {
    myRigidbody = GetComponent<Rigidbody>();
  }

  public void FixedUpdate() {
    myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);
  }

  public void Move(Vector3 moveVelocity)
  {
    velocity = moveVelocity;
  }
}
