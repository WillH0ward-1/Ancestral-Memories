using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTerrain : MonoBehaviour
{
    public Material terrainMaterial;

    [SerializeField]
    private CharacterClass player;

    public int Desert = 0;

    public float Oasis = 2.2f;

    public float Wet = 7.7f;

    public Renderer[] auraRenderers = new Renderer[0];

    private float targetAuraVal = 1f;
    private float terrainState = 0;

    public float VertexTileY;


    // Start is called before the first frame update
    void Start()
    {
        //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;

        terrainState = Oasis;

        VertexTileY = terrainMaterial.GetFloat("_VertexTile");

        StartCoroutine(ChanceOfDrought());
    }

    private IEnumerator ChanceOfDrought()
    {
        yield return new WaitForSeconds(Random.Range(60f, 300f));

        RandomDrought();

        yield return null;
    }

    void RandomDrought()
    {
        terrainState = Desert;

    }

    void GetTerrainType()
    {
        targetAuraVal = Oasis;

    }

    // Update is called once per frame
    void Update()
    {
        terrainState = Mathf.Lerp(terrainState, targetAuraVal, 2f * Time.deltaTime);

        foreach (Renderer renderer in auraRenderers)
        {
            renderer.material.SetFloat("_AuraIntensity", terrainState);
        }

    }
}
