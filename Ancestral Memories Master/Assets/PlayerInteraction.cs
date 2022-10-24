using UnityEngine;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    public Camera cam;

    string playerTag = "Player";

    Ray ray;

    

    void FixedUpdate()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        int playerLevelIndex = LayerMask.NameToLayer("Player");

        int playerMask = LayerMask.GetMask("Player");

        if (Input.GetMouseButtonDown(1)) {

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag(playerTag)){
                    print("We hit: " + hit.transform.gameObject.tag);
                }
            }
        }
    }    
}
