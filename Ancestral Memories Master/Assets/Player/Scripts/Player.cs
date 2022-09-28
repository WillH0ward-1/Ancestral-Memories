using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Human
{

    void Awake()
    {
        Human player = new();
        Debug.Log(player);

        health = gameObject.AddComponent<Health>();
        faith = gameObject.AddComponent<Faith>();
        hunger = gameObject.AddComponent<Hunger>();
        evolution = gameObject.AddComponent<Evolution>();
        animManager = gameObject.AddComponent<AnimationManager>();

        healthBar = gameObject.AddComponent<HealthBar>();
        faithBar = gameObject.AddComponent<FaithBar>();
        hungerBar = gameObject.AddComponent<HungerBar>();
        evolutionBar = gameObject.AddComponent<EvolutionBar>();

        playerWalk = gameObject.AddComponent<PlayerWalk>();
    }

    void Update()
    {

    }
}