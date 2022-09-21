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

        if (underwaterCheck.IsUnderWater() && underwaterCheck.IsDrowning())
        {
            Debug.Log("Drowning!");

            health.TakeDamage(1);

            if (health.IsDead())
            {
                animator.ChangeAnimationState(PLAYER_DROWN);
                underwaterCheck.HasDrowned();
            }
        }
    }
}
