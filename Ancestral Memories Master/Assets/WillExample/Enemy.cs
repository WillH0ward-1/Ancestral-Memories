using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ITakeDamage
{
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

    public virtual void TakeDamage(float damageTaken)
    {
        health -= damageTaken;

        if (health <= 0)
        {
            health = 0;
            Death();
        }
    }

    public virtual void Death()
    {
        print("Has died");
        // death anim
    }

}
