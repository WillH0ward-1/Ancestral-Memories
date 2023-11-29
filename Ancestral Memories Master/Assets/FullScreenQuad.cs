using UnityEngine;

[ExecuteAlways]
public class FullScreenQuad : MonoBehaviour
{
    private Vector2 screenResolution;
    private Camera mainCamera;

    void OnEnable()
    {
        mainCamera = Camera.main; // Cache the main camera reference
        MatchPlaneToScreenSize();
    }

    void Start()
    {
        screenResolution = new Vector2(Screen.width, Screen.height);
    }

    void Update()
    {
        if (screenResolution.x != Screen.width || screenResolution.y != Screen.height || mainCamera.rect != new Rect(0, 0, 1, 1))
        {
            MatchPlaneToScreenSize();
            screenResolution = new Vector2(Screen.width, Screen.height);
        }
    }

    private void MatchPlaneToScreenSize()
    {
        if (mainCamera == null) return; // Ensure there is a camera reference

        if (mainCamera.orthographic)
        {
            float size = mainCamera.orthographicSize * 2;
            float width = size * mainCamera.aspect;
            transform.localScale = new Vector3(width, 1, size);
        }
        else
        {
            float planeToCameraDistance = Vector3.Distance(transform.position, mainCamera.transform.position);
            float planeHeightScale = (2.0f * Mathf.Tan(0.5f * mainCamera.fieldOfView * Mathf.Deg2Rad) * planeToCameraDistance) / 10.0f;
            float planeWidthScale = planeHeightScale * mainCamera.aspect;
            transform.localScale = new Vector3(planeWidthScale, 1, planeHeightScale);
        }
    }
}
