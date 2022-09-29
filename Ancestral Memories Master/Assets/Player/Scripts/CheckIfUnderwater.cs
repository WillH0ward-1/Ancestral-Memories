using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CheckIfUnderwater : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField] private Health health;

    [SerializeField] private bool isUnderwater = false;

    [SerializeField] private bool playerDrowning = false;

    [SerializeField] private bool playerHasDrowned = false;

    private Health player;

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

                if (playerDrowning == false && !health.IsDead())
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
        if (playerDrowning == true)
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
        playerDrowning = false;
        playerHasDrowned = true;

        return;
    }

    IEnumerator StartDrowning()
    {
        yield return new WaitForSeconds(4f); // wait for this many seconds before taking damage from drowning. // make this value the 'O2' level or 'lung capacity'

        if (isUnderwater == true)
        {
            playerDrowning = true;

        } else if (isUnderwater == false)
        {
            yield break;
        }
    }
}

       
    

