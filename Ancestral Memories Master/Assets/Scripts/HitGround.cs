using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitGround : MonoBehaviour
{
    public bool hit = false;
    [SerializeField] private Vector3 fallForce = new Vector3(0, -10, 0);

    private WaterFloat floating;

    private void Awake()
    {
        floating = transform.gameObject.GetComponent<WaterFloat>();
        floating.enabled = false;

        hit = false;
       
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Walkable") || other.CompareTag("Water") || other.CompareTag("Rocks") || other.CompareTag("Cave"))
        {


            Rigidbody rigidBody = gameObject.transform.GetComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            rigidBody.velocity = new Vector3(0, 0, 0);

           ///
           //rigidBody.AddForce(fallForce, ForceMode.Force);

            hit = true;

            if (other.CompareTag("Water")){

                floating.enabled = true;
                StartCoroutine(floating.Float(transform.gameObject));

                return;

            }

            return;


        } else
        {
            hit = false;
            return;
        }
        
    }

}
