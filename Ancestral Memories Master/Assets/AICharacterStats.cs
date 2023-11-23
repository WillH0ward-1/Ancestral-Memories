using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AICharacterStats : MonoBehaviour
{
    // Define the base stats
    public float minStat = 0f;
    public float maxStat = 1f;

    public float health = 1f;
    public float hunger = 1f;
    public float faith = 1f;
    public float psych = 1f;
    public float evolution = 1f;
    public int age = 0;

    public bool isDiseased = false;
    public bool isBlessed = false;
    public bool isStarving = false;

    public bool isDead = false;

    public float healthFactor = 0.00000001f;
    public float faithFactor = 0.00000001f;
    public float hungerFactor = 0.00000001f;
    public float EvolutionFraction => (evolution - minStat) / (maxStat - minStat);

    public event Action<float, float, float> OnHealthChanged;
    public event Action<float, float, float> OnHungerChanged;
    public event Action<float, float, float> OnFaithChanged;
    public event Action<float, float, float> OnPsychChanged;
    public event Action<float, float, float> OnDiseaseDamageApplied;
    public event Action<float, float, float> OnEvolutionChanged;

    public bool IsBlessed => isBlessed;
    public bool IsFaithless => IsFaithless;
    public bool isFaithless;

    public float minBlessedDuration = 30f;
    public float maxBlessedDuration = 60f;

    public float HealthFraction => (health - minStat) / (maxStat - minStat);
    public float HungerFraction => (hunger - minStat) / (maxStat - minStat);
    public float FaithFraction => (faith - minStat) / (maxStat - minStat);
    public float PsychFraction => (psych - minStat) / (maxStat - minStat);

    public bool useFaith = true;

    public Animator animator;
    public float animationCrossFade = 0.2f;

    private string currentState;

    private DamageEffects damageEffects;

    public TimeCycleManager time;

    public virtual void OnAwake()
    {
        age = 0;

        health = maxStat;
        hunger = maxStat;
        faith = minStat;
        psych = minStat;
        isDiseased = false;

        OnHealthChanged?.Invoke(HealthFraction, minStat, maxStat);
        OnHungerChanged?.Invoke(HungerFraction, minStat, maxStat);
        OnFaithChanged?.Invoke(FaithFraction, minStat, maxStat);
        OnPsychChanged?.Invoke(PsychFraction, minStat, maxStat);

        animator = transform.GetComponentInChildren<Animator>();
        damageEffects = transform.GetComponentInChildren<DamageEffects>();

        isDead = false;

        calculatedLifespan = CalculateLifespan();
    }

    private bool subscribed = false;

    public void SubscribeToBirthday()
    {
        time.OnNewYear += IncrementAge;
        subscribed = true;
    }

    private void OnDisable()
    {
        if (time != null)
        {
            time.OnNewYear -= IncrementAge;
        }
    }

    void Start()
    {
        StartCoroutine(StartLifespanCheckWhenReady());
    }

    private IEnumerator StartLifespanCheckWhenReady()
    {
        // Wait until 'time' is not null
        yield return new WaitUntil(() => subscribed);

        StartCoroutine(LifespanCheckCoroutine());
    }

    public float calculatedLifespan;
    public int maxLifeSpan = 120; // Maximum lifespan
    public int minLifeSpan = 30;  // Minimum lifespan

    private IEnumerator LifespanCheckCoroutine()
    {
        while (!isDead)
        {
            float inGameHourDuration = 3600f / time.timeMultiplier; // 1 in-game hour
            yield return new WaitForSeconds(6f * inGameHourDuration); // Wait for 6 in-game hours

            calculatedLifespan = CalculateLifespan();

            Debug.Log($"Age: {age}, Calculated Lifespan: {calculatedLifespan}");

            if (age >= calculatedLifespan)
            {
                Debug.Log("Character has died of old age.");
                isDead = true;
            }
        }
    }
    private float CalculateLifespan()
    {
        // Sum of all stats, each ranging from 0 to 1
        float totalStatSum = health + hunger + faith + psych;

        // Normalized lifespan calculation
        float normalizedLifespan = totalStatSum / 4;
        float calculatedLifespan = normalizedLifespan * maxLifeSpan;

        // Clamping the lifespan between minLifeSpan and maxLifeSpan
        calculatedLifespan = Mathf.Clamp(calculatedLifespan, minLifeSpan, maxLifeSpan);

        Debug.Log($"Total Stat Sum: {totalStatSum}, Normalized Lifespan: {normalizedLifespan}, Calculated Lifespan: {calculatedLifespan}");

        return calculatedLifespan;
    }


    private void IncrementAge()
    {
        age++;
        Debug.Log($"Age incremented to: {age}");
    }

    public virtual void Update()
    {
        // Deplete hunger over time
        float faithInfluence = 1.0f - FaithFraction; // a value between 0.0 and 1.0, higher when faith is lower
        faithInfluence *= faithInfluence; // square it to make it exponential

        if (!isDead)
        {
            if (hunger > minStat)
            {
                isStarving = false;

                if (useFaith)
                {
                    hunger -= Time.deltaTime * hungerFactor * (1.0f + faithFactor * faithInfluence);
                }
                else
                {
                    hunger -= Time.deltaTime * hungerFactor;
                }

                hunger = Mathf.Clamp(hunger, minStat, maxStat);
            }
            else
            {
                isStarving = true;

                if (useFaith)
                {
                    health -= Time.deltaTime * healthFactor * (1.0f + faithFactor * faithInfluence);
                }
                else
                {
                    health -= Time.deltaTime * healthFactor;
                }

                health = Mathf.Clamp(health, minStat, maxStat);
            }

            if (useFaith && faith > minStat)
            {
                faith -= Time.deltaTime * faithFactor;
                faith = Mathf.Clamp(faith, minStat, maxStat);
            }

            if (health <= minStat)
            {
                isDead = true;
            }
        }

        OnHungerChanged?.Invoke(HungerFraction, minStat, maxStat);
        OnHealthChanged?.Invoke(HealthFraction, minStat, maxStat);
        OnFaithChanged?.Invoke(FaithFraction, minStat, maxStat);
        OnPsychChanged?.Invoke(PsychFraction, minStat, maxStat);

        if (transform.CompareTag("Human"))
        {
            UpdateEvolution();
        }
    }

    public float evolutionSpeedFactor = 0.1f;  // New factor to determine the speed of evolution convergence

    private void UpdateEvolution()
    {
        if (!isDead)
        {
            float previousEvolution = evolution;  // Store the current evolution value

            float targetEvolution = FaithFraction;
            float interpolationFactor = Mathf.Lerp(0.01f, evolutionSpeedFactor, FaithFraction);
            evolution = Mathf.Lerp(evolution, targetEvolution, interpolationFactor * Time.deltaTime);

            if (evolution != previousEvolution)  // Check if evolution has effectively changed
            {
                OnEvolutionChanged?.Invoke(EvolutionFraction, minStat, maxStat);
            }
        }
        
    }

    public void Heal(float healFactor)
    {
        if (!isDead)
        {
            health += healFactor;
            health = Mathf.Clamp(health, minStat, maxStat);
            OnHealthChanged?.Invoke(HealthFraction, minStat, maxStat);
        }
    }

    public void SetHealth(float value)
    {
        health = value;
        health = Mathf.Clamp(health, minStat, maxStat);
        OnHealthChanged?.Invoke(HealthFraction, minStat, maxStat);
    }

    public void SetFaith(float value)
    {
        faith = value;
        faith = Mathf.Clamp(faith, minStat, maxStat);
        OnHealthChanged?.Invoke(FaithFraction, minStat, maxStat);
    }

    public void SetHunger(float value)
    {
        hunger = value;
        hunger = Mathf.Clamp(hunger, minStat, maxStat);
        OnHealthChanged?.Invoke(HungerFraction, minStat, maxStat);
    }

    public void ReinitializeAll()
    {
        isDead = false;

        MaxAllStats();
        ResetAge();
    }

    public void MaxAllStats()
    {
        SetFaith(maxStat);
        SetHealth(maxStat);
        SetHunger(maxStat);
    }

    public void ResetAge()
    {
        age = 0;
    }

    public void TakeDamage(float damageTaken)
    {
        if (!isDead)
        {
            health -= damageTaken;
            health = Mathf.Clamp(health, minStat, maxStat);
            OnHealthChanged?.Invoke(HealthFraction, minStat, maxStat);
            damageEffects.FlashRed();

            if (health <= minStat && !isDead)
            {
                isDead = true;
            }
        }
    }

    public void FaithModify(float faithModifer)
    {
        if (useFaith)
        {
            faith += faithModifer;
            faith = Mathf.Clamp(faith, minStat, maxStat);
            OnFaithChanged?.Invoke(FaithFraction, minStat, maxStat);

            if (faith >= maxStat)
            {
                faith = maxStat;
                Debug.Log("Player has max faith!");

                if (!isBlessed)
                {
                    StartCoroutine(TransendenceTimer());
                }
            }
            else if (faith <= minStat)
            {
                faith = minStat;
                isFaithless = true;
                Debug.Log("Player is faithless!");
            }
            else
            {
                isFaithless = false;
            }

            if (faith <= maxStat / 2 && isBlessed)
            {
                isBlessed = false;
                Debug.Log("Player is no longer blessed.");
            }
        }
    }

    public IEnumerator TransendenceTimer()
    {
        float remainingTime = UnityEngine.Random.Range(minBlessedDuration, maxBlessedDuration);
        isBlessed = true;
        Debug.Log("Player is blessed.");

        while (remainingTime > 0 && isBlessed)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        if (isBlessed)
        {
            isBlessed = false;
        }

        yield break;
    }

    private Disease Disease { get; set; }

    public void ContractDisease()
    {
        if (!isDead)
        {
            if (!Disease.IsContracted)
            {
                int diseaseIndex = UnityEngine.Random.Range(0, Disease.Severity.Length);
                Disease.Contract();
            }
        }
    }

    public void DiseaseDamage()
    {
        if (Disease.IsContracted)
        {
            // Add multiplier based on faith. The lower the faith, the more damage.
            float faithMultiplier = 1 - FaithFraction;

            float damage = (0.00005f * Disease.GetDamageMultiplier()) * faithMultiplier;
            TakeDamage(damage);
            OnDiseaseDamageApplied?.Invoke(damage, minStat, maxStat);
        }
    }


    public void PsychModifier(float psychFactor)
    {
        psych = psychFactor;
        psych = Mathf.Clamp(psych, 0f, 1f);
        OnPsychChanged?.Invoke(psych, minStat, maxStat);
    }

    public void Hunger(float hungerFactor)
    {
        hunger -= hungerFactor;
        hunger = Mathf.Clamp(hunger, 0f, 1f);
        OnHungerChanged?.Invoke(HungerFraction, minStat, maxStat);
    }

    public void HealHunger(float hungerFactor)
    {
        hunger += hungerFactor;
        hunger = Mathf.Clamp(hunger, 0f, 1f);
        OnHungerChanged?.Invoke(HungerFraction, minStat, maxStat);
    }

    public void ChangeAnimationState(string newState)
    {
        float crossFadeLength = animationCrossFade;

        if (currentState == newState)
        {
            return;
        }

        animator.CrossFadeInFixedTime(newState, crossFadeLength);

        currentState = newState;
    }

    public void AdjustAnimationSpeed(float newSpeed)
    {
        animator.speed = newSpeed;
    }

}
