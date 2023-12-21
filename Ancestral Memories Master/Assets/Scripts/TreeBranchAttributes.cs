using System.Collections;
using UnityEngine;

public class TreeBranchAttributes : MonoBehaviour
{
    public bool isAvaliable = false;
    public float hungerReplenishFactor = 0.25f;
    public ResourcesManager resources;

    [Header("Launch Settings")]
    [SerializeField] private LayerMask hitGroundLayer;
    [SerializeField] private float minLaunchRadius = 5f;
    [SerializeField] private float maxLaunchRadius = 10f;
    [SerializeField] private float launchDuration = 2.0f;
    [SerializeField] private float launchHeight = 10f;
    [SerializeField] private float rotationSpeed = 90f;

    private void Awake()
    {
        hitGroundLayer = LayerMask.GetMask("Ground", "Water");
    }

    public void LaunchBranch()
    {
        StartCoroutine(LaunchAndFall());
    }

    private IEnumerator LaunchAndFall()
    {
        transform.SetParent(null);
        isAvaliable = false;

        Vector3 startPosition = transform.position;
        Vector3 peakPosition = startPosition + Vector3.up * launchHeight; // Peak of the trajectory

        // Calculate a random end position within a radius on the ground
        Vector3 randomDirection = Random.rotation * Vector3.forward;
        float randomRadius = Random.Range(minLaunchRadius, maxLaunchRadius);
        Vector3 endPosition = startPosition + randomDirection * randomRadius;

        // Perform a raycast to find the ground position for the end position
        if (Physics.Raycast(endPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, hitGroundLayer))
        {
            endPosition = hit.point;
        }
        else
        {
            Debug.LogError("Raycast did not hit any ground layer.");
            yield break;
        }

        float startTime = Time.time;
        while (Time.time - startTime < launchDuration)
        {
            float fractionOfJourney = (Time.time - startTime) / launchDuration;
            float parabola = (-4 * launchHeight * fractionOfJourney) + (4 * launchHeight * fractionOfJourney * fractionOfJourney);
            Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, fractionOfJourney) + new Vector3(0, parabola, 0);

            transform.position = currentPosition;

            // Add rotation for a dynamic effect
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

            yield return null;
        }

        transform.position = endPosition; // Ensure it lands exactly at the end position
        isAvaliable = true; // Set as available for collection or interaction
        resources.AddResourceObject("Wood", transform.gameObject);
    }
}
