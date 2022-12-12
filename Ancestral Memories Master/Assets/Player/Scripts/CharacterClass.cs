using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class CharacterClass : MonoBehaviour, IStats
{

    [System.NonSerialized] public Animator activeAnimator;

    [System.NonSerialized] public Animator inactiveAnimator;

    private string currentState;
    private string state;

    // ANIMATION STATES ==================================================

    // These are string references for animations, triggered by various events. (See the Animator tab)

    // Idle

    const string PLAYER_IDLE = "Player_idle";
    const string PLAYER_STARVINGIDLE = "Player_starvingIdle";
    const string PLAYER_STARVINGCRAWL = "Player_starvingCrawl";
    const string PLAYER_STARVINGCRAWLCRITICAL = "Player_starvingCrawlCritical";

    // Ailments

    const string PLAYER_DISEASED = "Player_diseased";


    // Death + Revival

    const string PLAYER_DEATHOLDAGE = "Player_deathOldAge";

    const string PLAYER_STARVE = "Player_starve";
    const string PLAYER_DROWN = "Player_drown";
    const string PLAYER_POISONED = "Player_poisoned";
    const string PLAYER_THUNDERSTRUCK = "Player_thunderStruck";
    const string PLAYER_DEHYDRATIONDEATH = "Player_deathThirst";
    const string PLAYER_REVIVING = "Player_revived";

    // PLAYER STATS =======================================================

    float evolutionThreshold = 25;

    private string name = "";
    private int age = 24;

    private int ageToDie = 0;

    public int maxStat = 100;
    public int minStat = 0;

    public HealthBar healthBar;
    public HungerBar hungerBar;
    public FaithBar faithBar;
    public EvolutionBar evolutionBar;

    public float health;
    public float hunger;
    public float faith;
    public float evolution;

    public bool starving = false;
    public bool hasStarved = false;

    public bool isDiseased = false;

    public bool isFaithless = false;
    public bool killedByGod = false;

    public bool hasDied = false;

    public CheckIfUnderwater underwaterCheck;

    public PlayerWalk playerWalk;

    public List<GameObject> activeAnimators = new List<GameObject>();
    public List<GameObject> inactiveAnimators = new List<GameObject>();

    [SerializeField] public float animationCrossFade = 0.1f;

    public bool respawn;

    //public GodRayControl godRay;

    public ControlAlpha alphaControl;

    public Shake earthQuake;

    // FUNCTIONS ============================================================

    public virtual void Awake()
    {
        name = "Jon";

        InitAnimators();

        health = maxStat;
        hunger = maxStat;
        faith = minStat;
        evolution = minStat;        

        isDiseased = false;
    }

    public virtual void InitAnimators()
    {
        var humanState = alphaControl.humanObject;
        var apeState = alphaControl.monkeyObject;
        var skeletonState = alphaControl.skeletonObject;

        if (alphaControl.playerIsSkeleton)
        {
            inactiveAnimators.Remove(skeletonState);
            activeAnimators.Add(skeletonState);

            inactiveAnimators.Add(humanState);
            activeAnimators.Remove(humanState);

            inactiveAnimators.Add(apeState);
            activeAnimators.Remove(apeState);

            return;
        }

        else if (alphaControl.playerIsHuman == false && !alphaControl.playerIsSkeleton)
        {
            inactiveAnimators.Remove(apeState);
            activeAnimators.Add(apeState);

            inactiveAnimators.Add(humanState);
            activeAnimators.Remove(humanState);

            inactiveAnimators.Add(skeletonState);
            activeAnimators.Remove(skeletonState);

            return;

        } else if (alphaControl.playerIsHuman == true && !alphaControl.playerIsSkeleton)
        { 

            inactiveAnimators.Remove(humanState);
            activeAnimators.Add(humanState);

            inactiveAnimators.Add(apeState);
            activeAnimators.Remove(apeState);

            inactiveAnimators.Add(skeletonState);
            activeAnimators.Remove(skeletonState);

            return;
        } 
    }

    public virtual void AssignAnimators()
    {
        foreach (GameObject g in activeAnimators)
        {
            foreach (Animator a in g.GetComponentsInChildren<Animator>())
            {
                activeAnimator = a;
            }
        }
    }

    public virtual void AssignInactiveAnimators()
    {
        foreach (GameObject g in inactiveAnimators)
        {
            foreach (Animator a in g.GetComponentsInChildren<Animator>())
            {
                inactiveAnimator = a;
            }
        }
    }

    public void ChangeAnimationState(string newState)
    {
        Assign();

        float crossFadeLength = animationCrossFade;

        if (currentState == newState)
        {
            return;
        }

        activeAnimator.CrossFadeInFixedTime(newState, crossFadeLength);
        inactiveAnimator.CrossFadeInFixedTime(newState, crossFadeLength);

        currentState = newState;
    }

    public virtual void Assign()
    {
        AssignAnimators();
        AssignInactiveAnimators();
    }

    public void AdjustAnimationSpeed(float newSpeed)
    {
        activeAnimator.speed = newSpeed;
        inactiveAnimator.speed = newSpeed;
    }

    public virtual void Update()
    {
        Hunger(0.1f);
        DepleteFaith(0.1f);
    }


    public virtual void SetHealth(int value)
    {
        health = value;

        if (health >= maxStat)
        {
            health = maxStat;
        }
    }

    private void Kill()
    {
        health = minStat;
        hasDied = true;

        ChangeAnimationState(PLAYER_STARVE);
        StartCoroutine(CheckForRevive());
    }

    private void Devolve()
    {
        evolution = minStat;
    }


    public IEnumerator CheckForRevive()
    {

        if (faith < 50) // In order to revive, currentFaith needs to be > x. 
        {
            StartCoroutine(Revive()); // Start Revive
            yield break;
        } else
        {
            StartCoroutine(RespawnBuffer()); // Start Respawn
            yield break;
        }
    }

    public virtual IEnumerator Revive()

    {   // REVIVE PLAYER - Complete Reset.
        health = maxStat / 2;
        hunger = maxStat / 2;
        faith = maxStat / 2;

        ChangeAnimationState(PLAYER_REVIVING);

        hasDied = false;

        ChangeAnimationState(PLAYER_IDLE);

        yield break;
    }

    [SerializeField]private float respawnCountdown = 5;

    public virtual IEnumerator RespawnBuffer()
    {
        yield return new WaitForSeconds(respawnCountdown);
        respawn = true;
        ResetGame();
        yield break;
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        print("Game Restarted.");
    }


    public virtual event Action<float, float, float> OnFaithChanged;
    public virtual event Action<float, float, float> OnHungerChanged;

    public bool isBlessed = false;

    [SerializeField] private DisasterManager naturalDisaster;

    public virtual void DepleteFaith(float faithDamage)
    {
        if (!isBlessed)
        {
            OnFaithChanged?.Invoke(faith, minStat, maxStat);

            faith -= faithDamage;
            faithBar.UpdateFaith(faith / maxStat);

            if (faith <= minStat )
            {
                faith = minStat;
                isFaithless = true;
                Debug.Log("Player is faithless!");  
          
            }
            else

            {
                isFaithless = false;
            }
        }
    }

    public virtual void GainFaith(float faithFactor)
    {
        if (!isBlessed)
        {
            faith += faithFactor;
            faithBar.UpdateFaith(faith / maxStat);

            if (faith >= maxStat)
            {
                faith = maxStat;
                Debug.Log("Player has max faith!");
                StartCoroutine(TransendenceTimer());
            }
        }
    }

    public virtual IEnumerator TransendenceTimer()
    {
        yield return new WaitForSeconds(Random.Range(10, 180));

        if (faith >= maxStat)
        {
            faith = maxStat;
            isBlessed = true;

            Debug.Log("Player is blessed.");

            yield return new WaitForSeconds(Random.Range(60, 180));

            if (faith <= maxStat / 1.25f)
            {
                isBlessed = false;
                Debug.Log("Player is no longer blessed.");
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(60, 180));

            isBlessed = false;
            Debug.Log("Player is no longer blessed.");

            yield break;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.UpdateHealth(health / maxStat);

        if (health <= minStat)
        {
            health = minStat;
            //Kill();
        }
    }

    public virtual void Heal(float healFactor)
    {
        health += healFactor;
        healthBar.UpdateHealth(health / maxStat);

        if (health >= maxStat)
        {
            health = maxStat;
        }
    }

    public virtual void Hunger(float hungerFactor)
    {
        hunger -= hungerFactor;
        hungerBar.UpdateHunger(hunger / maxStat);

        OnHungerChanged?.Invoke(hunger, minStat, maxStat);

        if (hunger <= maxStat / 3)
        {
            starving = true;
        }

        if (hunger <= minStat)
        {
            hunger = minStat;
            TakeDamage(0.1f);
        } else
        {
            starving = false;
        }
    }

    public virtual void Evolve(float evolutionFactor)
    {
        evolution += evolutionFactor;
        evolutionBar.UpdateEvolution(evolution / maxStat);

        if (evolution >= maxStat)
        {
            evolution = minStat; // reset evolution ( level up )
        }
    }

    int diseaseIndex;

    public virtual void ContractDisease()
    {
        if (!isDiseased) { 
        diseaseIndex = Random.Range(0, diseaseSeverities.Length - 1);
        NotifyOfDisease(diseaseIndex); // Factor in current health, faith, age etc...
        }
    }

    private void NotifyOfDisease(int diseaseIndex)
    {

        for (int i = 0; i < diseaseSeverities.Length; i++)
        {
            diseaseSeverity = diseaseSeverities[diseaseIndex];
            Debug.Log("Player has been infected with a " + diseaseSeverity + " disease!"); // Make so that there are stages. Mild, Severe, Fatal, Terminal

            isDiseased = true;
        }
    }

    string[] diseaseSeverities = { "mild", "severe", "fatal", "terminal" };
    string diseaseSeverity = "";

    int diseaseMultiplier;

    private void DiseaseDamage()
    {
        switch (diseaseSeverity)
        {
            case "mild":
                diseaseMultiplier = 2;
                TakeDamage(0.00005f * diseaseMultiplier);
                break;

            case "severe":
                diseaseMultiplier = 2;
                TakeDamage(0.00005f * diseaseMultiplier);
                break;

            case "fatal":
                diseaseMultiplier = 2;
                TakeDamage(0.00005f * diseaseMultiplier);
                break;

            case "terminal":
                diseaseMultiplier = 2;
                TakeDamage(0.00005f * diseaseMultiplier);
                break;
        }
    }

    public void HealDisease()
    {
        Debug.Log("Player has miraculously recovered from" + diseaseSeverity + "disease!");
        isDiseased = false;
    }
}