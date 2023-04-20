using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
  public float pathfinderRefreshRate = 0.25f;

  NavMeshAgent pathfinder;
  Transform target;

  protected override void Start()
  {
    base.Start();

    pathfinder = GetComponent<NavMeshAgent>();
    target = GameObject.FindGameObjectWithTag("Player").transform;

    StartCoroutine(nameof(UpdatePath));
  }

  void Update()
  {

  }

  IEnumerator UpdatePath()
  {
    while (target != null)
    {
      var targetPosition = target.position.WithY(0);

      if (!dead)
      {
        pathfinder.SetDestination(targetPosition);
      }

      yield return new WaitForSeconds(pathfinderRefreshRate);
    }
  }
}
