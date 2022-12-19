using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public string[] danceAnimClips;

    private float animationLength;

    public AreaManager areaManager;

    public ToolControl tool;

    public GameObject wieldedStoneAxe;
    public GameObject sheathedStoneAxe;

    [SerializeField] private ControlAlpha costumeControl;

    [SerializeField] private CheckIfUnderwater waterCheck;

    private PulseEffectControl pulseControl;

    void Start()
    {
        tool.Sheathe(wieldedStoneAxe, sheathedStoneAxe);
        waterCheck = player.GetComponent<CheckIfUnderwater>();

        pulseControl = player.GetComponentInChildren<PulseEffectControl>();
    }

    public void ChooseBehaviour(string selected, GameObject hitObject)
    {
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
                StartCoroutine(Reflect());
                break;
            case "Dance":
                StartCoroutine(Dance(GetRandomAnimation(danceAnimClips)));
                break;
            case "HarvestTree":
                StartCoroutine(HarvestTree());
                break;
            case "Heal":
                break;
            case "Eat":
                StartCoroutine(PickMushroom());
                break;
            case "Drink":
                StartCoroutine(Drink());
                break;
            case "Enter":
                StartCoroutine(areaManager.EnterPortal(hitObject));
                break;
            case "EatApple":
                StartCoroutine(PickupApple());
                break;
            case "KindleFire":
                StartCoroutine(KindleFire(hitObject));
                break;
            case "Talk":
                StartCoroutine(Talk(hitObject));
                break;
            case "PlayMusic":
                StartCoroutine(PlayMusic());
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
        StartCoroutine(playerWalk.WalkToward(hitObject, selected, null, null, rayHit));
    }

    public void SheatheItem()
    {
        tool.Sheathe(wieldedStoneAxe, sheathedStoneAxe);
    }

    [SerializeField] private LightningStrike lightning;

    int randChance;
    int randTarget = 1;
    private bool isSkeleton = false;


    private IEnumerator PlayMusic()
    {
        cinematicCam.scrollOverride = true;
        behaviourIsActive = true;

        player.ChangeAnimationState(PLAYER_PLAYFLUTE);

        cinematicCam.ToSpawnZoom();
        StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        player.ChangeAnimationState(PLAYER_IDLE);

        cinematicCam.scrollOverride = false;
        behaviourIsActive = false;
        cinematicCam.ToGameZoom();

        yield break;
    }

    public IEnumerator Electrocution() { 
    
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

    public bool cinematicCamActive = true;
    public bool firstPersonCamActive = false;

    private void Awake()
    {

        cinematicCam.gameObject.SetActive(true);
        firstPersonController.SetActive(false);

        cinematicCamActive = cinematicCam.gameObject.activeInHierarchy;
        firstPersonCamActive = firstPersonController.activeInHierarchy;

    }

    public IEnumerator Look()
    {
        cinematicCamActive = !cinematicCamActive;
        firstPersonCamActive = !firstPersonCamActive;

        cinematicCam.gameObject.SetActive(cinematicCamActive);
        firstPersonController.SetActive(firstPersonCamActive);

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        cinematicCamActive = !cinematicCamActive;
        firstPersonCamActive = !firstPersonCamActive;

        cinematicCam.gameObject.SetActive(cinematicCamActive);
        firstPersonController.SetActive(firstPersonCamActive);


        yield break;
    }


    private float faithFactor = 0.25f;

    float minPulseTrigger = 1;
    float maxPulseTrigger = 1;

    bool pulseActive = false;

    public IEnumerator Pray(GameObject hitObject)
    {
        cinematicCam.scrollOverride = true;
        behaviourIsActive = true;
        pulseActive = true;

        player.ChangeAnimationState(PLAYER_PRAYER_START);
        yield return new WaitWhile(() => player.activeAnimator.runtimeAnimatorController.name == PLAYER_PRAYER_START);

        player.ChangeAnimationState(PLAYER_PRAYER_LOOP);

        cinematicCam.ToPrayerZoom();
        StartCoroutine(PrayerPulseEffect());
        StartCoroutine(FaithModify());

        //god.StartGodRay(hitObject.transform, false);

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        pulseActive = false;

        player.ChangeAnimationState(PLAYER_PRAYER_END);
        yield return new WaitWhile(() => player.activeAnimator.runtimeAnimatorController.name == PLAYER_PRAYER_END);

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

    private IEnumerator FaithModify()
    {
        while (behaviourIsActive)
        {
            player.GainFaith(faithFactor);
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

        StartCoroutine(FaithModify());

        player.ChangeAnimationState(randomDanceAnim);
        cinematicCam.ToActionZoom();

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        player.ChangeAnimationState(PLAYER_IDLE);

        behaviourIsActive = false;
        cinematicCam.ToGameZoom();
        yield break;
    }

    public GameObject frontFacingPivot;
    public GameObject frontFacingAngledPivot;
    public GameObject backFacingPivot;
    public GameObject lookAtTarget;

    public IEnumerator PickMushroom()
    {
        behaviourIsActive = true;

        player.ChangeAnimationState(PLAYER_PICKUP);
        cinematicCam.ToActionZoom();
        StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        player.ChangeAnimationState(PLAYER_STANDINGEAT);

        behaviourIsActive = false;

        if (DetectIfPsychedelic())
        {
            isPsychdelicMode = true;
            cinematicCam.ToPsychedelicZoom();
        } else if (!DetectIfPsychedelic())
        {

            isPsychdelicMode = false;
            cinematicCam.ToGameZoom();
            
        }

        yield break;

    }


    public IEnumerator PickupApple()
    {
        behaviourIsActive = true;

        player.ChangeAnimationState(PLAYER_PICKUP);
        cinematicCam.ToActionZoom();
        StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        player.ChangeAnimationState(PLAYER_STANDINGEAT);

        behaviourIsActive = false;

        yield break;

    }

    public IEnumerator Vomit()
    {
        cinematicCam.ToGameZoom();
        player.ChangeAnimationState(PLAYER_VOMIT);
        yield return new WaitForSeconds(GetAnimLength());
        behaviourIsActive = false;
        yield break;

    }

    public IEnumerator Drink()
    {
        behaviourIsActive = true;

        player.ChangeAnimationState(PLAYER_TOCROUCH);
        yield return new WaitForSeconds(GetAnimLength());

        cinematicCam.ToActionZoom();
        StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));

        player.ChangeAnimationState(PLAYER_CROUCHDRINK);
        yield return new WaitForSeconds(GetAnimLength());


        player.ChangeAnimationState(PLAYER_CROUCHTOSTAND);
        yield return new WaitForSeconds(GetAnimLength());

        if (player.faith <= player.maxStat / 2)
        {
            StartCoroutine(Vomit());
            yield break;

        }
        else if (player.faith >= player.maxStat / 2)
        {


            behaviourIsActive = false;
            player.ChangeAnimationState(PLAYER_IDLE);
            cinematicCam.ToGameZoom();

            yield break;
        }

    }

    public IEnumerator Reflect()
    {
        behaviourIsActive = true;

        player.ChangeAnimationState(PLAYER_SITONFLOOR);
        yield return new WaitForSeconds(GetAnimLength());

        cinematicCam.ToActionZoom();
        StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, camMoveDuration));

        player.ChangeAnimationState(PLAYER_SITTINGFLOORIDLE);
        yield return new WaitForSeconds(GetAnimLength());

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        player.ChangeAnimationState(PLAYER_STANDUPFROMSIT);

        behaviourIsActive = false;
        player.ChangeAnimationState(PLAYER_IDLE);

        cinematicCam.ToGameZoom();

        yield break;
    }

    public bool isPsychdelicMode = false;

    bool DetectIfPsychedelic()
    {
        int chance = Random.Range(0, 1);

        if (chance <= 0.5f)
        {
            return true;
        } else
        {
            StartCoroutine(Vomit());
            return false;
        }
    }

    [SerializeField] float minAnimationSpeed = 1;
    [SerializeField] float maxAnimationSpeed = 4;
    float interval;

    private float camMoveDuration = 1f;

    public IEnumerator HarvestTree()
    {
        tool.Wield(wieldedStoneAxe, sheathedStoneAxe);

        behaviourIsActive = true;

       
        float time = 0;

        interval = Random.Range(minAnimationSpeed, maxAnimationSpeed);

        while (time <= interval && !Input.GetMouseButtonDown(0))
        {
            interval = Random.Range(minAnimationSpeed, maxAnimationSpeed);

            minAnimationSpeed = player.activeAnimator.speed;

            player.ChangeAnimationState(PLAYER_HARVEST_TREE);
            player.activeAnimator.speed = Mathf.Lerp(minAnimationSpeed, interval, time);

            time += Time.deltaTime / interval;

            yield return null;
        }

        cinematicCam.ToActionZoom();
        StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingAngledPivot, lookAtTarget, camMoveDuration));

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

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
        yield return new WaitWhile(() => player.activeAnimator.runtimeAnimatorController.name == PLAYER_PRAYER_START);

        cinematicCam.ToActionZoom();
        StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingAngledPivot, lookAtTarget, camMoveDuration));

        player.ChangeAnimationState(PLAYER_KINDLEFIRE);
        yield return new WaitForSeconds(GetAnimLength());

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

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
        GameObject DefaultCamPivot = player.transform.Find("DefaultCamPosition").gameObject;

        GameObject lookAtTarget = hitObject;
        Debug.Log("HITOBJECT: " + hitObject);

        dialogueIsActive = true;

        dialogue.StartDialogue(hitObject);

        if (!hitObject.CompareTag("Campfire"))
        {
            player.ChangeAnimationState(PLAYER_IDLE);
            behaviourIsActive = true;
            GameObject NPCPivot = hitObject.transform.Find("NPCpivot").gameObject;
            StartCoroutine(cinematicCam.MoveCamToPosition(NPCPivot, lookAtTarget, 1f));
            player.transform.LookAt(hitObject.transform);
            hitObject.transform.LookAt(player.transform);
            cinematicCam.ToCinematicZoom();
        }

        yield return new WaitUntil(() => dialogue.dialogueIsActive == false);

        if (!hitObject.CompareTag("Campfire"))
        {
            behaviourIsActive = false;
            player.ChangeAnimationState(PLAYER_IDLE);
            lookAtTarget = player.transform.gameObject;
            StartCoroutine(cinematicCam.MoveCamToPosition(DefaultCamPivot, lookAtTarget, 15f));
        }

        dialogueIsActive = false;
        cinematicCam.ToGameZoom();

        yield break;

    }

    public float animLength;

    private float GetAnimLength()
    {
        animLength = player.activeAnimator.GetCurrentAnimatorStateInfo(0).length;
        return animLength;
    }

  


}
