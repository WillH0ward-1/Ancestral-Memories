using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisease : MonoBehaviour
{

    public CharacterClass player;

    string[] diseaseSeverities = { "mild", "severe", "fatal", "terminal" };
    string diseaseSeverity = "";

    public void ContractDisease()
    {

        int diseaseIndex = diseaseSeverities.Length - 1;

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

                player.isDiseased = true;
            }
        }

        while (player.isDiseased == true)
        {
            int diseaseMultiplier;

            if (diseaseSeverity == "mild")
            {
                diseaseMultiplier = 2;
                player.UpdateStats(0.00005f * diseaseMultiplier, 0, 0, 0);
            }
            if (diseaseSeverity == "severe")
            {
                diseaseMultiplier = 3;
                player.UpdateStats(0.0005f * diseaseMultiplier, 0, 0, 0);
            }
            if (diseaseSeverity == "fatal")
            {
                diseaseMultiplier = 4;
                player.UpdateStats(0.005f * diseaseMultiplier, 0, 0, 0);
            }
            if (diseaseSeverity == "terminal")
            {
                diseaseMultiplier = 5;
                player.UpdateStats(0.05f * diseaseMultiplier, 0, 0, 0);
            }
        }

    }

    public void HealDisease()
    {
        Debug.Log("Player has miraculously recovered from" + diseaseSeverity + "disease!");
        player.isDiseased = false;
    }
}
