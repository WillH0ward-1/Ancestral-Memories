using System;
using System.Collections;
using System.Collections.Generic;
//using FMOD.Studio;
using ProceduralModeling;
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

    private AudioSFXManager playerAudioSFX;

    public MusicManager musicManager;
    public CloudControl clouds;

    public TimeCycleManager timeManager;

    public MapObjGen mapObjGen;

    private Animator animator;

    public LayerMask groundLayer;

    public string lastState;

    //EVENT_CALLBACK callbackDelegate;

    void Start()
    {
        // callbackDelegate = new EVENT_CALLBACK(ProgrammerCallBack.ProgrammerInstCallback);
        player = GetComponentInChildren<Player>();
        waterCheck = GetComponentInChildren<CheckIfUnderwater>();
        pulseControl = GetComponentInChildren<PulseEffectControl>();
        playerAudioSFX = GetComponentInChildren<AudioSFXManager>();
        vomit.Stop();
        tool.Sheathe(wieldedStoneAxe, sheathedStoneAxe);
        pickUpManager = player.GetComponentInChildren<PickUpObject>();
        animator = GetComponentInChildren<Animator>();

        groundLayer = LayerMask.GetMask("Ground");

        //animSpeed = player.activeAnimator.speed;
    }

    HumanAI humanAI;

    public void ChangeState(string selected, GameObject hitObject)
    {
        lastState = selected;

        animSpeed = defaultAnimSpeed;
        player.AdjustAnimationSpeed(animSpeed);

        if (hitObject != null && hitObject.CompareTag("Human"))
        {
             humanAI = hitObject.transform.GetComponentInParent<HumanAI>();
        }

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
                StartCoroutine(Dance());
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
            case "InstructHarvest":
                if (humanAI != null)
                {
                    humanAI.ChangeState(HumanAI.AIState.Harvest);
                }
                else
                {
                    Debug.Log("No HumanAI component found on " + hitObject + "!");
                }
                break;
            case "InstructHunt":
                if (humanAI != null)
                {
                    humanAI.ChangeState(HumanAI.AIState.HuntMeat);
                }
                else
                {
                    Debug.Log("No HumanAI component found on " + hitObject + "!");
                }
                break;
            case "PlayMusic":
                StartCoroutine(PlayMusic());
                break;
            case "ResetGame":
                StartCoroutine(ResetGame());
                break;
            case "Die":
                StartCoroutine(Die());
                break;
            //Look();
            default:
                Debug.Log("No such behaviour.");
                break;
        }

        print(selected);

        return;
    }

    public bool isDying = false;

    private void Update()
    {
        if (player.isDead && !isDying)
        {
            ChangeState("Die", null);
        }
    }

    private IEnumerator Die()
    {
        isDying = true;
        behaviourIsActive = true;

        animSpeed = defaultAnimSpeed;
        playerWalk.StopAgentOverride();
        player.ChangeAnimationState(HumanControllerAnimations.Death_Standing_FallForwards01);

        yield return new WaitForSeconds(AnimationUtilities.GetAnimLength(animator));

        float timeOnFloor = Random.Range(AnimationUtilities.GetAnimLength(animator), AnimationUtilities.GetAnimLength(animator) + Random.Range(0, 2));
        yield return new WaitForSeconds(timeOnFloor);

        // Start pulling the player underground
        float duration = 5f; // Duration to pull the player underground
        float time = 0;
        Vector3 startPosition = player.transform.position;
        RaycastHit hit;

        if (Physics.Raycast(player.transform.position, Vector3.up, out hit, Mathf.Infinity))
        {
            float targetY = hit.point.y - 10f; // Target y-position (10 units below the ground)

            while (time < duration)
            {
                time += Time.deltaTime;
                float lerpFactor = time / duration;
                player.transform.position = new Vector3(
                    player.transform.position.x,
                    Mathf.Lerp(startPosition.y, targetY, lerpFactor),
                    player.transform.position.z
                );
                yield return null;
            }
        }

        // Wait for a short random buffer
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));

        // Start the revive process
        StartCoroutine(Revive());

        yield break;
    }

    private IEnumerator Revive()
    {
        behaviourIsActive = true;

        player.ReinitializeAll();

        if (mapObjGen.spawnPointsList.Count > 0)
        {
            int randomIndex = Random.Range(0, mapObjGen.spawnPointsList.Count);
            Vector3 spawnPointPosition = mapObjGen.spawnPointsList[randomIndex].transform.position;

            Collider playerCollider = player.GetComponentInChildren<CapsuleCollider>();
            Vector3 undergroundPosition = new Vector3(spawnPointPosition.x, spawnPointPosition.y - 20f, spawnPointPosition.z);
            player.transform.position = undergroundPosition;

            player.ChangeAnimationState(HumanControllerAnimations.Climb_ClimbUp01);

            RaycastHit hit;
            while (!Physics.Raycast(playerCollider.bounds.min, Vector3.down, out hit, Mathf.Infinity, groundLayer) || hit.distance > playerCollider.bounds.extents.y)
            {
                // Move player up slightly each frame
                Vector3 currentPosition = player.transform.position;
                currentPosition.y += Time.deltaTime * 5f; // Adjust the speed as needed
                player.transform.position = currentPosition;

                // Update the bottom point of the collider for the next iteration
                playerCollider = player.GetComponentInChildren<CapsuleCollider>();

                yield return null;
            }

            // Correct the final position to ensure player is exactly at ground level
            Vector3 finalPosition = player.transform.position;
            finalPosition.y = hit.point.y + playerCollider.bounds.extents.y;
            player.transform.position = finalPosition;

            player.ChangeAnimationState(HumanControllerAnimations.Emotion_Scared_LookAround);

            yield return new WaitForSeconds(AnimationUtilities.GetAnimLength(animator));

            isDying = false;
            playerWalk.CancelAgentOverride();
            behaviourIsActive = false;
        }
        else
        {
            Debug.LogError("Spawn points list is empty.");
        }

        yield break;
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

    public IEnumerator Electrocution()
    {

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

        player.ChangeAnimationState(HumanControllerAnimations.Death_Standing_Electrocution);

        yield return new WaitForSeconds(AnimationUtilities.GetAnimLength(animator));


        if (isSkeleton)
        {
            costumeControl.SwitchSkeleton();
            isSkeleton = false;
        }
        var maxTimeOnFloor = AnimationUtilities.GetAnimLength(animator) + Random.Range(0, 2);

        float timeOnFloor = Random.Range(AnimationUtilities.GetAnimLength(animator), maxTimeOnFloor);
        yield return new WaitForSeconds(timeOnFloor);

        player.ChangeAnimationState(HumanControllerAnimations.OnFront_ToStand_InjuredMax);
        float time = 0;
        float duration = AnimationUtilities.GetAnimLength(animator);

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
       // player.StartCoroutine(player.CheckForRevive());
    }

    public IEnumerator Drown()
    {
        behaviourIsActive = true;

        player.ChangeAnimationState(HumanControllerAnimations.Death_Standing_Insanity);

        yield return StartCoroutine(WaitForAnimationCompletion(player.animator));

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

        if (firstPersonController != null)
        {
            firstPersonController.SetActive(false);
            thirdPersonCamActive = cinematicCam.gameObject.activeInHierarchy;
            firstPersonCamActive = firstPersonController.activeInHierarchy;
        }


    }

    public IEnumerator Look()
    {
        /*cinematicCamActive = !cinematicCamActive;
        firstPersonCamActive = !firstPersonCamActive;

        cinematicCam.gameObject.SetActive(cinematicCamActive);
        firstPersonController.SetActive(firstPersonCamActive);
        */

        behaviourIsActive = true;

        clouds.ManualCloudOverrideStart();
        cinematicCam.ToPanoramaZoom();

        //audioListener.StartCoroutine(audioListener.MoveAudioListener(cinematicCam.transform.gameObject, 1f));

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        behaviourIsActive = false;
        clouds.ManualCloudOverrideStop();

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
    [SerializeField] private float hungerReplenishFactor = 1f;

    float minPulseTrigger = 1;
    float maxPulseTrigger = 1;
    bool pulseActive = false;

    public AuraParticleControl auraParticles;

    bool praying = false;

    public IEnumerator Pray(GameObject hitObject)
    {

        cinematicCam.scrollOverride = false;
        behaviourIsActive = true;
        //pulseActive = true;

        player.ChangeAnimationState(HumanControllerAnimations.Action_StandingToKnees_ToPrayer);

        yield return StartCoroutine(WaitForAnimationCompletion(player.animator));

        cinematicCam.ToPrayerZoom();

        StartCoroutine(GainFaith());
        praying = true;

        StartCoroutine(PrayAnimDirection());
        //auraParticles.StartParticles();
        playerAudioSFX.PlayPrayerAudioLoop();
        //StartCoroutine(PrayerPulseEffect());
        //god.StartGodRay(hitObject.transform, false);


        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        praying = false;

        //auraParticles.StopParticles();

        playerAudioSFX.StopPrayerAudio();
        // pulseActive = false;

        player.ChangeAnimationState(HumanControllerAnimations.Action_KneesToStanding_FinishPray);

        behaviourIsActive = false;
        cinematicCam.ToGameZoom();
        yield break;

    }

    IEnumerator PrayAnimDirection()
    {
        while (praying)
        {
            if (player.faith <= player.maxStat / 2)
            {
                player.ChangeAnimationState(HumanControllerAnimations.Action_OnKnees_PrayToGround);
            }
            else
            {
                player.ChangeAnimationState(HumanControllerAnimations.Action_OnKnees_PrayToSky);
            }

            yield return null;
        }
    }

    [SerializeField] private FluteControl fluteControl;

    private IEnumerator PlayMusic()
    {
        cinematicCam.scrollOverride = false;
        behaviourIsActive = true;

        player.ChangeAnimationState(HumanControllerAnimations.Music_PlayFlute01);

        cinematicCam.ToPlayMusicZoom();
        //cinematicCam.StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));

        fluteControl.EnableFluteControl();


        Debug.Log("Right click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(1));

        behaviourIsActive = false;
        player.ChangeAnimationState(HumanControllerAnimations.Idle_Neanderthal);
        fluteControl.StopAll();
        //cinematicCam.scrollOverride = false;
        cinematicCam.ToGameZoom();

        yield break;
    }

    public IEnumerator Sleep(GameObject hitObject)
    {
        cinematicCam.scrollOverride = true;
        behaviourIsActive = true;

        player.ChangeAnimationState(HumanControllerAnimations.Sleeping_Idle);

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
            player.FaithModify(faithFactor);
            yield return null;
        }

        yield break;
    }


    private IEnumerator LoseFaith()
    {
        while (behaviourIsActive)
        {
            if (player.IsBlessed)
            {
                player.isBlessed = false;
            }
            player.FaithModify(faithFactor);
            yield return null;
        }
    }


    public virtual string GetRandomAnimation(string[] animClips)
    {
        string randomAnimation = animClips[Random.Range(0, animClips.Length)];

        return randomAnimation;
    }

    public RainControl rainControl;

    public bool isDancing = false;

    public IEnumerator Dance()
    {
        behaviourIsActive = true;
        cinematicCam.ToPanoramaZoom();
        rainControl.rainDanceMultiplier = rainControl.maxRainDanceMultiplier;

        isDancing = true;

        while (!Input.GetMouseButtonDown(0))
        {
            string randomDanceAnim = AnimAccess.Instance.GetRandomAnimation("Dance");
            player.ChangeAnimationState(randomDanceAnim);

            float animLength = AnimationUtilities.GetAnimLength(animator);
            yield return new WaitForSeconds(animLength);
        }

        isDancing = false;

        player.ChangeAnimationState(HumanControllerAnimations.Idle_Neanderthal);
        behaviourIsActive = false;
        cinematicCam.panoramaScroll = false;
        cinematicCam.ToGameZoom();
        rainControl.rainDanceMultiplier = rainControl.minRainDanceMultiplier;

        yield break;
    }

    public GameObject frontFacingPivot;
    public GameObject frontFacingAngledPivot;
    public GameObject backFacingPivot;
    public GameObject lookAtTarget;

    public PickUpObject pickUpManager;

    [SerializeField] private float psychBuffer = 60f;

    public IEnumerator PickMushroom(GameObject hitObject)
    {
        behaviourIsActive = true;

        pickUpManager.pickedUpObject = hitObject;

        player.ChangeAnimationState(HumanControllerAnimations.Action_Item_PickUp);
        yield return StartCoroutine(WaitForAnimationCompletion(player.animator));

        cinematicCam.ToActionZoom();
        //cinematicCam.StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        player.ChangeAnimationState(HumanControllerAnimations.Action_Standing_Eat01);
        yield return StartCoroutine(WaitForAnimationCompletion(player.animator));

        int minChance = 0;
        int maxChance = 1;

        float chance = Random.Range(minChance, maxChance);

        if (chance <= maxChance / 2)
        {
            if (!isPsychdelicMode)
            {
                StartCoroutine(PsychedelicModeBuffer());
                StartCoroutine(HealHunger());
                behaviourIsActive = false;
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

            cinematicCam.StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));
            cinematicCam.transform.SetParent(player.transform);
            cinematicCam.ToPsychedelicZoom();

        }

        yield break;
    }

    private IEnumerator HealHunger()
    {
        float time = 0;

        while (time <= 5f)
        {
            player.HealHunger(hungerReplenishFactor);
            time += Time.deltaTime * 2f;

            yield return null;
        }

        yield break;
    }


    public IEnumerator PickupApple()
    {
        behaviourIsActive = true;

        cinematicCam.ToActionZoom();
       // StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));

        player.ChangeAnimationState(HumanControllerAnimations.Action_Item_PickUp);
        yield return StartCoroutine(WaitForAnimationCompletion(player.animator));

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        cinematicCam.ToGameZoom();

        player.ChangeAnimationState(HumanControllerAnimations.Action_Standing_Eat01);
        yield return StartCoroutine(WaitForAnimationCompletion(player.animator));

        behaviourIsActive = false;

        yield break;

    }

    public ParticleSystem vomit;


    public IEnumerator Vomit()
    {
        behaviourIsActive = true;

        //StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));

        vomit.Play();

        player.ChangeAnimationState(HumanControllerAnimations.Idle_Neanderthal_ItchSelf01);
        yield return StartCoroutine(WaitForAnimationCompletion(player.animator));

        vomit.Stop();

        behaviourIsActive = false;

        yield break;

    }

    public IEnumerator Drink()
    {
        behaviourIsActive = true;

        //StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));

        player.ChangeAnimationState(HumanControllerAnimations.Action_OnKnee_Drink01);

        yield return StartCoroutine(WaitForAnimationCompletion(player.animator));

        player.ChangeAnimationState(HumanControllerAnimations.Idle_Neanderthal);

        yield return StartCoroutine(WaitForAnimationCompletion(player.animator));

        if (player.faith < player.maxStat / 2) // Vomit if the water is impure due to low faith
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

    private IEnumerator WaitForAnimationCompletion(Animator animator)
    {
        int layerIndex = 0;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
        int startStateNameHash = stateInfo.fullPathHash;

        // Wait for one frame to ensure the animation has started
        yield return null;

        // Wait until the animation has looped back or the animator has transitioned to a new state
        while (true)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            if (stateInfo.fullPathHash != startStateNameHash || stateInfo.normalizedTime >= 1.0f)
            {
                break;
            }
            yield return null;
        }
    }

    public IEnumerator ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        yield return null;
    }

    public float timeMultiplyFactor = 2;

    public IEnumerator Reflect()
    {

        behaviourIsActive = true;

        cinematicCam.ToActionZoom();

        StartCoroutine(ChangeTimeScale(timeMultiplyFactor));
        StartCoroutine(ReflectAnimIntensity());

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        behaviourIsActive = false;

        cinematicCam.ToGameZoom();

        yield break;
    }

    public IEnumerator ReflectAnimIntensity()
    {
        while (behaviourIsActive)
        {
            if (player.faith < player.maxStat / 2)
            {
                player.ChangeAnimationState(HumanControllerAnimations.Sitting_OnGround_Casually);
            }
            else
            {
                player.ChangeAnimationState(HumanControllerAnimations.Sitting_OnGround_Meditate);
            }

            yield return null;
        }

        timeManager.timeMultiplier = timeManager.defaultTimeMultiplier;

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

    public void ToFrontCam()
    {
        cinematicCam.StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));
    }

    public bool isPsychdelicMode = false;

    [SerializeField] float minAnimationSpeed = 1;
    [SerializeField] float maxAnimationSpeed = 4;

    float interval;

    private float camMoveDuration = 1f;

    public IEnumerator HarvestTree(GameObject hitObject)
    {
        PTGrowing treeDeathManager = hitObject.GetComponentInChildren<PTGrowing>();

        playerAudioSFX.numberOfHits = 0;
        playerAudioSFX.targetTree = hitObject;

        tool.Wield(wieldedStoneAxe, sheathedStoneAxe);

        behaviourIsActive = true;

        float duration = Random.Range(minAnimationSpeed, maxAnimationSpeed);
        cinematicCam.ToActionZoom();

        yield return StartCoroutine(AnimationUtilities.AnimateWithVariableSpeed(player.animator, HumanControllerAnimations.Action_Standing_HarvestTree, minAnimationSpeed, maxAnimationSpeed, duration, () =>
        {
            // Actions to perform after animation completes
            player.ChangeAnimationState(HumanControllerAnimations.Idle_Neanderthal);
            behaviourIsActive = false;
            cinematicCam.ToGameZoom();
            SheatheItem();
        }));

        // Other logic if needed
    }


    [SerializeField] private GameObject campFire;

    public IEnumerator KindleFire(GameObject hitObject)
    {
        tool.Wield(wieldedStoneAxe, sheathedStoneAxe);

        behaviourIsActive = true;

        cinematicCam.ToActionZoom();
        ToFrontCam();

        player.ChangeAnimationState(HumanControllerAnimations.Action_OnKnee_ForageGround);

        yield return new WaitUntil(() => player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime - Mathf.Floor(player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime) > 0.99f);

        player.ChangeAnimationState(HumanControllerAnimations.Idle_Neanderthal);

        behaviourIsActive = false;
        cinematicCam.ToGameZoom();

        GameObject newFire = Instantiate(campFire, hitObject.transform.position, Quaternion.identity);
        hitObject.transform.SetParent(newFire.transform);

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
        dialogue = hitObject.transform.GetComponentInChildren<Dialogue>();
        dialogue.StartDialogue();

        ToFrontCam();

        player.ChangeAnimationState(HumanControllerAnimations.Idle_Neanderthal);

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

    public Dialogue FindDialogueComponent(Transform currentTransform)
    {
        Dialogue dialogue = currentTransform.GetComponent<Dialogue>();
        if (dialogue != null)
        {
            return dialogue;
        }

        foreach (Transform child in currentTransform)
        {
            dialogue = FindDialogueComponent(child);
            if (dialogue != null)
            {
                return dialogue;
            }
        }

        return null;
    }

    public float animLength;

}
