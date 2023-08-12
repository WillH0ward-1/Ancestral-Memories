using System.Collections;
using System.Collections.Generic;
using Unity.DemoTeam.Hair;
using UnityEngine;

public class HairControl : MonoBehaviour
{
    private List<HairInstance> hairInstances;

    private void Awake()
    {
        SetHairInstances();
    }

    void SetHairInstances()
    {
        foreach (HairInstance hairInstance in transform.GetComponentsInChildren<HairInstance>())
        {
            hairInstances.Add(hairInstance);
        }
    }

    private void SetHairStyle()
    {

    }

    private void SetHairColour()
    {

    }

    private IEnumerator GrowHair()
    {
        yield break;
    }

    private IEnumerator ShrinkHair()
    {
        yield break;
    }

    private void LerpHairColour()
    {

    }

}
