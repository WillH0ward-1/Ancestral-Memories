using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class DecalProjectorManager : MonoBehaviour
{
    public DecalProjector decalProjector;
    private Vector2 previousSize;

    private void OnEnable()
    {
        // Get the DecalProjector component
        decalProjector = GetComponent<DecalProjector>();
        if (decalProjector == null)
        {
            Debug.LogError("DecalProjectorManager requires a DecalProjector component on the same GameObject.");
            return;
        }

        // Initialize previousSize with the current size of the decal projector
        previousSize = new Vector2(decalProjector.size.x, decalProjector.size.y);
        UpdateMaterialTiling();
    }

    private void Update()
    {
        // Check if the size of the decal projector has changed
        if (decalProjector != null && (decalProjector.size.x != previousSize.x || decalProjector.size.y != previousSize.y))
        {
            UpdateMaterialTiling();
            previousSize = new Vector2(decalProjector.size.x, decalProjector.size.y);
        }
    }

    private void UpdateMaterialTiling()
    {
        if (decalProjector.material == null) return;

        // Calculate the new tiling values based on the decal projector's size
        Vector2 newTiling = new Vector2(decalProjector.size.x / 10.0f, decalProjector.size.y / 10.0f);

        // Apply the new tiling to the material
        decalProjector.material.SetVector("_TexTiling", new Vector4(newTiling.x, newTiling.y, 0, 0));
    }
}
