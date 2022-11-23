using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitGround : MonoBehaviour
{
    public float minDecayTime = 5f;

    public float maxDecayTime = 10f;

    void OnTriggerEnter(Collider other)
    {
        // If a GameObject has an "Enemy" tag, remove him.


        Rigidbody rigidBody = gameObject.transform.GetComponent<Rigidbody>();
        GrowControl growControl = gameObject.transform.GetComponent<GrowControl>();
        rigidBody.useGravity = false;
        rigidBody.isKinematic = true;
        float decayTime = Random.Range(minDecayTime, maxDecayTime);

        Destroy(gameObject, decayTime);
        StartCoroutine(growControl.Grow(gameObject, gameObject.transform.localScale, new Vector3(0, 0, 0), decayTime));
        
    }
}
