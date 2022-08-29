using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionBar : MonoBehaviour
{
    public Image evolutionBar;

    public void UpdateHealth(float fraction)
    {
        evolutionBar.fillAmount = fraction;
    }
}
