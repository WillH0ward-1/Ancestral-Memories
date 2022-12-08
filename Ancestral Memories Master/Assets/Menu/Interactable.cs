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
   

    public void SpawnMenu(GameObject lastHit, RaycastHit rayHit)
    {       
        RadialMenuSpawner.menuInstance.SpawnMenu(this, lastHit, rayHit);
    }

}
