using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaithBar : MonoBehaviour
{
    public Image faithBar;

    public void UpdateFaith(float fraction)
    {
        faithBar.fillAmount = fraction;
    }
}
