using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDisgust : CharacterClass
{

    public PlayerDisease playerDisease;

    [SerializeField] private Animator animator;

    CharacterClass player;

    const string PLAYER_DISGUSTED = "Player_disgusted";

    public int diseaseChance;

    private void OnTriggerEnter(Collider other)
    {

        ChangeAnimationState(PLAYER_DISGUSTED);

        // Chance of getting disease

        diseaseChance = Random.Range(0, 100);

            if(diseaseChance >= currentFaith){

            playerDisease.ContractDisease();

        }         
    }

    override public void Update()
    {
        diseaseChance = Random.Range(0, 100);
    }

    private void OnTriggerExit(Collider other)
    {
        diseaseChance = 0;
    }
}
