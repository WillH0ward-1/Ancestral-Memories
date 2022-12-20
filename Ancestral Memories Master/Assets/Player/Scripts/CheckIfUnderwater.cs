using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfUnderwater : MonoBehaviour
{

    [SerializeField] private Player player;

    public bool isUnderwater = false;

    public bool playerDrowning = false;

    public bool playerHasDrowned = false;

    private CharacterBehaviours behaviours;

    [SerializeField] private AreaManager areaManager;

    PlayerWalk playerWalk;
    RaycastHit hit;

    private void Start()
    {
        behaviours = player.GetComponent<CharacterBehaviours>();
        playerWalk = player.GetComponent<PlayerWalk>();
    }
    void Update()
    {
    
        if (playerWalk.playerInWater)
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity))
            {
                //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.green);
                //Debug.Log("Did Hit");

                if (hit.collider.CompareTag("Water"))
                {
                    isUnderwater = true;

                    if (playerDrowning == false && player.hasDied == false)
                    {
                        StartCoroutine(DrownBuffer());
                    }
                }

                else
                {
                    //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * 1000, Color.red);
                    //Debug.Log("Did not Hit");
                    isUnderwater = false;

                }
            }

            IEnumerator DrownBuffer()
            {
                yield return new WaitForSeconds(4f); // wait for this many seconds before taking damage from drowning. // make this value the 'O2' level or 'lung capacity'

                if (isUnderwater == true)
                {
                    behaviours.StartCoroutine(behaviours.Drown());

                }
                else if (isUnderwater == false)
                {
                    StopCoroutine(behaviours.Drown());
                }

                yield break;
            }
        }

        return;
    }
}

       
    

