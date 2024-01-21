using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
[RequireComponent(typeof(SphereCollider))]
public class BuildModeValidator : MonoBehaviour
{
    public LayerMask invalidLayers; // Set this in the inspector to the layers that should invalidate the build area.
    public DecalProjector decalProjector; // Assign the DecalProjector in the inspector.

    private SphereCollider sphereCollider;
    private bool isValidBuildArea = true;
    private Vector3 lastKnownPosition;
    private float raycastStartHeight = 200f; // Height from which to start the raycast

    private void OnEnable()
    {
        sphereCollider = GetComponent<SphereCollider>();
        decalProjector = GetComponent<DecalProjector>();
        lastKnownPosition = transform.position;
        PositionColliderAtGroundLevel();
    }

    private bool isPositioning = false; // Flag to prevent repositioning loop

    private void Update()
    {
        if (!isPositioning && HasSignificantlyMoved())
        {
            isPositioning = true;
            PositionColliderAtGroundLevel();
            isPositioning = false;
        }

        if (!Application.isPlaying)
        {
            CheckForInvalidLayers();
        }
    }

    private bool HasSignificantlyMoved()
    {
        Vector3 currentColliderCenterWorldPosition = transform.TransformPoint(sphereCollider.center);
        return Vector3.Distance(transform.position, lastKnownPosition) > 0.01f;
    }

    private void PositionColliderAtGroundLevel()
    {
        // Force update collider bounds to reflect current position.
        sphereCollider.enabled = false;
        sphereCollider.enabled = true;

        Vector3 rayStart = transform.TransformPoint(sphereCollider.center);
        RaycastHit hit;

        if (Physics.Raycast(rayStart, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Debug.Log("Ground hit at: " + hit.point);
            Debug.DrawLine(rayStart, hit.point, Color.green, 2f);

            float adjustment = rayStart.y - hit.point.y - sphereCollider.center.z;
            Vector3 newCenter = sphereCollider.center + new Vector3(0, 0, adjustment);

            if (newCenter != sphereCollider.center)
            {
                sphereCollider.center = newCenter;
                lastKnownPosition = transform.position;
            }
        }
        else
        {
            Debug.LogWarning("No ground layer was hit by the raycast.");
            Debug.DrawRay(rayStart, Vector3.down * 10f, Color.red, 2f);
        }
    }



    private void CheckForInvalidLayers()
    {
        Vector3 colliderWorldPosition = transform.position + sphereCollider.center;
        isValidBuildArea = !Physics.CheckSphere(colliderWorldPosition, sphereCollider.radius, invalidLayers);
        if (decalProjector != null)
        {
            decalProjector.material.SetInt("_isValidBuildArea", isValidBuildArea ? 1 : 0);
        }
    }

    private void OnDisable()
    {
        if (Application.isPlaying)
        {
            StopAllCoroutines();
        }
    }

}
