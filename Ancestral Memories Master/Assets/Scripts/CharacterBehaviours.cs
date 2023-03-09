using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class CharacterBehaviours : MonoBehaviour
{

    public PlayerWalk playerWalk;
    public Player player;

    public CamControl cinematicCam;

    public GameObject firstPersonController;

    public bool behaviourIsActive = false;
    public bool dialogueIsActive = false;

    public GodRayControl god;

    // Idle

    const string PLAYER_IDLE = "Player_idle";

    // Emotion

    const string PLAYER_CURIOUS = "Player_curious";
    const string PLAYER_CELEBRATE = "Player_celebrate";
    const string PLAYER_SAD = "Player_sad";
    const string PLAYER_STARVING = "Player_starving";
    const string PLAYER_RAGE = "Player_rage";
    const string PLAYER_SCARED = "Player_scared";

    // PRAYER

    const string PLAYER_PRAYER_START = "Player_PrayerStart";
    const string PLAYER_PRAYER_LOOP = "Player_PrayerLoop";
    const string PLAYER_PRAYER_END = "Player_PrayerEnd";

    const string PLAYER_DANCE_01 = "Player_Dance_01";
    const string PLAYER_DANCE_02 = "Player_Dance_02";
    const string PLAYER_DANCE_03 = "Player_Dance_03";

    const string PLAYER_PICKUP = "Player_PickUp";
    const string PLAYER_STANDINGEAT = "Player_StandingEat";

    const string PLAYER_TOCROUCH = "Player_ToCrouch";

    const string PLAYER_CROUCHDRINK = "Player_CrouchDrink";
    const string PLAYER_CROUCHFORAGING = "Player_CrouchForaging";
    const string PLAYER_CROUCHPLANTSEEDS = "Player_CrouchPlantSeeds";
    const string PLAYER_CROUCHTOSTAND = "Player_CrouchToStand";

    const string PLAYER_KINDLEFIRE = "Player_KindleFire";

    const string PLAYER_HARVEST_TREE = "Player_Harvest_Tree";

    const string PLAYER_SITONFLOOR = "Player_SitOnFloor";
    const string PLAYER_SITTINGFLOORIDLE = "Player_SittingFloorIdle";
    const string PLAYER_STANDUPFROMSIT = "Player_StandUpFromSit";

    const string PLAYER_THUNDERSTRUCK = "Player_thunderstruck";
    const string PLAYER_FALLFLATONFLOOR = "Player_FallFlatOnFloor";
    const string PLAYER_FACEDOWNIDLE = "Player_FaceDownIdle"; 

    const string PLAYER_STANDUPFROMFLOOR = "Player_StandUpFromFloor";
    const string PLAYER_DROWN = "Player_Drown";

    const string PLAYER_VOMIT = "Player_Vomit";

    const string PLAYER_PLAYFLUTE = "Player_PlayFlute";


    const string PLAYER_SLEEPING = "Player_SleepingOnFloor";
  
    public string[] danceAnimClips;

    private float animationLength;

    public AreaManager areaManager;

    public ToolControl tool;

    public GameObject wieldedStoneAxe;
    public GameObject sheathedStoneAxe;

    [SerializeField] private ControlAlpha costumeControl;

    [SerializeField] private CheckIfUnderwater waterCheck;

    private PulseEffectControl pulseControl;

    private float defaultAnimSpeed = 1;
    private float animSpeed = 1;

    private PlayerSoundEffects playerAudioSFX;

    public MusicManager musicManager;

    public TimeCycleManager timeManager;


    void Start()
    {
        tool.Sheathe(wieldedStoneAxe, sheathedStoneAxe);
        waterCheck = player.GetComponent<CheckIfUnderwater>();
        pulseControl = player.GetComponentInChildren<PulseEffectControl>();
        playerAudioSFX = player.GetComponentInChildren<PlayerSoundEffects>();
        vomit.Stop();
        
        //animSpeed = player.activeAnimator.speed;
    }

    public void ChooseBehaviour(string selected, GameObject hitObject)
    {
        animSpeed = defaultAnimSpeed;
        player.AdjustAnimationSpeed(animSpeed);

        switch (selected)
        {
            case "Exit":
                StartCoroutine(ExitGame());
                break;
            case "Pray":
                StartCoroutine(Pray(hitObject));
                break;
            case "Look":
                StartCoroutine(Look());
                break;
            case "Reflect":
                animSpeed = defaultAnimSpeed * 2;
                player.AdjustAnimationSpeed(animSpeed);
                StartCoroutine(Reflect());
                break;
            case "Dance":
                StartCoroutine(Dance(GetRandomAnimation(danceAnimClips)));
                break;
            case "HarvestTree":
                StartCoroutine(HarvestTree(hitObject));
                break;
            case "Heal":
                break;
            case "Eat":
                StartCoroutine(PickMushroom(hitObject));
                break;
            case "Drink":
                StartCoroutine(Drink());
                break;
            case "Enter":
                areaManager.StartCoroutine(areaManager.EnterPortal(hitObject));
                break;
            case "EatApple":
                StartCoroutine(PickupApple());
                break;
            case "KindleFire":
                animSpeed = defaultAnimSpeed * 2;
                player.AdjustAnimationSpeed(animSpeed);
                StartCoroutine(KindleFire(hitObject));
                break;
            case "Talk":
                if (!dialogueIsActive)
                {
                StartCoroutine(Talk(hitObject));
                }
                break;
            case "Sleep":
                StartCoroutine(Sleep(hitObject));
                break;
            case "PlayMusic":
                StartCoroutine(PlayMusic());
                break;
            case "ResetGame":
                StartCoroutine(ResetGame());
                break;
            //Look();
            default:
                Debug.Log("No such behaviour.");
                break;
        }

        print(selected);

        return;
    }

    public IEnumerator ExitGame()
    {

        yield return null;
    }

    public void WalkToward(GameObject hitObject, string selected, RaycastHit rayHit)
    {
        StartCoroutine(playerWalk.WalkToward(hitObject, selected, null, rayHit));
    }

    public void SheatheItem()
    {
        tool.Sheathe(wieldedStoneAxe, sheathedStoneAxe);
    }

    [SerializeField] private LightningStrike lightning;

    int randChance;
    int randTarget = 1;
    private bool isSkeleton = false;

    public IEnumerator Electrocution() {

        animSpeed = defaultAnimSpeed;
        behaviourIsActive = true;

        playerWalk.StopAgentOverride();

        int randChance = Random.Range(0, 2);
        int randTarget = 1;

        if (randChance == randTarget)
        {
            costumeControl.SwitchSkeleton();
            isSkeleton = true;
        }

        player.ChangeAnimationState(PLAYER_THUNDERSTRUCK);

        yield return new WaitForSeconds(GetAnimLength());


        if (isSkeleton)
        {
            costumeControl.SwitchSkeleton();
            isSkeleton = false;
        }
        var maxTimeOnFloor = GetAnimLength() + Random.Range(0, 2);

        player.ChangeAnimationState(PLAYER_FACEDOWNIDLE);

        float timeOnFloor = Random.Range(GetAnimLength(), maxTimeOnFloor);
        yield return new WaitForSeconds(timeOnFloor);
     
        player.ChangeAnimationState(PLAYER_STANDUPFROMFLOOR);
        float time = 0;
        float duration = GetAnimLength();

        while (time <= duration)
        {
            time += Time.deltaTime / duration;

            yield return null;
        }

        if (Input.GetMouseButtonDown(0) || time >= duration)
        {

            behaviourIsActive = false;
            playerWalk.CancelAgentOverride();
            yield break;
        }

        //cinematicCam.ToGameZoom();

    }

    public void StartDeathSequence()
    {
        player.StartCoroutine(player.CheckForRevive());
    }

    public IEnumerator Drown()
    {
        behaviourIsActive = true;

        player.ChangeAnimationState(PLAYER_DROWN);
        yield return new WaitForSeconds(GetAnimLength());

        behaviourIsActive = false;

        StartDeathSequence();

        yield break;

    }

    public bool thirdPersonCamActive = true;
    public bool firstPersonCamActive = false;

    [SerializeField] AudioListenerManager audioListener;

    private void Awake()
    {

        cinematicCam.gameObject.SetActive(true);
        firstPersonController.SetActive(false);

        thirdPersonCamActive = cinematicCam.gameObject.activeInHierarchy;
        firstPersonCamActive = firstPersonController.activeInHierarchy;

    }

    public IEnumerator Look()
    {
        /*cinematicCamActive = !cinematicCamActive;
        firstPersonCamActive = !firstPersonCamActive;

        cinematicCam.gameObject.SetActive(cinematicCamActive);
        firstPersonController.SetActive(firstPersonCamActive);
        */

        behaviourIsActive = true;

        cinematicCam.ToPanoramaZoom();

        //audioListener.StartCoroutine(audioListener.MoveAudioListener(cinematicCam.transform.gameObject, 1f));

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        behaviourIsActive = false;
        cinematicCam.panoramaScroll = false;

        //audioListener.SetDefaultAttenuation();

        cinematicCam.ToGameZoom();

        /*cinematicCamActive = !cinematicCamActive;
       firstPersonCamActive = !firstPersonCamActive;

       cinematicCam.gameObject.SetActive(cinematicCamActive);
       firstPersonController.SetActive(firstPersonCamActive);
         */

        yield break;
    }


    [SerializeField] private float faithFactor = 0.25f;
    [SerializeField] private float hungerFactor = 1f;

    float minPulseTrigger = 1;
    float maxPulseTrigger = 1;
    bool pulseActive = false;

    public AuraParticleControl auraParticles;

    public IEnumerator Pray(GameObject hitObject)
    {
        cinematicCam.scrollOverride = true;
        behaviourIsActive = true;
        //pulseActive = true;

        player.ChangeAnimationState(PLAYER_PRAYER_START);

        player.ChangeAnimationState(PLAYER_PRAYER_LOOP);
        cinematicCam.ToPrayerZoom();

        StartCoroutine(GainFaith());
        auraParticles.StartParticles();
        //StartCoroutine(PrayerPulseEffect());
        //god.StartGodRay(hitObject.transform, false);


        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));


        auraParticles.StopParticles();
       // pulseActive = false;

        player.ChangeAnimationState(PLAYER_PRAYER_END);

        cinematicCam.scrollOverride = false;
        behaviourIsActive = false;
        cinematicCam.ToGameZoom();
        yield break;
        
    }

    [SerializeField] private PlayFlute fluteControl;

    private IEnumerator PlayMusic()
    {
        cinematicCam.scrollOverride = true;
        behaviourIsActive = true;

        player.ChangeAnimationState(PLAYER_PLAYFLUTE);

        //cinematicCam.ToPlayMusicZoom();

        //cinematicCam.StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));

        fluteControl.EnableFluteControl();


        Debug.Log("Right click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(1));

        behaviourIsActive = false;
        player.ChangeAnimationState(PLAYER_IDLE);
        fluteControl.StopAll();
        cinematicCam.scrollOverride = false;
        cinematicCam.ToGameZoom();

        yield break;
    }

    public IEnumerator Sleep(GameObject hitObject)
    {
        cinematicCam.scrollOverride = true;
        behaviourIsActive = true;

        if (player.hunger <= 25)
        {
            player.ChangeAnimationState(PLAYER_FALLFLATONFLOOR);
            yield return new WaitUntil(() => player.activeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime - Mathf.Floor(player.activeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime) > 0.99f);

        } else
        {
            player.ChangeAnimationState(PLAYER_TOCROUCH);
            yield return new WaitUntil(() => player.activeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime - Mathf.Floor(player.activeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime) > 0.99f);

        }

        player.ChangeAnimationState(PLAYER_SLEEPING);

        Debug.Log("Do you want to sleep?");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        cinematicCam.scrollOverride = false;
        behaviourIsActive = false;

        cinematicCam.ToGameZoom();

        yield break;

    }

    private IEnumerator PrayerPulseEffect()
    {
        while (pulseActive)
        {
            interval = Random.Range(minPulseTrigger, maxPulseTrigger);

            yield return new WaitForSeconds(interval);

            pulseControl.Pulse();
            yield return null;
        }

        yield break;
    }

    private IEnumerator GainFaith()
    {
        while (behaviourIsActive)
        {
            player.GainFaith(faithFactor);
            yield return null;
        }

        yield break;
    }


    private IEnumerator LoseFaith()
    {
        while (behaviourIsActive)
        {
            if (player.isBlessed)
            {
                player.isBlessed = false;
            }
            player.DepleteFaith(faithFactor);
            yield return null;
        }
    }


    public virtual string GetRandomAnimation(string[] animClips)
    {
        string randomAnimation = animClips[Random.Range(0, animClips.Length)];

        return randomAnimation;
    }

    public IEnumerator Dance(string randomDanceAnim)
    {
        behaviourIsActive = true;

        StartCoroutine(GainFaith());

        player.ChangeAnimationState(randomDanceAnim);

        cinematicCam.ToPanoramaZoom();
        //cinematicCam.StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        player.ChangeAnimationState(PLAYER_IDLE);

        behaviourIsActive = false;
        cinematicCam.panoramaScroll = false;

        cinematicCam.ToGameZoom();
        yield break;
    }

    public GameObject frontFacingPivot;
    public GameObject frontFacingAngledPivot;
    public GameObject backFacingPivot;
    public GameObject lookAtTarget;

    [SerializeField] private PickUpObject pickUpManager;

    [SerializeField] private float psychBuffer = 60f;

    public IEnumerator PickMushroom(GameObject hitObject)
    {
        behaviourIsActive = true;

        pickUpManager.pickedUpObject = hitObject;

        player.ChangeAnimationState(PLAYER_PICKUP);
        cinematicCam.ToActionZoom();
        cinematicCam.StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        player.ChangeAnimationState(PLAYER_STANDINGEAT);

        yield return new WaitUntil(() => player.activeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime - Mathf.Floor(player.activeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime) > 0.99f);

        int minChance = 0;
        int maxChance = 1;

        float chance = Random.Range(minChance, maxChance);

        if (chance <= maxChance / 2)
        {
            if (!isPsychdelicMode)
            {
                StartCoroutine(PsychedelicModeBuffer());
                StartCoroutine(HealHunger());
            }
        }
        else if (chance >= maxChance / 2)
        {
            StartCoroutine(Vomit());

            if (isPsychdelicMode)
            {
                isPsychdelicMode = false;
            }
        }

        pickUpManager.DestroyPickup();
        behaviourIsActive = false;
        cinematicCam.ToGameZoom();

        yield break;
    }

    public bool psychModeIncoming = false;

    private IEnumerator PsychedelicModeBuffer()
    {

        if (!isPsychdelicMode)
        {
            psychModeIncoming = true;

            yield return new WaitForSeconds(psychBuffer);

            psychModeIncoming = false;
            cinematicCam.ToPsychedelicZoom();
      
        }

        yield break;
    }

    private IEnumerator HealHunger()
    {
        float time = 0;

        while (time <= 5f)
        {
            player.HealHunger(hungerFactor);
            time += Time.deltaTime * 2f;

            yield return null;
        }

        yield break;
    }


    public IEnumerator PickupApple()
    {
        behaviourIsActive = true;

        cinematicCam.ToActionZoom();
        StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));

        player.ChangeAnimationState(PLAYER_PICKUP);
        yield return new WaitUntil(() => player.activeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime - Mathf.Floor(player.activeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime) > 0.99f);

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        player.ChangeAnimationState(PLAYER_STANDINGEAT);

        behaviourIsActive = false;

        yield break;

    }

    public ParticleSystem vomit;


    public IEnumerator Vomit()
    {
        behaviourIsActive = true;
        // cinematicCam.ToGameZoom();
        //cinematicCam.StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));
        player.ChangeAnimationState(PLAYER_VOMIT);
    
        vomit.Play();
       yield return new WaitUntil(() => player.activeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime - Mathf.Floor(player.activeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime) > 0.99f);
        behaviourIsActive = false;
        vomit.Stop();
        yield break;

    }

    public IEnumerator Drink()
    {
        behaviourIsActive = true;

        player.ChangeAnimationState(PLAYER_TOCROUCH);
       
        player.ChangeAnimationState(PLAYER_CROUCHDRINK);
        yield return new WaitUntil(() => player.activeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime - Mathf.Floor(player.activeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime) > 0.99f);


        player.ChangeAnimationState(PLAYER_CROUCHTOSTAND);

        if (player.faith <= player.maxStat / 2)
        {
            StartCoroutine(Vomit());
            yield break;
        }
        else if (player.faith >= player.maxStat / 2)
        {
            behaviourIsActive = false;
            cinematicCam.ToGameZoom();

            yield break;
        }

    }

    public IEnumerator ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        yield return null;
    }

    float timeMultiplyFactor = 1f;

    public IEnumerator Reflect()
    {
        
        behaviourIsActive = true;

        cinematicCam.ToActionZoom();
        StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));

        player.ChangeAnimationState(PLAYER_SITTINGFLOORIDLE);

        timeMultiplyFactor = 2f;
        StartCoroutine(ChangeTimeScale(timeMultiplyFactor));

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        player.ChangeAnimationState(PLAYER_STANDUPFROMSIT);

        behaviourIsActive = false;

        cinematicCam.ToGameZoom();

        yield break;
    }

    public IEnumerator ChangeTimeScale(float factor)
    {
        while (behaviourIsActive)
        {
            timeManager.timeMultiplier = factor;
            yield return null;
        }

        timeManager.timeMultiplier = timeManager.defaultTimeMultiplier;

        yield break;
    }

    public bool isPsychdelicMode = false;

    [SerializeField] float minAnimationSpeed = 1;
    [SerializeField] float maxAnimationSpeed = 4;
    float interval;

    private float camMoveDuration = 1f;

    public IEnumerator HarvestTree(GameObject hitObject)
    {
        TreeDeathManager treeDeathManager = hitObject.GetComponentInChildren<TreeDeathManager>();

        //killThreshold = hitObject.transform.localScale.x;
        playerAudioSFX.numberOfHits = 0;
        playerAudioSFX.targetTree = hitObject;

        tool.Wield(wieldedStoneAxe, sheathedStoneAxe);

        behaviourIsActive = true;
        StartCoroutine(LoseFaith());


        float time = 0;

        interval = Random.Range(minAnimationSpeed, maxAnimationSpeed);

        cinematicCam.ToActionZoom();

        while (time <= interval && !Input.GetMouseButtonDown(0) && !treeDeathManager.treeDead)
        {
            interval = Random.Range(minAnimationSpeed, maxAnimationSpeed);

            minAnimationSpeed = player.activeAnimator.speed;

            player.ChangeAnimationState(PLAYER_HARVEST_TREE);
            player.activeAnimator.speed = Mathf.Lerp(minAnimationSpeed, interval, time);

            time += Time.deltaTime / interval;

            yield return null;
        }

        player.activeAnimator.speed = 1f;

        player.ChangeAnimationState(PLAYER_IDLE);

        behaviourIsActive = false;
        cinematicCam.ToGameZoom();

        SheatheItem();

        yield break;
    }

    [SerializeField] private GameObject campFire;

    public IEnumerator KindleFire(GameObject hitObject)
    {
        tool.Wield(wieldedStoneAxe, sheathedStoneAxe);

        behaviourIsActive = true;

        player.ChangeAnimationState(PLAYER_TOCROUCH);
        yield return new WaitUntil(() => player.activeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime - Mathf.Floor(player.activeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime) > 0.99f);

        cinematicCam.ToActionZoom();
        StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingAngledPivot, lookAtTarget, camMoveDuration));

        player.ChangeAnimationState(PLAYER_KINDLEFIRE);
        yield return new WaitUntil(() => player.activeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime - Mathf.Floor(player.activeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime) > 0.99f);

        player.ChangeAnimationState(PLAYER_IDLE);

        behaviourIsActive = false;
        cinematicCam.ToGameZoom();

        GameObject newFire = Instantiate(campFire, hitObject.transform.position, Quaternion.identity);
        hitObject.transform.SetParent(newFire.transform);

        LookAtTarget faceCamera = newFire.GetComponentInChildren<LookAtTarget>();
        faceCamera.target = cinematicCam.transform;

        Destroy(hitObject);
        //hitObject.GetComponent<Renderer>().enabled = false;

        SheatheItem();

        yield break;
    }

    private Dialogue dialogue;

    public IEnumerator Talk(GameObject hitObject)
    {
        Debug.Log("HITOBJECT: " + hitObject);

        //GameObject lookAtTarget = hitObject;
        //GameObject DefaultCamPivot = player.transform.root.Find("DefaultCamPosition").gameObject;
        //GameObject NPCPivot = hitObject.transform.Find("NPCpivot").gameObject;
        //StartCoroutine(cinematicCam.MoveCamToPosition(NPCPivot, lookAtTarget, 1f));

        dialogueIsActive = true;
        dialogue = hitObject.transform.GetComponent<Dialogue>();
        dialogue.StartDialogue(dialogue, player);

        player.ChangeAnimationState(PLAYER_IDLE);

        player.transform.LookAt(hitObject.transform);

        cinematicCam.ToDialogueZoom();

        yield return new WaitUntil(() => dialogue.dialogueActive == false);

        //player.ChangeAnimationState(PLAYER_IDLE);

        dialogueIsActive = false;
        cinematicCam.ToGameZoom();
        yield break;

        //lookAtTarget = player.transform.gameObject;
        //StartCoroutine(cinematicCam.MoveCamToPosition(DefaultCamPivot, lookAtTarget, 15f));


    }

    public float animLength;

    private float GetAnimLength()
    {
        animLength = player.activeAnimator.GetCurrentAnimatorStateInfo(0).length;
        return animLength;
    }

  


}
