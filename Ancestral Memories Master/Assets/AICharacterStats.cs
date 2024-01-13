using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AICharacterStats : MonoBehaviour
{
    // Define the base stats

    public string npcName;
    public int age = 0;

    public bool isDying = false;
    public bool isDead = false;

    public float minStat = 0f;
    public float maxStat = 1f;

    public bool isDiseased = false;
    public bool isBlessed = false;
    public bool isStarving = false;
    public bool isTerrified = false;
    public bool isKnockedOut = false;

    public float health = 1f;
    public float hunger = 1f;
    public float faith = 1f;
    public float psych = 1f;
    public float evolution = 1f;

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

    public enum RelationshipType { Friend, Stranger, Enemy }

    public enum NpcType { Human, Animal }

    public enum NpcGender { Male, Animal }

    public enum HumanEvolutionState { Neanderthal, MidSapien, Sapien }

    private HumanEvolutionState evolutionState;

    [Serializable]
    public struct Relationship
    {
        public GameObject npc;
        public RelationshipType type;
    }

    public List<Relationship> relationships = new List<Relationship>();

    public MapObjGen mapObjGen;

    private HumanAI humanAI;


    public virtual void OnAwake()
    {
        age = 0;

        health = maxStat;
        hunger = maxStat;
        faith = minStat;
        psych = minStat;
        evolution = minStat;
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

    public IEnumerator InitAllRelationships(List<GameObject> npcList)
    {
        foreach (GameObject g in npcList)
        {
            AICharacterStats stats = g.GetComponentInChildren<AICharacterStats>();

            // Clear existing relationships
            stats.relationships.Clear();

            // Add relationships
            foreach (GameObject other in npcList)
            {
                if (other != g) // Don't add self
                {
                    Relationship relationship = new Relationship
                    {
                        npc = other,
                        type = RelationshipType.Stranger // default to Stranger
                    };
                    stats.relationships.Add(relationship);
                }
            }
        }

        yield break;
    }

    public void GiveName()
    {
        if (NameDictionary.Instance != null)
        {
            npcName = NameDictionary.Instance.GetRandomMaleName();

            // Check the tag and prepend the string accordingly
            string typeIdentifier = "";
            if (gameObject.CompareTag("Human"))
            {
                typeIdentifier = "Citizen: ";
            }
            else if (gameObject.CompareTag("Animal"))
            {
                typeIdentifier = "Animal: "; // Or any other identifier you want to use for animals
            }

            transform.gameObject.name = typeIdentifier + npcName;
        }
        else
        {
            Debug.LogError("NameDictionary instance is not found.");
        }
    }


    public void UpdateRelationshipStatus(GameObject npc, RelationshipType newRelation)
    {
        // Update this NPC's relationship status towards the other NPC
        for (int i = 0; i < relationships.Count; i++)
        {
            if (relationships[i].npc == npc)
            {
                // Check if the relationship is already in the desired state
                if (relationships[i].type == newRelation)
                {
                    // Relationship is already in the desired state, no need to update
                    return;
                }

                relationships[i] = new Relationship
                {
                    npc = npc,
                    type = newRelation
                };
                break;
            }
        }

        // Update the other NPC's relationship status towards this NPC
        AICharacterStats otherStats = npc.GetComponentInChildren<AICharacterStats>();
        if (otherStats != null)
        {
            for (int i = 0; i < otherStats.relationships.Count; i++)
            {
                if (otherStats.relationships[i].npc == gameObject)
                {
                    // Check if the relationship is already in the desired state
                    if (otherStats.relationships[i].type == newRelation)
                    {
                        // Relationship is already in the desired state, no need to update
                        return;
                    }

                    otherStats.relationships[i] = new Relationship
                    {
                        npc = gameObject,
                        type = newRelation
                    };
                    break;
                }
            }
        }
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
        if (transform.CompareTag("Human"))
        {
            humanAI = GetComponentInChildren<HumanAI>();
        }

        StartCoroutine(StartLifespanCheckWhenReady());
        GiveName();
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

//        Debug.Log($"Total Stat Sum: {totalStatSum}, Normalized Lifespan: {normalizedLifespan}, Calculated Lifespan: {calculatedLifespan}");

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

            if (isDead && !isDying && humanAI != null)
            {
                humanAI.ChangeState(HumanAI.AIState.Die);
            }

        } else if (transform.CompareTag("Player"))
        {
            UpdateEvolution();
        }
    }

    public float evolutionSpeedFactor = 0.1f;  // New factor to determine the speed of evolution convergence

    private void UpdateEvolution()
    {
        if (!isDead)
        {
            float previousEvolution = evolution;
            float targetEvolution = FaithFraction; // Ensure this is a value between 0 and 1
            float interpolationFactor = Mathf.Lerp(0.01f, evolutionSpeedFactor, FaithFraction);
            evolution = Mathf.Lerp(evolution, targetEvolution, interpolationFactor * Time.deltaTime);

            // Update the evolution state based on the current evolution value
            evolutionState = EvolutionState;

            // Check for significant change before invoking the event
            if (Mathf.Abs(evolution - previousEvolution) > Mathf.Epsilon)
            {
                OnEvolutionChanged?.Invoke(EvolutionFraction, minStat, maxStat);
            }
        }
    }

    public HumanEvolutionState EvolutionState
    {
        get
        {
            if (evolution < 0.33f) return HumanEvolutionState.Neanderthal;
            else if (evolution < 0.66f) return HumanEvolutionState.MidSapien;
            else return HumanEvolutionState.Sapien;
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
        OnFaithChanged?.Invoke(FaithFraction, minStat, maxStat);
    }

    public void SetHunger(float value)
    {
        hunger = value;
        hunger = Mathf.Clamp(hunger, minStat, maxStat);
        OnHungerChanged?.Invoke(HungerFraction, minStat, maxStat);
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
//                Debug.Log("Player has max faith!");

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
