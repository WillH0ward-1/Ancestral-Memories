using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{

    [SerializeField] protected int minVal;
    [SerializeField] protected int maxVal;

    [SerializeField] protected string characterName;

    [SerializeField] protected Health health;
    [SerializeField] protected Faith faith;
    [SerializeField] protected Hunger hunger;
    [SerializeField] protected Evolution evolution;

    [SerializeField] protected HungerBar hungerBar;
    [SerializeField] protected FaithBar faithBar;
    [SerializeField] protected HealthBar healthBar;
    [SerializeField] protected EvolutionBar evolutionBar;

    [SerializeField] protected AnimationManager animManager;

    [SerializeField] protected PlayerWalk playerWalk;

    [SerializeField] protected bool hasDied = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
