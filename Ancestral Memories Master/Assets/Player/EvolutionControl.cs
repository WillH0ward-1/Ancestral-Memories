using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionControl : MonoBehaviour
{
    public GameObject playerRoot;

    public GameObject humanState;
    public GameObject monkeyState;

    public CharacterClass player;

    public bool playerIsMonkey;

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            SwitchCostume();
        }
    }

    void SwitchCostume()
    {
        Debug.Log(player.playerIsMonkey);

        if (!player.playerIsMonkey)
        {
            SwitchToHuman();
        }
        else if (player.playerIsMonkey == true)
        {
            SwitchToMonkey();
        }
    }

    void SwitchToHuman()
    {
        playerIsMonkey = false;

        EnableRenderers(humanState);
        DisableRenderers(monkeyState);
    }

    void SwitchToMonkey()
    {
        playerIsMonkey = true;

        EnableRenderers(monkeyState);
        DisableRenderers(humanState);
    }

    public Component[] meshRenderers;

    void DisableRenderers(GameObject state)
    {
        for (int i = 0; i < state.transform.childCount; i++)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            meshRenderer.enabled = false;
        }
    }

    void EnableRenderers(GameObject state)
    {
        //state = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < state.transform.childCount; i++)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            meshRenderer.enabled = false;
        }
    }
}