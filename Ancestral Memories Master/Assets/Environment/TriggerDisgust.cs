using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDisgust : MonoBehaviour
{

    [SerializeField] private Animator animator;

    CharacterClass player;

    const string PLAYER_DISGUSTED = "Player_disgusted";

    public int diseaseChance;

    private void OnTriggerEnter(Collider other)
    {

        player.ChangeAnimationState(PLAYER_DISGUSTED);

        // Chance of getting disease

        diseaseChance = Random.Range(0, 100);
            if(diseaseChance >= player.currentFaith){

            player.ContractDisease();

        }         
    }

    private void Update()
    {
        diseaseChance = Random.Range(0, 100);
    }

    private void OnTriggerExit(Collider other)
    {
       
    }
}
