using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LerpTerrain : MonoBehaviour
{
    public CharacterClass player;

    public float Desert = 0f;

    public float Oasis = 12f;

    public float Wet = 22f;

    [SerializeField] List<Renderer> rendererList = new List<Renderer>();

    private float targetState = 0f;
    private float terrainState = 0f;

    [SerializeField] private float duration = 15f;

    private Material material;

    private RainControl weather;

    [SerializeField] private float currentState;

    // Start is called before the first frame update

    void Start()
    {
        //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;
        //Vector4 initState = new Vector4(0, Oasis, 0, 0);

        GetRenderers();
        ToState(Oasis, 0.1f);
        //StartCoroutine(SetTerrainState(Desert));
    }

 
    IEnumerator SetTerrainState(float newState)
    {
        foreach (Renderer r in rendererList)
        {
            newState = r.sharedMaterial.GetVector("_VertexTile").y;

            r.sharedMaterial.SetVector("_VertexTile", new Vector4(0, newState, 0, 0));
            yield return null;
        }

        yield break;
    }

    void GetRenderers()
    {
        Renderer[] objectRenderers = transform.GetComponentsInChildren<Renderer>();
        rendererList = objectRenderers.ToList();
    }

    public IEnumerator ToOasis(float duration)
    {
        ToState(Oasis, duration);
        yield break;

    }

    public IEnumerator ToWetOasis(float duration)
    {
        ToState(Wet, duration);
        yield break;
    }

    float minDroughtWaitTime = 10;
    float maxDroughtWaitTime = 15;

    public IEnumerator ToDesert(float duration)
    {
        ToState(Desert, duration);
        yield break;

    }

    void ToState(float newState, float duration)
    {
        StopAllCoroutines();

        if (newState != currentState)
        {
            currentState = newState;
            StartCoroutine(LerpTerrainTexture(newState, duration));
            return;
        }

        return;
        
    }

    // Update is called once per frame

    float time;
    private bool isLerping;

    float state;

    private IEnumerator LerpTerrainTexture(float targetState, float duration)
    {

        float time = 0;

        while (time <= 1f)
        {

            isLerping = true;

            if (isLerping == false)
            {
                yield break;
            }

            foreach (Renderer r in rendererList)
            {
                state = r.sharedMaterial.GetVector("_VertexTile").y;
                float stateval = state;
                state = Mathf.Lerp(stateval, targetState, time);
                r.sharedMaterial.SetVector("_VertexTile", new Vector4(0, state, 0, 0));
                time += Time.deltaTime / duration;

                yield return null;
            }

            yield return null;

        }


        if (time >= 1f)
        {
            isLerping = false;
            yield break;
        }

        yield break;

    }
}
