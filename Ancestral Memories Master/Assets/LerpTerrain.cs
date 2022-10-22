using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTerrain : MonoBehaviour
{
    public Material terrainMaterial;

    [SerializeField]
    private CharacterClass player;

    public float Desert = 0f;

    public float Oasis = 2.2f;

    public float Wet = 7.7f;

    public Renderer[] auraRenderers = new Renderer[0];

    private float targetState = 1f;
    private float terrainState = 0;

    public float VertexTileY;

    public float timeMultiplier = 2;

    // Start is called before the first frame update
    void Start()
    {
        //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;

        terrainState = Desert;

        VertexTileY = terrainMaterial.GetFloat("_VertexTile");

        StartCoroutine(ChanceOfDrought());
    }

    private IEnumerator ChanceOfDrought()
    {
        
        yield return new WaitForSeconds(Random.Range(60f, 300f));

        ToState(Oasis);

        yield return null;
    }

    void ToState(float state)
    {
        targetState = state;
        return;
    }

    // Update is called once per frame
    void Update()
    {
        
        terrainState = Mathf.Lerp(terrainState, targetState, timeMultiplier * Time.deltaTime);

        foreach (Renderer renderer in auraRenderers)
        {
            renderer.material.SetFloat("_AuraIntensity", terrainState);
        }

    }
}
