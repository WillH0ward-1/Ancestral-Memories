using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterClass
{
    // Start is called before the first frame update
    public Camera interactableCam;
    public Player player;

    void Start()
    {
        base.Awake();
    }

}
