using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTerrain : MonoBehaviour
{
    public CharacterClass player;

    public float Desert = 0f;

    public float Oasis = 10f;

    public float Wet = 21f;

    public List<Material> Materials;

    private float targetState = 0f;
    private float terrainState = 0f;

    [SerializeField] private float duration = 5f;

    Material material;

    // Start is called before the first frame update
    void Start()
    {
        //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;



        material = GetComponent<Renderer>().material;
        Materials.Add(material);


        StartCoroutine(ChanceOfDrought());

        material.SetVector("_VertexTile", new Vector4(0, Oasis, 0, 0));
    }

    private IEnumerator ChanceOfDrought()
    {
        
        yield return new WaitForSeconds(Random.Range(10f, 15f));

        ToState(Desert);

        yield return null;
    }

    void ToState(float state)
    {
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
            terrainState = Mathf.Lerp(terrainState, targetState, time );

            foreach (Material material in Materials)
            {
                material.SetVector("_VertexTile", new Vector4(0, terrainState, 0, 0));
            }

            time += Time.deltaTime / duration;

            yield return null;
        }

        if (time >= 1f)
        {
            yield break;
        }



        yield break;

    }
}
