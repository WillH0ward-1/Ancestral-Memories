using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IBehaviours : MonoBehaviour
{
    public virtual void Pray(Player player)
    {
        StartCoroutine(player.GetComponent<CharacterBehaviours>().PrayerAnimation());
    }
}
