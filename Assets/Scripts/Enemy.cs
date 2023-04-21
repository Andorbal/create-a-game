using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
  public enum State
  {
    Idle,
    Chasing,
    Attacking,
  }

  public float pathfinderRefreshRate = 0.25f;

  State currentState;
  NavMeshAgent pathfinder;
  Transform target;
  Material skinMaterial;
  Color originalColor;

  float attackDistanceThreshold = 0.5f;
  float timeBetweenAttacks = 1;

  float nextAttackTime;
  float myCollisionRadius;
  float targetCollisionRadius;

  protected override void Start()
  {
    base.Start();

    pathfinder = GetComponent<NavMeshAgent>();
    skinMaterial = GetComponent<Renderer>().material;
    originalColor = skinMaterial.color;

    currentState = State.Chasing;
    target = GameObject.FindGameObjectWithTag("Player").transform;

    myCollisionRadius = GetComponent<CapsuleCollider>().radius;
    targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

    StartCoroutine(nameof(UpdatePath));
  }

  void Update()
  {
    if (Time.time > nextAttackTime)
    {
      var sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;

      if (sqrDistanceToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
      {
        nextAttackTime = Time.time + timeBetweenAttacks;
        StartCoroutine(nameof(Attack));
      }
    }
  }

  IEnumerator Attack()
  {
    currentState = State.Attacking;
    pathfinder.enabled = false;

    var originalPostion = transform.position;
    var dirToTarget = (target.position - transform.position).normalized;
    var attackPosition = target.position - dirToTarget * (myCollisionRadius);
    var attackSpeed = 3f;

    var percent = 0f;

    skinMaterial.color = Color.red;

    while (percent <= 1)
    {
      percent += Time.deltaTime * attackSpeed;
      var interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
      transform.position = Vector3.Lerp(originalPostion, attackPosition, interpolation);

      yield return null;
    }

    skinMaterial.color = originalColor;

    currentState = State.Chasing;
    pathfinder.enabled = true;
  }

  IEnumerator UpdatePath()
  {
    while (target != null)
    {
      if (currentState == State.Chasing)
      {
        var dirToTarget = (target.position - transform.position).normalized;
        var targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);

        if (!dead)
        {
          pathfinder.SetDestination(targetPosition);
        }
      }


      yield return new WaitForSeconds(pathfinderRefreshRate);
    }
  }
}
