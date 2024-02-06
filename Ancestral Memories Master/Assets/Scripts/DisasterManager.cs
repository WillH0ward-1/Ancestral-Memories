using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisasterManager : MonoBehaviour
{
    [SerializeField] private LightningStrike lightning;
    public Player player;
    [SerializeField] private Transform target;
    public bool disasterOccuring = false;
    public bool disasterCountdown = false;
    [SerializeField] private AreaManager areaManager;
    public List<GameObject> humanTargets;
    public List<GameObject> animalTargets;
    public List<GameObject> treeTargets;

    private struct StruckTarget
    {
        public Transform TargetTransform;
        public float EligibleTime;
    }
    private List<StruckTarget> struckTargets = new List<StruckTarget>();
    [SerializeField] private float targetCooldownDuration = 30f; // Time in seconds before a target can be struck again

    private Transform lastStruckTarget = null; // Field to store the last target

    private void Awake()
    {
        fireManager = GetComponentInChildren<FireManager>();
    }

    public void InitDisasters()
    {
        StartCoroutine(DisasterCountdown());
    }

    private Transform GetRandomTarget()
    {
        List<Transform> allTargets = new List<Transform>();
        allTargets.AddRange(humanTargets.ConvertAll(h => h.transform));
        //allTargets.AddRange(animalTargets.ConvertAll(a => a.transform));
        allTargets = allTargets.OrderBy(x => Random.value).ToList();

        float currentTime = Time.time;
        allTargets.RemoveAll(target =>
            target == null || !IsValidTarget(target) ||
            struckTargets.Any(st => st.TargetTransform == target && st.EligibleTime > currentTime) ||
            target == lastStruckTarget ||
            fireManager.IsOnFire(target.gameObject)
        );

        if (allTargets.Count > 0)
        {
            return allTargets[Random.Range(0, allTargets.Count)];
        }

        Debug.Log("No valid target found for lightning strike.");
        return null;
    }


    private bool IsValidTarget(Transform target)
    {
        switch (target.tag)
        {
            case "Player":
                var playerBehaviour = target.GetComponentInChildren<CharacterBehaviours>();
                return (playerBehaviour.behaviourIsActive && !areaManager.traversing);

            case "Animal":
                var animalAIstats = target.GetComponentInChildren<AICharacterStats>();
                return !animalAIstats.isDead;

            case "Human":
                var humanAIstats = target.GetComponentInChildren<AICharacterStats>();
                var humanAI = target.GetComponentInChildren<HumanAI>();
                return !humanAI.isElectrocuted && !humanAIstats.isDead;

            default:
                return false;
        }
    }

    [SerializeField] float minCountdown = 10;
    [SerializeField] float maxCountdown = 30;
    private FireManager fireManager;

    public IEnumerator DisasterCountdown()
    {
        disasterCountdown = true;
        yield return new WaitForSeconds(Random.Range(minCountdown, maxCountdown));
        TriggerDisaster();
    }

    void TriggerDisaster()
    {
        if (player.faith <= player.minStat / 3)
        {
            target = GetRandomTarget();
            disasterOccuring = true;

            if (target != null)
            {
                lightning.StrikeLightning(target);

                var existingTarget = struckTargets.FirstOrDefault(st => st.TargetTransform == target);
                if (existingTarget.TargetTransform != null)
                {
                    existingTarget.EligibleTime = Time.time + targetCooldownDuration;
                }
                else
                {
                    struckTargets.Add(new StruckTarget { TargetTransform = target, EligibleTime = Time.time + targetCooldownDuration });
                }

                lastStruckTarget = target; // Update the last struck target

                struckTargets.RemoveAll(st => st.EligibleTime <= Time.time);

            }
        }

        StartCoroutine(DisasterCoolDown());
    }
    [SerializeField] float minDisasterRetrigger = 10;
    [SerializeField] float maxDisasterRetrigger = 30;

    public IEnumerator DisasterCoolDown()
    {
        disasterCountdown = false;
        disasterOccuring = false;
        yield return new WaitForSeconds(Random.Range(minDisasterRetrigger, maxDisasterRetrigger));

        StartCoroutine(DisasterCountdown());
    }
}
