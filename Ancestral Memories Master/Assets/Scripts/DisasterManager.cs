using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisasterManager : MonoBehaviour
{
    [SerializeField] private LightningStrike lightning;
    [SerializeField] private Player player;

    [SerializeField] private CharacterBehaviours behaviours;

    [SerializeField] private Transform target;

    public bool disasterOccuring = false;
    public bool disasterCountdown = false;

    [SerializeField] private AreaManager areaManager;


    /*
    private void Update()
    {
        if (!behaviours.behaviourIsActive && !behaviours.dialogueIsActive && player.isFaithless && !disasterOccuring && !disasterCountdown)
        {
            StartCoroutine(DisasterCountdown());
        } else
        {
            return;
        }
    }
    */
    

    void GetRandomDisaster()
    {
        
    }


    [SerializeField] float minCountdown = 10;
    [SerializeField] float maxCountdown = 30;

    public IEnumerator DisasterCountdown()
    {
  
        disasterCountdown = true;
        yield return new WaitForSeconds(Random.Range(minCountdown, maxCountdown));
        TriggerDisaster();
        yield break;

    }


    void TriggerDisaster()
    {
        if (!behaviours.behaviourIsActive && !areaManager.traversing)
        {
            target = player.transform;
            disasterOccuring = true;
            lightning.StrikeLightning(target);
            return;

        }
        else if(behaviours.behaviourIsActive || areaManager.traversing)
        {
            return;
        }
    }

    [SerializeField] float minDisasterRetrigger = 10;
    [SerializeField] float maxDisasterRetrigger = 30;

    public IEnumerator DisasterCoolDown()
    {
        disasterCountdown = false;
        disasterOccuring = false;

        if (!disasterCountdown)
        {
            yield return new WaitForSeconds(Random.Range(minDisasterRetrigger, maxDisasterRetrigger));

            if (player.isFaithless && !disasterOccuring)
            {
                TriggerDisaster();
                yield break;

            }
            else if (!player.isFaithless)
            {
                yield break;
            }
        }
    }


    void EndDisaster()
    {

    }

        void EarthQuake()
        {
            //earthQuake.start = true;
        }

}
