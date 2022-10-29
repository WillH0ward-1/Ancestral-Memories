using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenu : MonoBehaviour
{

    public RadialButton buttonPrefab;
    public RadialButton selected;

    private void Start()
    {
        RadialButton newButton = Instantiate(buttonPrefab) as RadialButton;
        newButton.transform.SetParent(transform, false); // false = worldposition

        newButton.transform.localPosition = new Vector3(0f, 100f, 0f);
    }

}
