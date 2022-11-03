using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject player;
    public Action[] options;

    [System.Serializable]
    public class Action
    {
        public Color color;
        public Sprite sprite;
        public string title;
        //public Animation animation;
        //public float alpha = 1f;
    }

    public LayerMask layer;
    public Vector3 collision = Vector3.zero;
    public GameObject lastHit;
    public Camera cam;

    public PlayerWalk playerWalk;
    public RadialMenu radialMenu;
    
    Ray ray;

    private void Start()
    {
         cam = player.GetComponent<Player>().interactableCam;
    }

    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!player.GetComponent<CharacterBehaviours>().behaviourIsActive)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer))
            {
                GameObject lastHit = hit.transform.gameObject;

                if (Input.GetMouseButtonDown(1))
                {
                    radialMenu.playerWalk = playerWalk;
                    radialMenu.player = player;
                    radialMenu.hitObject = lastHit;
                    
                    RadialMenuSpawner.menuInstance.SpawnMenu(this);
                  
                    Debug.Log(lastHit + "selected");
                }
            }
        } 
    }

}
