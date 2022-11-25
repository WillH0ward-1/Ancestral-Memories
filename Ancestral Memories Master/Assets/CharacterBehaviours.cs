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

    private string currentState;
    public bool behaviourIsActive = false;

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

    const string PLAYER_HARVEST_TREE = "Player_Harvest_Tree";
    public string[] danceAnimClips = { PLAYER_DANCE_01, PLAYER_DANCE_02, PLAYER_DANCE_03 };

    private float animationLength;

    public AreaManager areaManager;

    public ToolControl wield;

    public GameObject wieldedStoneAxe;
    public GameObject sheathedStoneAxe;

    void Start()
    {
        wield.Sheathe(wieldedStoneAxe, sheathedStoneAxe);
    }

    public void ChooseBehaviour(string selected, GameObject hitObject)
    {
        switch (selected)
        {
            case "Pray":
                StartCoroutine(Pray(hitObject));
                break;
            case "Look":
                //Look();
                break;
            case "Reflect":
                //Reflect();
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
                StartCoroutine(PickApple());
                break;
            //Look();
            default:
                Debug.Log("No such behaviour.");
                break;
        }

        print(selected);

        return;
    }


    public void WalkToward(GameObject hitObject, string selected)
    {
        StartCoroutine(playerWalk.WalkToward(hitObject, selected, null, null));
    }

    public void SheatheItem()
    {
        wield.Sheathe(wieldedStoneAxe, sheathedStoneAxe);
    }

    public IEnumerator Pray(GameObject hitObject)
    {
        behaviourIsActive = true;

        ChangeState(PLAYER_PRAYER_START);

        animationLength = player.activeAnimator.GetCurrentAnimatorStateInfo(0).length;

        ChangeState(PLAYER_PRAYER_LOOP);

        cinematicCam.ToActionZoom();
        StartCoroutine(
                GainFaith());
        //god.StartGodRay(hitObject.transform, false);


        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        ChangeState(PLAYER_PRAYER_END);

        behaviourIsActive = false;

        cinematicCam.ToGameZoom();
        yield break;
    }

    public IEnumerator GainFaith()
    {
        while (behaviourIsActive)
        {
            player.GainFaith(0.25f);
            yield return null;
        }
    }

    public virtual string GetRandomAnimation(string[] animClips)
    {
        string randomAnimation = animClips[Random.Range(0, animClips.Length - 1)];

        animationLength = player.activeAnimator.GetCurrentAnimatorStateInfo(0).length;

        return randomAnimation;
    }


    public IEnumerator Dance(string randomDanceAnim)
    {
        behaviourIsActive = true;

        ChangeState(randomDanceAnim);
        cinematicCam.ToActionZoom();

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        ChangeState(PLAYER_IDLE);

        behaviourIsActive = false;
        cinematicCam.ToGameZoom();
        yield break;
    }

    public GameObject frontFacingPivot;
    public GameObject lookAtTarget;

    public IEnumerator PickMushroom()
    {
        behaviourIsActive = true;

        ChangeState(PLAYER_PICKUP);
        cinematicCam.ToActionZoom();
        StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, false, 15f));

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        ChangeState(PLAYER_STANDINGEAT);

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


    public IEnumerator PickApple()
    {
        behaviourIsActive = true;

        ChangeState(PLAYER_PICKUP);
        cinematicCam.ToActionZoom();
        StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, false, 15f));

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        ChangeState(PLAYER_STANDINGEAT);

        behaviourIsActive = false;

        yield break;

    }

    public IEnumerator Drink()
    {
        behaviourIsActive = true;

        ChangeState(PLAYER_TOCROUCH);

        cinematicCam.ToActionZoom();
        StartCoroutine(cinematicCam.MoveCamToPosition(frontFacingPivot, lookAtTarget, false, 15f));

        ChangeState(PLAYER_CROUCHDRINK);

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        ChangeState(PLAYER_CROUCHTOSTAND);

        cinematicCam.ToGameZoom();

        yield break;
    }

    public bool isPsychdelicMode = false;

    bool DetectIfPsychedelic()
    {
        int chance = Random.Range(0, 100);

        if (chance <= 35)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public IEnumerator HarvestTree()
    {
        wield.Wield(wieldedStoneAxe, sheathedStoneAxe);

        behaviourIsActive = true;

        ChangeState(PLAYER_HARVEST_TREE);
        cinematicCam.ToActionZoom();

        Debug.Log("Click to exit this action.");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        ChangeState(PLAYER_IDLE);

        behaviourIsActive = false;
        cinematicCam.ToGameZoom();

        SheatheItem();

        yield break;
    }

    void ChangeState(string newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;

        player.ChangeAnimationState(newState);
    }

}
