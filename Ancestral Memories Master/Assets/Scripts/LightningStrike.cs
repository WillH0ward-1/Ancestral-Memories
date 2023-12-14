using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private int poolSize = 50;
    private Queue<GameObject> lightningPool = new Queue<GameObject>();

    [SerializeField] private DisasterManager disasterManager;
    private LightningSoundEffects lightningSFX;

    [SerializeField] private AreaManager areaManager;
    [SerializeField] private GameObject fire;
    [SerializeField] private FireManager fireManager;
    private string insideCave = "InsideCave";

    private void Awake()
    {
        InitializeLightningPool();
        groundImpacts = transform.GetComponentInChildren<GroundImpacts>();
        disasterManager = transform.GetComponentInChildren<DisasterManager>();
        fireManager = transform.GetComponent<FireManager>();
    }

    private void InitializeLightningPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(lightningPrefab);
            obj.SetActive(false);
            lightningPool.Enqueue(obj);
        }
    }

    private GameObject GetLightningFromPool()
    {
        GameObject obj = lightningPool.Count > 0 ? lightningPool.Dequeue() : Instantiate(lightningPrefab);
        obj.SetActive(true);
        PlayParticleSystems(obj, true);
        return obj;
    }

    private void ReturnLightningToPool(GameObject lightning)
    {
        PlayParticleSystems(lightning, false);
        lightning.SetActive(false);
        lightningPool.Enqueue(lightning);
    }

    private void PlayParticleSystems(GameObject obj, bool play)
    {
        foreach (ParticleSystem ps in obj.GetComponentsInChildren<ParticleSystem>())
        {
            if (play) ps.Play();
            else
            {
                ps.Stop();
                ps.Clear();
            }
        }
    }

    public void StrikeLightning(Transform target)
    {
        if (target == null)
        {
            Debug.LogError("Target is null in StrikeLightning");
            return;
        }

        Debug.Log("Target: " + target.name);

        // Define the delegate outside of the switch for scope reasons
        Action deathAnimationTrigger = null;

        switch (target.tag)
        {
            case "Player":
                deathAnimationTrigger = () =>
                {
                    var playerBehaviours = target.GetComponentInChildren<CharacterBehaviours>();
                    StartCoroutine(playerBehaviours.Electrocution());
                };
                break;

            case "Animal":
                deathAnimationTrigger = () =>
                {
                    var animalAI = target.GetComponentInChildren<AnimalAI>();
                    StartCoroutine(animalAI.Die());
                };
                break;

            case "Human":
                deathAnimationTrigger = () =>
                {
                    var humanAI = target.GetComponentInChildren<HumanAI>();
                    humanAI.ChangeState(HumanAI.AIState.Electrocution);
                };
                break;

            default:
                Debug.LogError("Cannot strike " + target.name + "! Qualify " + target.name + " as a target in the LightningStrike script.");
                return;
        }

        StartCoroutine(Strike(target, deathAnimationTrigger));
    }


    [SerializeField] private GroundImpacts groundImpacts;

    public IEnumerator Strike(Transform target, Action killTarget)
    {
        Debug.Log("Lightning!");

        GameObject lightning = GetLightningFromPool();
        lightningSFX = lightning.GetComponent<LightningSoundEffects>();
        lightningSFX.PlayLightningStrike(target.transform.gameObject);

        lightning.transform.position = new Vector3(target.position.x, target.position.y, target.position.z);

        groundImpacts.ActivateImpactEffects("Lightning", target.transform.position);

        // Trigger the death animation and start the fire
        killTarget?.Invoke();
        fireManager.StartFireAtPosition(target.transform.position);

        // Calculate the total duration of the lightning particle effect
        float totalDuration = GetTotalDurationOfParticleSystems(lightning);

        yield return new WaitForSeconds(totalDuration); // Wait for the lightning effect to complete

        StartCoroutine(Retreat(lightning));
    }

    private float GetTotalDurationOfParticleSystems(GameObject obj)
    {
        float maxDuration = 0f;
        foreach (ParticleSystem ps in obj.GetComponentsInChildren<ParticleSystem>())
        {
            if (ps.main.duration > maxDuration)
            {
                maxDuration = ps.main.duration;
            }
        }
        return maxDuration;
    }



    private IEnumerator Retreat(GameObject lightning)
    {
        Debug.Log("Lightning End!");
        StartCoroutine(disasterManager.DisasterCoolDown());
        ReturnLightningToPool(lightning);  // Returning to pool instead of destroying
        yield break;
    }
}
