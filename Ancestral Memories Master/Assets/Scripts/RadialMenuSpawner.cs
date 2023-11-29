using UnityEngine;

public class RadialMenuSpawner : MonoBehaviour
{
    public static RadialMenuSpawner menuInstance;
    public RadialMenu menuPrefab;

    private void Awake()
    {
        menuInstance = this;
    }

    public void SpawnMenu(Interactable obj, GameObject lastHit, RaycastHit hit)
    {
        RadialMenu newMenu = Instantiate(menuPrefab);
        newMenu.transform.SetParent(transform, false); // Set worldPositionStays to false
        newMenu.transform.position = GetMenuPositionWithinScreenBounds(Input.mousePosition);

        newMenu.SpawnButtons(obj, lastHit, hit);
    }

    private Vector3 GetMenuPositionWithinScreenBounds(Vector3 mousePosition)
    {
        // Get viewport position of mouse (values between 0 and 1)
        Vector3 viewportPosition = Camera.main.ScreenToViewportPoint(mousePosition);

        // Clamp values to ensure menu stays within the screen
        viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.1f, 0.9f);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.1f, 0.9f);

        // Convert back to screen position
        return Camera.main.ViewportToScreenPoint(viewportPosition);
    }
}
