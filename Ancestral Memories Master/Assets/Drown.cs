using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drown : MonoBehaviour
{
    public CheckIfUnderwater underwaterCheck;

    private Health health;
    private AnimationManager animator;

    const string PLAYER_DROWN = "Player_drown";

    void Update()
    {
        // DROWN

        if (underwaterCheck.isUnderwater && underwaterCheck.playerDrowning)
        {
            Debug.Log("Drowning!");

            health.TakeDamage(1);

            if (health.playerHasDied == true)
            {
                animator.ChangeAnimationState(PLAYER_DROWN);
                underwaterCheck.playerDrowning = false;
                underwaterCheck.playerHasDrowned = true;
            }
        }
    }
}
