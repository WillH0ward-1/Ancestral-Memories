using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evolution : Human
{
    private int maxEvolution = 100;
    private int minEvolution = 0;

    public float currentEvolution;

    // Start is called before the first frame update
    private void Awake()
    {
        currentEvolution = minEvolution;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (currentEvolution <= 25)
        {
        alphaControl.playerIsHuman = false;
        } else
        {
        alphaControl.playerIsHuman = true;
        }
        */

        float evolutionMultiplier = 0.5f;
        Evolve(0.1f * evolutionMultiplier);
    }

    public void Evolve(float evolutionMultiplier)
    {
        currentEvolution += evolutionMultiplier;

        if (currentEvolution >= maxEvolution)
        {
            currentEvolution = minEvolution; // reset evolution ( level up )
            // Possibly get rid of evolution bar after transcending from Monkey, returning only if devolved by god.
        }
    }

    private void Devolve()
    {
        currentEvolution = minEvolution;
    }
}
