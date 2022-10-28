using UnityEngine;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{

    [SerializeField] private LayerMask layer;
    public Vector3 collision = Vector3.zero;
    public GameObject lastHit;
    [SerializeField] private Camera cam;

    public RadialMenu radialMenu;

    Ray ray;


    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer))
        {

            lastHit = hit.transform.gameObject;

            if (Input.GetMouseButtonDown(1))
            {
                if (lastHit.CompareTag("Player"))
                {
                    radialMenu.Open();
                    Debug.Log("Player Selected");
                }
            }
        }
    }

}
