using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : Human
{

    [SerializeField] private HealthBar health;

    [SerializeField] private float currentHealth;

    [SerializeField] private int minVal = 0;
    [SerializeField] private int maxVal = 100;

    [SerializeField] private bool hasDied = false;

    [SerializeField] private bool isReviving = false;

    [SerializeField] private Faith faith;

    [SerializeField] private AnimationManager animator;

    [SerializeField] private Hunger hunger;

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
        health.UpdateHealth((float)currentHealth / (float)maxVal);

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
        currentHealth = maxVal;
        hunger.currentHunger = hunger.maxHunger;
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
