using UnityEngine;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{

    [SerializeField] private LayerMask layer;
    public Vector3 collision = Vector3.zero;
    public GameObject lastHit;
    [SerializeField] private Camera cam;

    Ray ray;

    private void Awake()
    {

    }

    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {

            lastHit = hit.transform.gameObject;

            if (Input.GetMouseButtonDown(1))
            {
                if (lastHit.CompareTag("Player"))
                {
                    Debug.Log("Player Selected");
                }
            }
        }
    }

}
