using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : Human
{


    [SerializeField] private float currentHealth;

    [SerializeField] private bool isReviving = false;

    [SerializeField] private AnimationManager animator;

    [SerializeField] private AnimReferences animBank;

    private void Awake()
    {
        currentHealth = maxVal;
    }

    public void SetHealth(int value)
    {
        currentHealth = value;

        if (currentHealth >= maxVal)
        {
            currentHealth = maxVal;
        }
    }

    public void TakeDamage(float damage)
    {

        currentHealth -= damage;
        healthBar.UpdateHealthBar((float)currentHealth / (float)maxVal);

        if (currentHealth <= minVal)
        {
            Kill();
        }
    }

    public bool IsDead()
    {
        if (hasDied == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsReviving()
    {
       if (isReviving == true)
        {
            return true;
        } else
        {
            return false;
        }
    }


    public void Kill()
    {
        currentHealth = minVal;
        hasDied = true;
        CheckForRevival();
        StartCoroutine(CheckForRevival());
    }

    public IEnumerator CheckForRevival()
    {
        yield return new WaitForSeconds(animator.GetAnimationLength());

        if (faith.CheckFaith()) // In order to revive, currentFaith needs to be > x. 
        {
            isReviving = true;
            StartCoroutine(Revive());
        }
        else if (!faith.CheckFaith())
        {
            StartCoroutine(RespawnBuffer());
        }
    }

    public IEnumerator Revive()

    {   // REVIVE PLAYER - Complete Reset.

        hasDied = false;

        SetHealth(maxVal);
        hunger.SetHunger(maxVal);
        faith.SetFaith(maxVal);

        animator.ChangeAnimationState(animBank.PLAYER_REVIVING);

 
        yield return new WaitForSeconds(animator.GetAnimationLength());

        //StartCoroutine(god.TriggerGodRay());

        //godRay.godRay = false;
        isReviving = false;

        animator.ChangeAnimationState(animBank.PLAYER_IDLE);
    }

    public IEnumerator RespawnBuffer()
    {
        yield return new WaitForSeconds(animator.GetAnimationLength()); // Wait for this many seconds before respawning. This may be an audio cue in future.
        ResetGame();
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        print("Game Restarted.");
    }
}
