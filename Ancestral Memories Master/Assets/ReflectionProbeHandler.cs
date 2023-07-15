using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class ReflectionProbeHandler : MonoBehaviour
{
    // Reference to the Reflection Probe in the scene
    private ReflectionProbe probe;

    // Reference to the Renderer of the object that's using the shader
    private Renderer rend;

    private void OnEnable()
    {
        probe = GetComponent<ReflectionProbe>();
        rend = GetComponent<Renderer>();

        if (probe == null)
        {
            Debug.LogError("No Reflection Probe component attached to the object.");
            return;
        }

        if (rend == null)
        {
            Debug.LogError("No Renderer attached to the object.");
            return;
        }

        AssignCubemapToMaterial();
    }

    public void AssignCubemapToMaterial()
    {
        if (probe.mode != ReflectionProbeMode.Realtime)
        {
            Debug.LogError("Reflection Probe is not set to Realtime. Please change it to Realtime in the Inspector.");
            return;
        }

        // Ensure the reflection probe has finished rendering before assigning the cubemap
        probe.RenderProbe();
        rend.sharedMaterial.SetTexture("_Cube", probe.bakedTexture);
    }
}
