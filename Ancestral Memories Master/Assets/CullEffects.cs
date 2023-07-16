using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullEffects : MonoBehaviour
{
    public float cullRange = 50;
    public bool isCulled = false;
    private Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        // Check if the distance between the object and the player is greater than the cullRange
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > cullRange)
        {
            isCulled = true;
            // Perform any culling-related actions here, such as disabling the object or stopping its effects
        }
        else
        {
            isCulled = false;
            // Perform any actions here to restore the object or resume its effects if it was previously culled
        }
    }
}
