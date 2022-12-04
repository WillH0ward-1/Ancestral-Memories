using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LerpTerrain : MonoBehaviour
{
    public CharacterClass player;

    public float Desert = 0f;

    public float Oasis = 10f;

    public float Wet = 21f;

    [SerializeField] List<Renderer> rendererList = new List<Renderer>();

    private float targetState = 0f;
    private float terrainState = 0f;

    [SerializeField] private float duration = 15f;

    private Material material;

    private RainControl weather;

    // Start is called before the first frame update
    void Start()
    {
        //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;
        Vector4 initState = new Vector4(0, Oasis, 0, 0);

        GetRenderers();

        StartCoroutine(ChanceOfDrought());

        material.SetVector("_VertexTile", initState);
    }

    void GetRenderers()
    {
        Renderer[] objectRenderers = transform.GetComponentsInChildren<Renderer>();
        rendererList = objectRenderers.ToList();
    }


    public IEnumerator GrowGrass(float duration)
    {

        ToState(Wet, duration);
        yield break;
        
    }

    public IEnumerator DryGrass(float duration)
    {

        ToState(Oasis, duration);
        yield break;

    }

    private IEnumerator ChanceOfDrought()
    {
        
        yield return new WaitForSeconds(Random.Range(10f, 15f));

        if (!weather.isRaining)
        {
            ToState(Desert, 15f);
        }
        else if (weather.isRaining)
        {
            StartCoroutine(ChanceOfDrought());
            yield return null;
        }

        yield return null;
    }

    void ToState(float state, float duration)
    {

        //StopCoroutine(LerpTerrainTexture(0, 0));

        targetState = state;
        StartCoroutine(LerpTerrainTexture(duration, targetState));
        return;
    }

    // Update is called once per frame

    float time;

    private IEnumerator LerpTerrainTexture(float duration, float targetState)
    {
        float time = 0;

        while (time <= 1f)
        {

            foreach (Renderer r in rendererList)
            {
                float state = r.sharedMaterial.GetVector("_VertexTile").y;

                time += Time.deltaTime / duration;
                state = Mathf.Lerp(state, targetState, time / duration);
   
                r.sharedMaterial.SetVector("_VertexTile", new Vector4(0, state, 0, 0));
                yield return null;
            }


            yield return null;
        }

        if (time >= 1f)
        {
            yield break;
        }



        yield break;

    }
}
