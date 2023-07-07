using System;
using UnityEngine;
using UnityEngine.UI;

public class AICharacterStats : MonoBehaviour
{
    // Define the base stats
    public float minStat = 0;
    public float maxStat = 1;

    [SerializeField] private float health = 1f;
    [SerializeField] private float hunger = 1f;
    [SerializeField] private float faith = 1f;
    [SerializeField] private float psych = 1f;
    [SerializeField] private bool isDiseased;

    public float healthFactor = 0.1f;
    public float faithFactor = -0.01f;
    public float hungerFactor = 0.1f;

    public event Action<float> OnHealthChanged;
    public event Action<float> OnHungerChanged;
    public event Action<float> OnFaithChanged;
    public event Action<float> OnDiseaseDamageApplied;

    public float HealthFraction => (health - minStat) / (maxStat - minStat);
    public float HungerFraction => (hunger - minStat) / (maxStat - minStat);
    public float FaithFraction => (faith - minStat) / (maxStat - minStat);

    private void Awake()
    {
        health = maxStat;
        hunger = maxStat;
        faith = minStat;
        psych = minStat;
        isDiseased = false;

        // Invoke the initial values of the stats
        OnHealthChanged?.Invoke(HealthFraction);
        OnHungerChanged?.Invoke(HungerFraction);
        OnFaithChanged?.Invoke(FaithFraction);
    }


    private void Update()
    {
        // Deplete hunger over time
        if (hunger > minStat)
        {
            hunger -= Time.deltaTime * hungerFactor;
            hunger = Mathf.Clamp(hunger, minStat, maxStat);
        }
        else
        {
            // If hunger is fully depleted, deplete health
            health -= Time.deltaTime * healthFactor;
            health = Mathf.Clamp(health, minStat, maxStat);
        }

        OnHungerChanged?.Invoke(HungerFraction);
        OnHealthChanged?.Invoke(HealthFraction);
        OnFaithChanged?.Invoke(FaithFraction);
    }

    public void Heal(float healFactor)
    {
        health += healFactor;
        health = Mathf.Clamp(health, 0f, 1f);
        OnHealthChanged?.Invoke(HealthFraction);
    }

    public void SetHealth(int value)
    {
        health = value;
        health = Mathf.Clamp(health, 0f, 1f);
        OnHealthChanged?.Invoke(HealthFraction);
    }

    public void TakeDamage(float damageTaken)
    {
        health -= damageTaken;
        health = Mathf.Clamp(health, 0f, 1f);
        OnHealthChanged?.Invoke(HealthFraction);
    }

    public void FaithModify(float faithModifer)
    {
        faith += faithModifer;
        faith = Mathf.Clamp(faith, 0f, 1f);
        OnFaithChanged?.Invoke(FaithFraction);
    }

    private Disease Disease { get; set; }

    public void ContractDisease()
    {
        if (!Disease.IsContracted)
        {
            int diseaseIndex = UnityEngine.Random.Range(0, Disease.Severity.Length);
            Disease.Contract();
        }
    }

    public void DiseaseDamage()
    {
        if (Disease.IsContracted)
        {
            float damage = 0.00005f * Disease.GetDamageMultiplier();
            TakeDamage(damage);
            OnDiseaseDamageApplied?.Invoke(damage);
        }
    }

    public void PsychModifier(float psychFactor)
    {
        // Implement PsychModifier method as needed
    }

    public void Hunger(float hungerFactor)
    {
        hunger -= hungerFactor;
        hunger = Mathf.Clamp(hunger, 0f, 1f);
        OnHungerChanged?.Invoke(HungerFraction);
    }

    public void HealHunger(float hungerFactor)
    {
        hunger += hungerFactor;
        hunger = Mathf.Clamp(hunger, 0f, 1f);
        OnHungerChanged?.Invoke(HungerFraction);
    }
}
