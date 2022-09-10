using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisease : MonoBehaviour
{

    public CharacterClass player;

    public void ContractDisease()
    {
        string[] diseaseSeverities = { "mild", "severe", "fatal", "terminal" };
        int diseaseIndex = diseaseSeverities.Length - 1;
        string diseaseSeverity = "";

        CaculateChanceOfDisease();

        void CaculateChanceOfDisease()
        {
            // Factor in current health, faith, age etc...
            NotifyOfDisease();
        }

        void NotifyOfDisease()
        {

            for (int i = 0; i < diseaseSeverities.Length; i++)
            {
                diseaseSeverity = diseaseSeverities[diseaseIndex];
                Debug.Log("Player has been infected with a " + diseaseSeverity + " disease!"); // Make so that there are stages. Mild, Severe, Fatal, Terminal

                player.playerIsDiseased = true;
            }
        }

        while (player.playerIsDiseased == true)
        {
            int diseaseMultiplier;

            if (diseaseSeverity == "mild")
            {
                diseaseMultiplier = 2;
                player.TakeDamage(0.00005f * diseaseMultiplier);
            }
            if (diseaseSeverity == "severe")
            {
                diseaseMultiplier = 3;
                player.TakeDamage(0.0005f * diseaseMultiplier);
            }
            if (diseaseSeverity == "fatal")
            {
                diseaseMultiplier = 4;
                player.TakeDamage(0.005f * diseaseMultiplier);
            }
            if (diseaseSeverity == "terminal")
            {
                diseaseMultiplier = 5;
                player.TakeDamage(0.05f * diseaseMultiplier);
            }
        }

    }

    public void HealDisease()
    {
        Debug.Log("Player has miraculously recovered from disease!");
        player.playerIsDiseased = false;
    }
}
