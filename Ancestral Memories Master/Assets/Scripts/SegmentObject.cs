using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentObject : MonoBehaviour
{
    public GameObject parentObject;
    private Vector3 lastPosition;
    private float positionChangeThreshold = 0.001f; // Adjust this threshold as needed
    private float timeWithoutChange = 0f;
    private float maxTimeWithoutChange = 1f; // Adjust this value as needed

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (parentObject != null)
        {
            Vector3 meshCenter = GetComponent<Renderer>().bounds.center;
            parentObject.transform.position = meshCenter;

            // Check if the position has changed
            if (Vector3.Distance(transform.position, lastPosition) > positionChangeThreshold)
            {
                // Position has changed, reset the timer
                timeWithoutChange = 0f;
            }
            else
            {
                // Position hasn't changed, increment the timer
                timeWithoutChange += Time.fixedDeltaTime;

                // If it's been too long without a change, stop FixedUpdate
                if (timeWithoutChange >= maxTimeWithoutChange)
                {
                    enabled = false; // Disable FixedUpdate
                }
            }

            // Update the last position
            lastPosition = transform.position;
        }
    }
}
