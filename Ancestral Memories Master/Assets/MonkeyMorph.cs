using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyMorph : MonoBehaviour
{

    public GameObject humanState;
    public GameObject monkeyState;

    public CharacterClass player;

    public SkinnedMeshRenderer meshRenderer;

   [SerializeField] float currentVal = 0;
   [SerializeField] float endVal = 0;

    public float timeElapsed;
    public float lerpDuration = 5;

    public bool playerIsMonkey = false;

    private int blendShapeIndex = 0;

    // Start is called before the first frame update

    private void Awake()
    {
        playerIsMonkey = true;
        Renderers();
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            SwitchCostume();
        }
    }

    void SwitchCostume()
    {
        Morph();
    }

    public void Morph()
    {
        StartCoroutine(StartMorph());
    }

    IEnumerator StartMorph()
    {
        if (playerIsMonkey == true)
        {
            currentVal = 100;
            endVal = 0;
        }

        else if (playerIsMonkey == false)
        {
            currentVal = 0;
            endVal = 100;
        }

        float timeElapsed = 0;

        while (timeElapsed <= lerpDuration)
        {
            var lerpVal = Mathf.Lerp(currentVal, endVal, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;

            meshRenderer.SetBlendShapeWeight(blendShapeIndex, lerpVal);

            if (timeElapsed >= lerpDuration)
            {
                Renderers();
            }

            yield return null;
        }

    }

    void Renderers()
    {
        if (playerIsMonkey == false)
        {
            EnableRenderers(humanState);
            DisableRenderers(monkeyState);
            playerIsMonkey = false;
        }

        else if (playerIsMonkey == true)
        {
            EnableRenderers(monkeyState);
            DisableRenderers(humanState);
            playerIsMonkey = true;
        }
    }

    public void DisableRenderers(GameObject state)
    {
        SkinnedMeshRenderer[] meshRenderers = state.transform.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = false;
        }

    }

    public void EnableRenderers(GameObject state)
    {
        SkinnedMeshRenderer[] meshRenderers = state.transform.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = true;
        }
    }
}

