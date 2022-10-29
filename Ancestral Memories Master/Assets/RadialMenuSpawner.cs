using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenuSpawner : MonoBehaviour
{
    public static RadialMenuSpawner menuInstance;
    public RadialMenu menuPrefab;

    private void Awake()
    {
        menuInstance = this;
    }


    public void SpawnMenu(Interactable obj) {
        RadialMenu newMenu = Instantiate(menuPrefab);
        newMenu.transform.SetParent(transform, true); // false = worldposition

        newMenu.transform.position = Input.mousePosition;
        newMenu.SpawnButtons(obj);
    }

}
