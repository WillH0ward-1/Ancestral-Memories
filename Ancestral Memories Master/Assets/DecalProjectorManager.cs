using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class DecalProjectorManager : MonoBehaviour
{
    public DecalProjector decalProjector;
    [SerializeField] private LayerMask invalidBuildLayers;

    // Gizmo drawing variables
    private bool shouldDrawGizmos = false;
    private Vector3 gizmoSphereCenter;
    private float gizmoSphereRadius;
    private float height = 200f;

    private void OnEnable()
    {
        decalProjector = GetComponent<DecalProjector>();
        if (decalProjector == null)
        {
            Debug.LogError("DecalProjectorManager requires a DecalProjector component on the same GameObject.");
        }
    }

    public void SetDecalSize(Vector3 newSize)
    {
        if (decalProjector != null)
        {
            decalProjector.size = newSize;
            decalProjector.pivot = Vector3.zero;
        }
    }

    public void SetIsValidBuildArea(bool isValid)
    {
        if (decalProjector != null && decalProjector.material != null)
        {
            decalProjector.material.SetInt("_isValidBuildArea", isValid ? 1 : 0);
        }
    }

    public void SetIsBuilt(bool isBuilt)
    {
        if (decalProjector != null && decalProjector.material != null)
        {
            decalProjector.material.SetInt("_isBuilt", isBuilt ? 1 : 0);
        }
    }

    public bool CheckForCollisions(float sphereRadius)
    {
        Vector3 sphereCenter = transform.position;
        gizmoSphereCenter = sphereCenter;
        gizmoSphereRadius = sphereRadius;
        shouldDrawGizmos = true;

        bool isCollisionDetected = Physics.CheckSphere(sphereCenter, sphereRadius, invalidBuildLayers.value);

        Vector3 highPosition = new Vector3(sphereCenter.x, sphereCenter.y + height, sphereCenter.z);
        LayerMask groundAndWaterLayers = LayerMask.GetMask("Ground", "Water");

        if (Physics.SphereCast(highPosition, sphereRadius, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, groundAndWaterLayers))
        {
            if (hitInfo.collider.CompareTag("Water"))
            {
                isCollisionDetected = true;
            }
        }

        return isCollisionDetected;
    }

    void OnDrawGizmos()
    {
        if (shouldDrawGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(gizmoSphereCenter, gizmoSphereRadius);

            Vector3 highPosition = new Vector3(gizmoSphereCenter.x, gizmoSphereCenter.y + height, gizmoSphereCenter.z);
            Gizmos.DrawLine(highPosition, gizmoSphereCenter);
            Gizmos.DrawWireSphere(highPosition, gizmoSphereRadius);
        }
    }
}
