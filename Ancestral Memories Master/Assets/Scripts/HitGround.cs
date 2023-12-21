using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitGround : MonoBehaviour
{
    public bool hit = false;

    [SerializeField] private WaterFloat floating;
    private Rigidbody rigidBody;
    private LayerMask hitGroundLayer;

    // Reference to the TreeFruitManager
    private TreeFruitManager fruitManager;

    private void Awake()
    {
        // Get the TreeFruitManager reference
        fruitManager = FindObjectOfType<TreeFruitManager>();

        floating = GetComponent<WaterFloat>();
        floating.enabled = false;

        hitGroundLayer = LayerMask.GetMask("Ground", "Water", "Rocks", "Cave");

        hit = false;
        rigidBody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((hitGroundLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            hit = true;
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            rigidBody.velocity = Vector3.zero;
            //StartCoroutine(fruitManager.Decay(gameObject));
            if (other.CompareTag("Water"))
            {
                CheckWaterDepth(other);
            }
        }
    }

    private void CheckWaterDepth(Collider waterCollider)
    {
        float floatDepthThreshold = 1f;

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, Mathf.Infinity, hitGroundLayer))
        {
            float distanceToGround = hit.distance;

            if (distanceToGround >= floatDepthThreshold)
            {
                floating.enabled = true;
                StartCoroutine(floating.Float(transform.gameObject));
            }
            else
            {
                floating.enabled = false;
            }
        }
    }
}
