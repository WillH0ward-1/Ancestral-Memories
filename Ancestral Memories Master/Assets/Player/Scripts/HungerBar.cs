using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HungerBar : MonoBehaviour
{
    public Image hungerBar;

    public  void UpdateHunger(float fraction)
    {
        hungerBar.fillAmount = fraction;
    }
}
