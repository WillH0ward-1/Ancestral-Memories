using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviours : MonoBehaviour
{

    public PlayerWalk playerWalk;
    public Player player;

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

    const string PLAYER_STARTPRAYER = "Player_PrayerStart";
    const string PLAYER_PRAYERLOOP = "Player_PrayerLoop";
    const string PLAYER_ENDPRAYER = "Player_PrayerEnd";

    private float animationLength;

    public IEnumerator ChooseBehaviour(RadialButton selected)
    {
        switch (selected.title)
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
            default:
                Debug.Log("No such behaviour.");
                break;
        }

        print(selected.title);

        yield break;
    }

    public void WalkToward(Vector3 hitDestination, RadialMenu radialMenu)
    {
        StartCoroutine(playerWalk.WalkToObject(hitDestination, radialMenu));
    }

    public IEnumerator Pray()
    {

        behaviourIsActive = true;

        ChangeState(PLAYER_STARTPRAYER);

        float animationLength = player.activeAnimator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animationLength);

        ChangeState(PLAYER_PRAYERLOOP);

        Debug.Log("Click to exit this action.");
   
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));       

        player.ChangeAnimationState(PLAYER_ENDPRAYER);

        yield return new WaitForSeconds(animationLength);

        ChangeState(PLAYER_IDLE);

        behaviourIsActive = false;

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
