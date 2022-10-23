using UnityEngine;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{

    [SerializeField] private Camera cam;

    Ray ray;

    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log("Pressed Player");
                }
            }
        }

    }    
}
