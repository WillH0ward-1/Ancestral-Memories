using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisasterManager : MonoBehaviour
{
    [SerializeField] private LightningStrike lightning;
    [SerializeField] private Player player;

    private void Update()
    {
        if (player.isFaithless && !disasterOccuring && !disasterCountdown)
        {
            StartCoroutine(DisasterCountdown());
            return;
        } else
        {
            return;
        }
    }

    void GetRandomDisaster()
    {
        
    }

    public bool disasterOccuring = false;
    public bool disasterCountdown = false;


    public IEnumerator DisasterCountdown()
    {
        disasterCountdown = true;
        yield return new WaitForSeconds(Random.Range(3, 5));
        TriggerDisaster();
        yield break;

    }


    void TriggerDisaster()
    {
        disasterOccuring = true;
        disasterCountdown = false;
        lightning.StrikeLightning(player.transform);
        return;
    }

    public IEnumerator DisasterCoolDown()
    {
        disasterOccuring = false;
        yield return new WaitForSeconds(Random.Range(3, 5));

        if (player.isFaithless && !disasterOccuring)
        {
            TriggerDisaster();
            yield break;

        } else if (!player.isFaithless)
        {
            yield break;
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
