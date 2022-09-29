using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDisgust : MonoBehaviour
{

    public PlayerDisease playerDisease;

    const string PLAYER_DISGUSTED = "Player_disgusted";

    public int diseaseChance;

    private Faith faith;

    private AnimationManager animator;

    private void OnTriggerEnter(Collider other)
    {

        animator.ChangeAnimationState(PLAYER_DISGUSTED);

        // Chance of getting disease

        diseaseChance = Random.Range(0, 100);
            if(diseaseChance >= faith.currentFaith){

            playerDisease.ContractDisease();

        }         
    }

    private void Update()
    {
        diseaseChance = Random.Range(0, 100);
    }

    private void OnTriggerExit(Collider other)
    {
        diseaseChance = 0;
    }
}
