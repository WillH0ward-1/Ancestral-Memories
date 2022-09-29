using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterClass : MonoBehaviour
{

<<<<<<< HEAD
=======
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

>>>>>>> parent of b61090f (ShamanSimPrototype-11.4)
    private int playerAge = 24;
    private int ageToDie = 0;


    public bool playerKilledByGod = false;



<<<<<<< HEAD
=======
    public CheckIfUnderwater underwaterCheck;

    public PlayerWalk playerWalk;

    //[SerializeField] private GodRayControl god;

    public List<GameObject> activeAnimators = new List<GameObject>();
    public List<GameObject> inactiveAnimators = new List<GameObject>();

    [SerializeField] private float animationCrossFade = 2f;
    //private AuraControl auraControl;

    public bool respawn;

    //public GodRayControl godRay;

    public ControlAlpha alphaControl;

    public Shake earthQuake;

    // FUNCTIONS ============================================================

    private void Awake()
    {
        InitAnimators();

        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentFaith = maxFaith;
        currentEvolution = minEvolution;

        playerIsDiseased = false;


    }

    void InitAnimators()
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


    public void SwitchAnimators()
    {
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

    [SerializeField] private float crossFadeLength;

    public void ChangeAnimationState(string newState)
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

        /*
        if (currentEvolution <= 25)
        {
            alphaControl.playerIsHuman = false;
        } else
        {
            alphaControl.playerIsHuman = true;
        }
        */

        // UPDATE PLAYER STATS

        float hungerMultipler = 0.5f;

        float faithMultiplier = 0.5f;

        float evolutionMultiplier = 0.5f;

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
            KillPlayer();
        }
    }

    public void KillPlayer()
    {

        currentHealth = minHealth;
        playerHasDied = true;
        CheckForRevival();
        StartCoroutine(CheckForRevival());
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
            earthQuake.start = true;

            currentFaith = minFaith;

            playerIsFaithless = true;

            Debug.Log("Player is faithless!");

            //  Trigger chance to be struck down by god / natural disasters
            // Have a vocal cue + thunder rumble 'Heretic!'. First Testament style.

        } else
        {
            earthQuake.start = false;

            playerIsFaithless = false;
        }

    }

    public IEnumerator CheckForRevival()
    {
        float animationLength = activeAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        if (currentFaith < 50) // In order to revive, currentFaith needs to be > x. 
        {
            playerIsReviving = true;
            StartCoroutine(RevivePlayer());
        } else
        {
            StartCoroutine(RespawnBuffer());
        }
    }

    public IEnumerator RevivePlayer()

    {   // REVIVE PLAYER - Complete Reset.

        playerHasDied = false;
        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentFaith = maxFaith;

        ChangeAnimationState(PLAYER_REVIVING);

        float animationLength = activeAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        //StartCoroutine(god.TriggerGodRay());

        //godRay.godRay = false;
        playerIsReviving = false;

        ChangeAnimationState(PLAYER_IDLE);
    }

    public IEnumerator RespawnBuffer()
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
>>>>>>> parent of b61090f (ShamanSimPrototype-11.4)
}