using UnityEngine;

public class ShaderLightColor : MonoBehaviour
{
    public Light sceneLight; // Set this to your directional light in the inspector
    private Renderer rend;
    private MaterialPropertyBlock propBlock;

    private void Awake() // Using Awake instead of Start to ensure it is called even if the script is not enabled
    {
        rend = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    public void UpdateLightColor(Color lightColor)
    {
        propBlock.SetColor("_LightColor", lightColor);
        rend.SetPropertyBlock(propBlock);
    }
}
