using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    public Action[] options;

    [System.Serializable]
    public class Action
    {
        public Color color;
        public Sprite sprite;
        public string title;
    }

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
                    Debug.Log("Player Selected");
                    RadialMenuSpawner.menuInstance.SpawnMenu();
                }
            }
        }
    }

}
