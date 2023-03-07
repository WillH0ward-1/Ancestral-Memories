using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomMoveNavMesh : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private float radius = 10f;
    [SerializeField] private float minWait = 10f;
    [SerializeField] private float maxWait = 50f;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = false;
    }

    private void OnEnable()
    {
        navMeshAgent.enabled = true;
        StartCoroutine(UpdateDirection());
    }

    private IEnumerator UpdateDirection()
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas);
        Vector3 finalPosition = hit.position;

        navMeshAgent.SetDestination(finalPosition);

        yield return new WaitForSeconds(Random.Range(minWait, maxWait));

        StartCoroutine(UpdateDirection());

        yield break;
    }
}