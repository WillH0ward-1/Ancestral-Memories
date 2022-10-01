using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterClass : MonoBehaviour
{

    private Animator activeAnimator;

    private Animator inactiveAnimator;

    private string currentState;

    // ANIMATION STATES ==================================================

    // These are string references for animations, triggered by various events. (See the Animator tab)

    // Idle

    const string PLAYER_IDLE = "Player_idle";
    const string PLAYER_STARVINGIDLE = "Player_starvingIdle";
    const string PLAYER_STARVINGCRAWL = "Player_starvingCrawl";
    const string PLAYER_STARVINGCRAWLCRITICAL = "Player_starvingCrawlCritical";

    // Emotion

    const string PLAYER_CURIOUS = "Player_curious";
    const string PLAYER_CELEBRATE = "Player_celebrate";
    const string PLAYER_SAD = "Player_sad";
    const string PLAYER_STARVING = "Player_starving";
    const string PLAYER_RAGE = "Player_rage";
    const string PLAYER_SCARED = "Player_scared";

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

    private int maxHealth = 100;
    private int minHealth = 0;

    private int maxHunger = 100;
    private int minHunger = 0;

    private int maxFaith = 100;
    private int minFaith = 0;

    private int maxEvolution = 100;
    private int minEvolution = 0;

    public HealthBar healthBar;
    public HungerBar hungerBar;
    public FaithBar faithBar;
    public EvolutionBar evolutionBar;

    public float health;
    public float currentHunger;
    public float currentFaith;
    public float evolution;

    public bool starving = false;
    public bool hasStarved = false;

    public bool isDiseased = false;

    public bool isFaithless = false;
    public bool killedByGod = false;

    public bool hasDied = false;
    public bool isReviving = false;

    public CheckIfUnderwater underwaterCheck;

    public PlayerWalk playerWalk;

    public List<GameObject> activeAnimators = new List<GameObject>();
    public List<GameObject> inactiveAnimators = new List<GameObject>();

    [SerializeField] private float animationCrossFade = 2f;

    public bool respawn;

    //public GodRayControl godRay;

    public ControlAlpha alphaControl;

    public Shake earthQuake;

    // FUNCTIONS ============================================================

    public virtual void Awake()
    {
        name = "Jon";

        InitAnimators();

        health = maxHealth;
        currentHunger = maxHunger;
        currentFaith = maxFaith;
        evolution = minEvolution;

        isDiseased = false;
    }

    public virtual void InitAnimators()
    {
        var humanState = alphaControl.humanObject;
        var monkeyState = alphaControl.monkeyObject;


        if (alphaControl.playerIsHuman == false) // If player is monkey
        {
            activeAnimators.Remove(humanState);
            activeAnimators.Add(monkeyState);

            inactiveAnimators.Remove(monkeyState);
            inactiveAnimators.Add(humanState);

        } else if (alphaControl.playerIsHuman == true){// If player is Human

            activeAnimators.Remove(monkeyState);
            activeAnimators.Add(humanState);

            inactiveAnimators.Remove(humanState);
            inactiveAnimators.Add(monkeyState);
        }
    }


    public virtual void SwitchAnimators()
    {
        AssignAnimators();
        AssignInactiveAnimators();
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

    //private float crossFadeLength;

    public virtual void ChangeAnimationState(string newState)
    {
        SwitchAnimators();

        float crossFadeLength = animationCrossFade;

        if (currentState == newState)
        {
            return;
        }

        activeAnimator.CrossFadeInFixedTime(newState, crossFadeLength);
        inactiveAnimator.CrossFadeInFixedTime(newState, crossFadeLength);

        currentState = newState;
    }

    public virtual void AdjustAnimationSpeed(float newSpeed)
    {
        activeAnimator.speed = newSpeed;
        inactiveAnimator.speed = newSpeed;
    }

    public virtual void Update()

    {

        // DEAL STARVE DAMAGE

        if (starving && !hasDied)
        {
            Debug.Log("Starving!");

            //ChangeAnimationState(PLAYER_STARVING);

            if (hasDied)
            {
                ChangeAnimationState(PLAYER_STARVE);
                starving = false;
                hasStarved = true;
            }
        }

        if (evolution <= evolutionThreshold)
        {
            alphaControl.playerIsHuman = false;
        } else
        {
            alphaControl.playerIsHuman = true;
        }

        // DROWN

        if (underwaterCheck.isUnderwater && underwaterCheck.playerDrowning)
        {
            Debug.Log("Drowning!");

            UpdateStats(1f, 0, 0, 0);

            if (hasDied)
            {
                ChangeAnimationState(PLAYER_DROWN);
                underwaterCheck.playerDrowning = false;
                underwaterCheck.playerHasDrowned = true;
            }
        }

        // UPDATE PLAYER STATS 

        UpdateStats(0, 0.1f, 0.1f, 0.1f);
    }

    public virtual void SetHealth(int value)
    {
        health = value;

        if (health >= maxHealth)
        {
            health = maxHealth;
        }
    }


    public virtual void Kill()
    {
        health = minHealth;
        hasDied = true;
        StartCoroutine(CheckForRevive());
    }

    private void Devolve()
    {
        evolution = minEvolution;
    }


    public virtual IEnumerator CheckForRevive()
    {
        float animationLength = activeAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        if (currentFaith < 50) // In order to revive, currentFaith needs to be > x. 
        {
            isReviving = true;
            StartCoroutine(Revive()); // Start Revive
        } else
        {
            StartCoroutine(RespawnBuffer()); // Start Respawn
        }
    }

    public virtual IEnumerator Revive()

    {   // REVIVE PLAYER - Complete Reset.

        hasDied = false;
        health = maxHealth;
        currentHunger = maxHunger;
        currentFaith = maxFaith;

        ChangeAnimationState(PLAYER_REVIVING);

        float animationLength = activeAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        //StartCoroutine(god.TriggerGodRay());

        //godRay.godRay = false;
        isReviving = false;

        ChangeAnimationState(PLAYER_IDLE);
    }

    public virtual IEnumerator RespawnBuffer()
    {
        float animationLength = activeAnimator.GetCurrentAnimatorStateInfo(1).length;
        yield return new WaitForSeconds(animationLength); // Wait for this many seconds before respawning. This may be an audio cue in future.
        respawn = true;
        ResetGame();
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        print("Game Restarted.");
    }


    public virtual event Action<int, int> OnFaithChanged;

    public virtual void UpdateStats(float healthDamage, float faithDamage, float hunger, float evolveMultiplier)
    {

        OnFaithChanged?.Invoke((int)currentFaith, maxFaith);

        health -= healthDamage;
        currentFaith -= faithDamage;
        currentHunger -= hunger;
        evolution += evolveMultiplier;

        healthBar.UpdateHealth(health / maxHealth);
        faithBar.UpdateFaith(currentFaith / maxFaith);
        hungerBar.UpdateHunger(currentHunger / maxHunger);
        evolutionBar.UpdateEvolution(evolution / maxEvolution);

        if (health <= minHealth)
        {
            Kill();
        }

        if (currentHunger <= minHunger)
        {
            currentHunger = minHunger;
            starving = true;
        } else
        {
            starving = false;
        }

        if (evolution >= maxEvolution)
        {
            evolution = minEvolution; // reset evolution ( level up )
        }

        if (currentFaith <= minFaith)
        {
            currentFaith = minFaith;
            isFaithless = true;

            Debug.Log("Player is faithless!");

            //earthQuake.start = true;

            //  Trigger chance to be struck down by god / natural disasters
            // vocal cue + thunder rumble 'Heretic!'. First Testament style.

        }

        else

        {
            earthQuake.start = false;
            isFaithless = false;
        }
    }
}