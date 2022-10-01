using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    public HealthBar playerHealth;
    public HungerBar playerHunger;
    public FaithBar playerFaith;
    public EvolutionBar evolutionBar;

    [SerializeField] float maxHealth;
    [SerializeField] float health;

    public bool IsAlive
    {
        get
        {
            return health > 0;
        }
    }

    public virtual void Start()
    {
        health = maxHealth;
    }

    public virtual void HealDamage(float healthHealed)
    {
        health += healthHealed;

        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }


    public virtual void Death()
    {
        print("Has died");
        // death anim
    }
}
