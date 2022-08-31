using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterClass : MonoBehaviour
{
    [SerializeField]
    public Animator animator;

    private string currentState;

    // ANIMATION STATES ==================================================

    // These are string references to animations, triggered by various events. (See the Animator tab)

    const string PLAYER_IDLE = "Player_idle";
    const string PLAYER_STARVINGIDLE = "Player_starvingIdle";
    const string PLAYER_STARVINGCRAWL = "Player_starvingCrawl";
    const string PLAYER_STARVINGCRAWLCRITICAL = "Player_starvingCrawlCritical";

    // Death

    const string PLAYER_DEATHOLDAGE = "Player_deathOldAge";

    const string PLAYER_STARVE = "Player_starve";
    const string PLAYER_DROWN = "Player_drown";
    const string PLAYER_POISONED = "Player_poisoned";
    const string PLAYER_THUNDERSTRUCK = "Player_thunderStruck";
    const string PLAYER_DEHYDRATIONDEATH = "Player_deathThirst";

    // Emotion

    const string PLAYER_CURIOUS = "Player_curious";
    const string PLAYER_CELEBRATE = "Player_celebrate";
    const string PLAYER_SAD = "Player_sad";
    const string PLAYER_STARVING = "Player_starving";
    const string PLAYER_RAGE = "Player_rage";
    const string PLAYER_SCARED = "Player_scared";

    // Ailments

    const string PLAYER_DISEASED = "Player_diseased";

    // Ressurection

    const string PLAYER_REVIVING = "Player_revived";

    // PLAYER STATS =======================================================

    private int playerAge = 24;
    private int ageToDie = 0;

    private int maxHealth = 100;
    private int minHealth = 0;

    private int maxHunger = 100;
    private int minHunger = 0;

    private int maxFaith = 100;
    private int minFaith = 0;

    private int maxEvolution = 100;
    private int minEvolution = 0;

    public HealthBar playerHealth;
    public HungerBar playerHunger;
    public FaithBar playerFaith;
    public EvolutionBar evolutionBar;

    [SerializeField] public float currentHealth;
    [SerializeField] public float currentHunger;
    [SerializeField] public float currentFaith;
    [SerializeField] public float currentEvolution;

    public bool playerIsStarving = false;
    public bool playerHasStarved = false;

    public bool playerIsDiseased = false;

    public bool playerIsFaithless = false;
    public bool playerKilledByGod = false;

    public bool playerHasDied = false;
    public bool playerIsReviving = false;

    public CheckIfUnderwater underwaterCheck;

    public PlayerWalk playerWalk;

    //private AuraControl auraControl;

    public bool respawn;

    public GodRayControl godRay;

    // FUNCTIONS ============================================================

    private void Start()
    {
        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentFaith = maxFaith;

        playerIsDiseased = false;

        currentEvolution = minEvolution;

        animator = GetComponent<Animator>();

    }

    public float crossFadeLength = 2f;

    public void ChangeAnimationState(string newState)
    {
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

    private void Update()

    {

        // DROWN

        if (underwaterCheck.isUnderwater == true && underwaterCheck.playerDrowning == true)
        {
            Debug.Log("Drowning!");

            TakeDamage(1);

            if (playerHasDied == true)
            {
                ChangeAnimationState(PLAYER_DROWN);
                underwaterCheck.playerDrowning = false;
                underwaterCheck.playerHasDrowned = true;
            }
        }

        // DEAL STARVE DAMAGE

        if (playerIsStarving == true && playerHasDied == false)
        {
            Debug.Log("Starving!");

            //ChangeAnimationState(PLAYER_STARVING);

            TakeDamage(0.1f);

            if (playerHasDied == true)
            {
                ChangeAnimationState(PLAYER_STARVE);
                playerIsStarving = false;
                playerHasStarved = true;
            }
        }

 
        if (currentEvolution <= 25)
        {
            //monkeyMorph.playerIsMonkey = true;

        }


        // UPDATE PLAYER STATS

        float hungerMultipler = 0.5f;

        float faithMultiplier = 0.5f;

        float evolutionMultiplier = 0.25f;

        // If multipliers set to negative, it should drop faster and vice versa for positive.

        GetHungry(0.1f * hungerMultipler);
        DepleteFaith(0.1f * faithMultiplier);
        Evolve(0.1f * evolutionMultiplier);
    }

    public void ContractDisease()
    {
        string[] diseaseSeverities = { "mild", "severe", "fatal", "terminal" };
        int diseaseIndex = diseaseSeverities.Length - 1;
        string diseaseSeverity = "";

        CaculateChanceOfDisease();

        void CaculateChanceOfDisease()
        {
            // Factor in current health, faith, age etc...
            NotifyOfDisease();
        }

        void NotifyOfDisease(){

            for (int i = 0; i < diseaseSeverities.Length; i++)
            {
                diseaseSeverity = diseaseSeverities[diseaseIndex];
                Debug.Log("Player has been infected with a " + diseaseSeverity + " disease!"); // Make so that there are stages. Mild, Severe, Fatal, Terminal

                playerIsDiseased = true;
            }
        }

        while (playerIsDiseased == true)
        {
            int diseaseMultiplier;

            if (diseaseSeverity == "mild")
            {
                diseaseMultiplier = 2;
                TakeDamage(0.00005f * diseaseMultiplier);
            }
            if (diseaseSeverity == "severe")
            {
                diseaseMultiplier = 3;
                TakeDamage(0.0005f * diseaseMultiplier);
            }
            if (diseaseSeverity == "fatal")
            {
                diseaseMultiplier = 4;
                TakeDamage(0.005f * diseaseMultiplier);
            }
            if (diseaseSeverity == "terminal")
            {
                diseaseMultiplier = 5;
                TakeDamage(0.05f * diseaseMultiplier);
            }
        }

    }

    public void HealDisease()
    {
        Debug.Log("Player has miraculously recovered from disease!");
        playerIsDiseased = false;
    }

    public void TakeDamage(float damage)
    {

        currentHealth -= damage;
        playerHealth.UpdateHealth((float)currentHealth / (float)maxHealth);

        if (currentHealth <= minHealth)
        {
            currentHealth = minHealth;
            SetHealth(0);
            playerHasDied = true;
            CheckForRevival();
        }
    }

    public void SetHealth(int value)
    {
        currentHealth = value;

        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
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



    public void GetHungry(float hunger)
    {

        currentHunger -= hunger;
        playerHunger.UpdateHunger(currentHunger / maxHunger);

        if (currentHunger <= minHunger)
        {
            playerIsStarving = true;
        } else
        {
            playerIsStarving = false;
        }
    }

    public event Action<int, int> OnFaithChanged;

    public void DepleteFaith(float faith)
    {

        currentFaith -= faith;

        OnFaithChanged?.Invoke((int)currentFaith, maxFaith);

        playerFaith.UpdateFaith(currentFaith / maxFaith);

        if (currentFaith <= minFaith)
        {
            currentFaith = minFaith;

            playerIsFaithless = true;

            Debug.Log("Player is faithless!");

            //  Trigger chance to be struck down by god / natural disasters
            // Have a vocal cue + thunder rumble 'Heretic!'. First Testament style.

        } else
        {
            playerIsFaithless = false;
        }

    }

    public void CheckForRevival()
    {
        if (currentFaith < 50) // currentFaith should be > x. 
        {
            playerIsReviving = true;
            PrepareRevive();
        } else
        {
            PrepareRespawn();
        }
    }

    private void PrepareRespawn()
    {
        StartCoroutine(RespawnBuffer());
    }

    private void PrepareRevive()
    {
        
        StartCoroutine(GetReviveAnimationLength());
    }

    IEnumerator GetReviveAnimationLength()
    {
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);
        RevivePlayer();
    }

    private void RevivePlayer()
    {
        // REVIVE PLAYER - Complete Reset.

        godRay.godRay = true;

        ChangeAnimationState(PLAYER_REVIVING);

        StartCoroutine(ResumeGame());

        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentFaith = maxFaith;
    }

    IEnumerator ResumeGame()
    {
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        playerHasDied = false;
        godRay.godRay = false;
        playerIsReviving = false;

        ChangeAnimationState(PLAYER_IDLE);
    
        Debug.Log("Player Has Been Resurrected!");
    }

    // RESPAWN + RESET GAME

    IEnumerator RespawnBuffer()
    {
        float animationLength = animator.GetCurrentAnimatorStateInfo(1).length;
        yield return new WaitForSeconds(animationLength); // Wait for this many seconds before respawning. This may be an audio cue in future.
        respawn = true;
        ResetGame();
    }

    private void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        print("Game Restarted.");
    }
}