using System.Collections;
using UnityEngine;
using Pathfinding;

public class RandomMoveAStar : MonoBehaviour
{
    [SerializeField] private float radius = 10f;
    [SerializeField] private float maxTravelRadius = 20f; // The maximum radius the AI can move from its starting position
    [SerializeField] private float minWait = 10f;
    [SerializeField] private float maxWait = 50f;
    [SerializeField] private float defaultStoppingDistance = 1f;

    private RichAI ai;
    private Vector3 startPosition; // To keep track of the starting position

    private void Awake()
    {
        ai = GetComponent<RichAI>();

        ai = transform.GetComponentInChildren<RichAI>();
        ai.endReachedDistance = defaultStoppingDistance;
        ai.acceleration = 10000;

        startPosition = transform.position; // Save the starting position
    }

    private void OnEnable()
    {
        StartCoroutine(UpdateDirection());
    }

    private IEnumerator UpdateDirection()
    {
        while (true)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += transform.position;

            // Ensure the destination is within the maxTravelRadius from the startPosition
            if ((randomDirection - startPosition).sqrMagnitude > maxTravelRadius * maxTravelRadius)
            {
                // If outside maxTravelRadius, adjust to be within the limit
                randomDirection = startPosition + (randomDirection - startPosition).normalized * maxTravelRadius;
            }

            Vector3 finalPosition = randomDirection;

            ai.destination = finalPosition;

            yield return new WaitForSeconds(Random.Range(minWait, maxWait));
        }
    }
}
