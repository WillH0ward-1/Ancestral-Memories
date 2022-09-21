using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunger : Human
{
    public HungerBar playerHunger;

    public int minHunger = 0;
    public int maxHunger = 100;

    public float currentHunger;

    public bool playerIsStarving = false;
    public bool playerHasStarved = false;

    private Health health;

    const string PLAYER_STARVING = "Player_starving";

    const string PLAYER_STARVINGIDLE = "Player_starvingIdle";
    const string PLAYER_STARVINGCRAWL = "Player_starvingCrawl";
    const string PLAYER_STARVINGCRAWLCRITICAL = "Player_starvingCrawlCritical";

    const string PLAYER_STARVE = "Player_starve";


    private void Awake()
    {
        currentHunger = maxHunger;
    }

    void Update()
    {
        float hungerMultipler = 0.5f;
        GetHungry(0.1f * hungerMultipler);

        // DEAL STARVE DAMAGE

        if (playerIsStarving == true && !health.IsDead())
        {
            Debug.Log("Starving!");

            //ChangeAnimationState(PLAYER_STARVING);

            health.TakeDamage(0.1f);

            if (health.IsDead())
            {
                animManager.ChangeAnimationState(PLAYER_STARVE);
                playerIsStarving = false;
                playerHasStarved = true;
            }
        }
    }

    public void GetHungry(float hunger)
    {

        currentHunger -= hunger;
        playerHunger.UpdateHunger(currentHunger / maxHunger);

        if (currentHunger <= minHunger)
        {
            playerIsStarving = true;
        }
        else
        {
            playerIsStarving = false;
        }
    }
}
