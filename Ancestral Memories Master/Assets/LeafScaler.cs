using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafScaler : MonoBehaviour
{
    public ProceduralModeling.ProceduralTree proceduralTree;

    public float minGrowthScale = 0f; // The minimum scale for the leaves
    public float maxGrowthScale = 1f; // The maximum scale for the leaves
    public float lerpSpeed = 1f; // The speed at which the leaf growth occurs

    public bool isRendererEnabled = true;

    private void OnEnable()
    {
        proceduralTree = transform.GetComponent<ProceduralModeling.ProceduralTree>();
    }

    public IEnumerator GrowLeaves(float startScale, float targetScale, float lerpSpeed)
    {
        SetLeafScale(startScale);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * lerpSpeed;

            float scale = Mathf.Lerp(startScale, targetScale, t);
            SetLeafScale(scale);
            yield return null;
        }
    }

    public void SetLeafScale(float scale)
    {
        if (scale < 1f)
        {
            DisableRenderers();
        } else
        {
            EnableRenderers();
        }

        foreach (var leaf in proceduralTree.leafList)
        {
            leaf.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    public void EnableRenderers()
    {
        foreach (var leaf in proceduralTree.leafList)
        {
            leaf.GetComponent<MeshRenderer>().enabled = true;
            isRendererEnabled = true;
        }
    }

    public void DisableRenderers()
    {
        foreach (var leaf in proceduralTree.leafList)
        {
            leaf.GetComponent<MeshRenderer>().enabled = false;
            isRendererEnabled = false;
        }
    }
}
