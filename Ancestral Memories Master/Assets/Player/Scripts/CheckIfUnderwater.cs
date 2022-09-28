using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CheckIfUnderwater : Human
{

    [SerializeField] private bool isUnderwater = false;

    [SerializeField] private bool isDrowning = false;

    [SerializeField] private bool hasDrowned = false;

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.green);
            //Debug.Log("Did Hit");

            if (hit.collider.CompareTag("Water"))
            {
                isUnderwater = true;

                if (isDrowning == false && !health.IsDead())
                {
                    StartCoroutine(StartDrowning());
                }
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * 1000, Color.red);
            //Debug.Log("Did not Hit");
            isUnderwater = false;

        }
    }

    public bool IsUnderWater()
    {
        if (isUnderwater == true)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public bool IsDrowning()
    {
        if (isDrowning == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void HasDrowned()
    {
        isDrowning = false;
        hasDrowned = true;

        return;
    }

    IEnumerator StartDrowning()
    {
        yield return new WaitForSeconds(4f); // wait for this many seconds before taking damage from drowning. // make this value the 'O2' level or 'lung capacity'

        if (isUnderwater == true)
        {
            isDrowning = true;

        } else if (isUnderwater == false)
        {
            yield break;
        }
    }
}

       
    

