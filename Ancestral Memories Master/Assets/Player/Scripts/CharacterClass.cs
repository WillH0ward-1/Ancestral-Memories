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

    public float currentHealth;
    public float currentHunger;
    public float currentFaith;
    public float currentEvolution;

    public bool playerIsStarving = false;
    public bool playerHasStarved = false;

    public bool playerIsDiseased = false;

    public bool playerIsFaithless = false;
    public bool playerKilledByGod = false;

    public bool playerHasDied = false;
    public bool playerIsReviving = false;

    private GameManagement gameManager;

    public CheckIfUnderwater underwaterCheck;

    public PlayerWalk playerWalk;

    [SerializeField] private GodRayControl god;

    public List<GameObject> activeAnimators = new List<GameObject>();
    public List<GameObject> inactiveAnimators = new List<GameObject>();

    [SerializeField] private float animationCrossFade = 2f;
    //private AuraControl auraControl;

    public bool respawn;

    public GodRayControl godRay;

    public ControlAlpha alphaControl;

    // FUNCTIONS ============================================================

    private void Awake()
    {
        SwitchAnimators();

        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentFaith = maxFaith;
        currentEvolution = minEvolution;

        playerIsDiseased = false;


    }

    public void SwitchAnimators()
    {

     //   AnimatorClipInfo[] CurrentClipInfo = activeAnimator.GetCurrentAnimatorClipInfo(0);

        if (alphaControl.playerIsHuman == false) // If player is already monkey, it has to switch to human & vice versa.
        {
            inactiveAnimators.Remove(alphaControl.humanObject);
            activeAnimators.Add(alphaControl.monkeyObject);

            if (activeAnimators.Contains(alphaControl.humanObject))
            {
                activeAnimators.Remove(alphaControl.humanObject);
                inactiveAnimators.Add(alphaControl.humanObject);
            }

            AssignAnimators();
            AssignInactiveAnimators();
        }

        else if (alphaControl.playerIsHuman == true)
        {
            inactiveAnimators.Remove(alphaControl.monkeyObject);
            activeAnimators.Add(alphaControl.humanObject);

            if (activeAnimators.Contains(alphaControl.monkeyObject))
            {
                activeAnimators.Remove(alphaControl.monkeyObject);
                inactiveAnimators.Add(alphaControl.monkeyObject);
            }

            AssignAnimators();
            AssignInactiveAnimators();
        }

         void AssignAnimators()
        {
            foreach (GameObject g in activeAnimators)
            {
                foreach (Animator a in g.GetComponentsInChildren<Animator>())
                {
                    activeAnimator = a;
                }
            }
        }

         void AssignInactiveAnimators()
        {
            foreach (GameObject g in inactiveAnimators)
            {
                foreach (Animator a in g.GetComponentsInChildren<Animator>())
                {
                    inactiveAnimator = a;
                }
            }
        }
    }

    [SerializeField] private float crossFadeLength;

    public void ChangeAnimationState(string newState)
    {

        float crossFadeLength = animationCrossFade;

        if (currentState == newState)
        {
            return;
        }

        activeAnimator.CrossFadeInFixedTime(newState, crossFadeLength);
        inactiveAnimator.CrossFadeInFixedTime(newState, crossFadeLength);

        currentState = newState;
    }

    public void AdjustAnimationSpeed(float newSpeed)
    {
        activeAnimator.speed = newSpeed;
        inactiveAnimator.speed = newSpeed;
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
        if (currentFaith < 50) // In order to revive, currentFaith needs to be > x. 
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
        float animationLength = activeAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);
        RevivePlayer();
    }

    public IEnumerator ResetPlayer()
    {
        float animationLength = activeAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        playerHasDied = false;
        godRay.godRay = false;
        playerIsReviving = false;

        ChangeAnimationState(PLAYER_IDLE);

        Debug.Log("Player Has Been Resurrected!");
    }

    public IEnumerator RespawnBuffer()
    {
        float animationLength = activeAnimator.GetCurrentAnimatorStateInfo(1).length;
        yield return new WaitForSeconds(animationLength); // Wait for this many seconds before respawning. This may be an audio cue in future.
        respawn = true;
        ResetGame();
    }

    private void RevivePlayer()
    {
        // REVIVE PLAYER - Complete Reset.

        StartCoroutine(god.TriggerGodRay());

        ChangeAnimationState(PLAYER_REVIVING);

        StartCoroutine(ResumeGame());

        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentFaith = maxFaith;
    }

    public IEnumerator ResumeGame()
    {
        StartCoroutine(ResetPlayer());
        yield return null;
    }


    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        print("Game Restarted.");
    }
}