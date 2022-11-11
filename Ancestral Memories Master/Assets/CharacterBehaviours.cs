using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviours : MonoBehaviour
{

    public PlayerWalk playerWalk;
    public Player player;

    public CamControl cinematicCam;

    private string currentState;
    public bool behaviourIsActive = false;

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


    private float animationLength;

    public void ChooseBehaviour(string selected)
    {
        switch (selected)
        {
            case "Pray":
                StartCoroutine(Pray());
                break;
            case "Look":
                //Look();
                break;
            case "Reflect":
                //Reflect();
                break;
            case "Dance":
                break;
            case "Harvest":
                //Harvest();
                break;
            case "Heal":
                break;
            case "Eat":
                break;
            //Look();
            default:
                Debug.Log("No such behaviour.");
                break;
        }

        print(selected);

        return;
    }

    public void WalkToward(Vector3 hitDestination, string selected)
    {
        StartCoroutine(playerWalk.WalkToObject(hitDestination, selected));
    }

    public IEnumerator Pray()
    {
        behaviourIsActive = true;

        ChangeState(PLAYER_PRAYER_START);

        float animationLength = player.activeAnimator.GetCurrentAnimatorStateInfo(0).length;

        ChangeState(PLAYER_PRAYER_LOOP);

        cinematicCam.ToActionZoom();

        Debug.Log("Click to exit this action.");
   
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        ChangeState(PLAYER_PRAYER_END);

        behaviourIsActive = false;

        cinematicCam.ToGameZoom();

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

    public IEnumerator HarvestAnimation()
    {
        yield return null;
    }
}
