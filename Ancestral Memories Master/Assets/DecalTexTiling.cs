using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class DecalTexTiling : MonoBehaviour
{
    private DecalProjector decalProjector;
    private Vector2 previousSize;

    private void Awake()
    {
        decalProjector = GetComponent<DecalProjector>();
        if (decalProjector == null)
        {
            Debug.LogError("DecalTexTiling requires a DecalProjector component on the same GameObject.");
            return;
        }

        previousSize = new Vector2(decalProjector.size.x, decalProjector.size.y);
    }

    private void Update()
    {
        if (decalProjector.size.x != previousSize.x || decalProjector.size.y != previousSize.y)
        {
            UpdateMaterialTiling();
            previousSize = new Vector2(decalProjector.size.x, decalProjector.size.y);
        }
    }

    private void UpdateMaterialTiling()
    {
        if (decalProjector.material == null) return;

        Vector2 newTiling = new Vector2(decalProjector.size.x / 10.0f, decalProjector.size.y / 10.0f);
        decalProjector.material.SetVector("_TexTiling", new Vector4(newTiling.x, newTiling.y, 0, 0));
    }
}
