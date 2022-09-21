using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDisgust : Human
{

    public PlayerDisease playerDisease;

    const string PLAYER_DISGUSTED = "Player_disgusted";

    public int diseaseChance;

    private AnimationManager animator;

    private Human player;

    [SerializeField] private Faith faith;

    private void OnTriggerEnter(Collider other)
    {

        animator.ChangeAnimationState(PLAYER_DISGUSTED);

        // Chance of getting disease

        diseaseChance = Random.Range(0, 100);
            if(diseaseChance >= faith.GetFaith())
        {

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
