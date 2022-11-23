using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDisgust : MonoBehaviour
{

    public Player player;

    const string PLAYER_DISGUSTED = "Player_disgusted";

    public int diseaseChance;

    private void OnTriggerEnter(Collider other)
    {

        // Chance of getting disease

        diseaseChance = Random.Range(0, 100);

        if(diseaseChance >= player.faith)
        {

            player.ContractDisease();

        }         
    }

    private void OnTriggerExit(Collider other)
    {
        diseaseChance = 0;
    }
}
