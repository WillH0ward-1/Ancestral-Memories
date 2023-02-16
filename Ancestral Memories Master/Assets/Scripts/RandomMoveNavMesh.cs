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
        navMeshAgent = transform.GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = false;
    }

    private void OnEnable()
    {
    navMeshAgent.enabled = true;
    StartCoroutine(UpdateDirection());
    }

    private IEnumerator UpdateDirection()
    {
        navMeshAgent.SetDestination(RandomNavmeshLocation(radius));

        yield return new WaitForSeconds(Random.Range(minWait, maxWait));
        StartCoroutine(UpdateDirection());

        yield break;
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
