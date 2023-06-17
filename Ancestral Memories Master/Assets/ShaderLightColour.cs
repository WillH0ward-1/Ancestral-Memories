using UnityEngine;

public class ShaderLightColor : MonoBehaviour
{
    public Light sceneLight; // Set this to your directional light in the inspector
    private Renderer rend;
    private MaterialPropertyBlock propBlock;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        propBlock.SetColor("_LightColor", sceneLight.color);
        propBlock.SetVector("_LightDir", -sceneLight.transform.forward);
        rend.SetPropertyBlock(propBlock);
    }
}