using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
  public float pathfinderRefreshRate = 0.25f;

  NavMeshAgent pathfinder;
  Transform target;

  void Start()
  {
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
      pathfinder.SetDestination(targetPosition);
      yield return new WaitForSeconds(pathfinderRefreshRate);
    }
  }
}
