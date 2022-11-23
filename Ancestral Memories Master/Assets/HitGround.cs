using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitGround : MonoBehaviour
{
    public float minDecayTime = 5f;

    public float maxDecayTime = 10f;

    public bool hit = false;

    private void Awake()
    {
        hit = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // If a GameObject has an "Enemy" tag, remove him.

        if (other.CompareTag("Walkable") || other.CompareTag("Water"))
        {

            Rigidbody rigidBody = gameObject.transform.GetComponent<Rigidbody>();
            GrowControl growControl = gameObject.transform.GetComponent<GrowControl>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            float decayTime = Random.Range(minDecayTime, maxDecayTime);

            hit = true;

            Destroy(gameObject, decayTime);
            
        } else
        {
            return;
        }
        
    }
}
