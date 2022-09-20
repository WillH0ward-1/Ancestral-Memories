using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{

    public HealthBar playerHealth;

    public float currentHealth;

    public int minHealth = 0;
    public int maxHealth = 100;

    public bool playerHasDied = false;

    public bool playerIsReviving = false;

    private Faith faith;

    private AnimationManager animator;

    private Hunger hunger;

    public bool respawn;

    const string PLAYER_IDLE = "Player_idle";
    const string PLAYER_REVIVING = "Player_revived";

    private void Awake()
    {
        currentHealth = maxHealth;
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

    public IEnumerator CheckForRevival()
    {
        float animationLength = animator.activeAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        if (faith.currentFaith < 50) // In order to revive, currentFaith needs to be > x. 
        {
            playerIsReviving = true;
            StartCoroutine(RevivePlayer());
        }
        else
        {
            StartCoroutine(RespawnBuffer());
        }
    }

    public IEnumerator RevivePlayer()

    {   // REVIVE PLAYER - Complete Reset.

        playerHasDied = false;
        currentHealth = maxHealth;
        hunger.currentHunger = hunger.maxHunger;
        faith.currentFaith = faith.maxFaith;

        animator.ChangeAnimationState(PLAYER_REVIVING);

        float animationLength = animator.activeAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        //StartCoroutine(god.TriggerGodRay());

        //godRay.godRay = false;
        playerIsReviving = false;

        animator.ChangeAnimationState(PLAYER_IDLE);
    }

    public IEnumerator RespawnBuffer()
    {
        float animationLength = animator.activeAnimator.GetCurrentAnimatorStateInfo(1).length;
        yield return new WaitForSeconds(animationLength); // Wait for this many seconds before respawning. This may be an audio cue in future.
        respawn = true;
        ResetGame();
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        print("Game Restarted.");
    }
}
