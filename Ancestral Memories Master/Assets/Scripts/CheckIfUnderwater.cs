using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfUnderwater : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private AreaManager areaManager;

    private CharacterBehaviours behaviours;
    private PlayerWalk playerWalk;
    private bool isUnderwater = false;
    private bool playerDrowning = false;
    private bool playerHasDrowned = false;

    private void Start()
    {
        behaviours = player.GetComponent<CharacterBehaviours>();
        playerWalk = player.GetComponent<PlayerWalk>();
       // StartCoroutine(CheckUnderwaterCoroutine());
    }

    private IEnumerator CheckUnderwaterCoroutine()
    {
        while (true)
        {
            if (playerWalk.playerInWater)
            {
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out RaycastHit hit, Mathf.Infinity))
                {
                    if (hit.collider.CompareTag("Water"))
                    {
                        isUnderwater = true;

                        if (!playerDrowning && !player.isDead)
                        {
                            StartCoroutine(DrownBuffer());
                        }
                    }
                    else
                    {
                        isUnderwater = false;
                    }
                }
            }
            else
            {
                isUnderwater = false;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator DrownBuffer()
    {
        yield return new WaitForSeconds(4f);

        if (isUnderwater)
        {
            behaviours.StartCoroutine(behaviours.Drown());
        }

        yield break;
    }
}