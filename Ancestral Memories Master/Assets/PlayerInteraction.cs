using UnityEngine;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{

    public Camera cam;
    private readonly string playerTag = "Player";

    void Update()
    {

        int playerLevelIndex = LayerMask.NameToLayer("Player");
        int playerMask = (1 << playerLevelIndex);

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag(playerTag))
                {
                    Debug.Log(hit.collider.tag + "pressed.");
                }
               
            }
        }
    }    
}
