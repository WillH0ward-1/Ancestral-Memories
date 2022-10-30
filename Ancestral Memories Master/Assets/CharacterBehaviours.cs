using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviours : MonoBehaviour
{

    public PlayerWalk playerWalk;
    public CharacterClass player;

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

    private void Awake()
    {
        
    }

    public IEnumerator PrayerAnimation()
    {
        behaviourIsActive = true;
        playerWalk.StopAgentOverride();
        player.ChangeAnimationState(PLAYER_STARTPRAYER);
        float animationLength = player.activeAnimator.GetCurrentAnimatorStateInfo(1).length;
        yield return new WaitForSeconds(animationLength);

        player.ChangeAnimationState(PLAYER_PRAYERLOOP);

        StartCoroutine(WaitForActionBreak());

        if (!behaviourIsActive)
        {
            player.ChangeAnimationState(PLAYER_ENDPRAYER);

            yield return new WaitForSeconds(animationLength);

            player.ChangeAnimationState(PLAYER_IDLE);

            yield return null;
        }

        yield return null;
    }

    private IEnumerator WaitForActionBreak()
    {
        Debug.Log("Click to exit this action.");

        if (Input.GetKeyDown(0))
        {
            behaviourIsActive = false;
        }

        yield return null;
    }


}
