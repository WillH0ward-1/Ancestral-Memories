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
        //public Animation animation;
        //public float alpha = 1f;
    }
   

    public void SpawnMenu(GameObject lastHit)
    {       
        RadialMenuSpawner.menuInstance.SpawnMenu(this, lastHit);
    }

}
