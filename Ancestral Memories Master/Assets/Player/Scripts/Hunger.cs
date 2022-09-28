using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunger : Human
{

    public float currentHunger;

    public bool isStarving = false;
    public bool hasStarved = false;

    const string PLAYER_STARVING = "Player_starving";

    const string PLAYER_STARVINGIDLE = "Player_starvingIdle";
    const string PLAYER_STARVINGCRAWL = "Player_starvingCrawl";
    const string PLAYER_STARVINGCRAWLCRITICAL = "Player_starvingCrawlCritical";

    const string PLAYER_STARVE = "Player_starve";


    private void Awake()
    {
        currentHunger = maxVal;
    }

    void Update()
    {
        float hungerMultipler = 0.5f;
        GetHungry(0.1f * hungerMultipler, hungerBar);

        // DEAL STARVE DAMAGE

        if (isStarving && !health.IsDead())
        {
            Debug.Log("Starving!");

            //ChangeAnimationState(PLAYER_STARVING);

            health.TakeDamage(0.1f);

            if (health.IsDead())
            {
                animManager.ChangeAnimationState(PLAYER_STARVE);
                isStarving = false;
                hasStarved = true;
            }
        }
    }

    public void SetHunger(float value)
    {
        currentHunger = value;
    }

    public void GetHungry(float hunger, HungerBar hungerBar)
    {

        currentHunger -= hunger;
        hungerBar.UpdateHunger(currentHunger / maxVal);

        if (currentHunger <= minVal)
        {
            isStarving = true;
        }
        else
        {
            isStarving = false;
        }
    }
}
