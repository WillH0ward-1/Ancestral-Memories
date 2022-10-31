using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : IBehaviours
{

    public RadialButton buttonPrefab;
    public RadialButton selected;

    public GameObject hitObject;
    public GameObject player;

    public void SpawnButtons(Interactable obj)
    {
        for (int i = 0; i < obj.options.Length; i++)
        {
            RadialButton newButton = Instantiate(buttonPrefab);
            newButton.transform.SetParent(transform, false); // false = use worldposition

            float theta = (2 * Mathf.PI / obj.options.Length) * i;
            float xPos = Mathf.Sin(theta);
            float yPos = Mathf.Cos(theta);

            newButton.transform.localPosition = new Vector3(xPos, yPos, 0f) * 100f;

            //newButton.transform.localPosition = new Vector3(0f, 100f, 0f);

            // colour = newButton.circle.color;
            //Color colourIndex = obj.options[i].color;
            // colour = colourIndex;
            //colour.a = colourIndex.a;

            newButton.circle.color = obj.options[i].color;
            newButton.icon.sprite = obj.options[i].sprite;
            newButton.title = obj.options[i].title;
            newButton.myMenu = this;
        }

    }
    

    private void Update()
    {

        if (Input.GetMouseButtonUp(1))
        {
            if (selected) // PASS SELECTED HERE - trigger events
            {
                switch (selected.title)
                {
                    case "Pray":
                        Pray(player);
                        break;
                    case "Look":
                        break;
                    case "Reflect":
                        break;
                    case "Dance":
                        break;
                    case "Harvest":
                        StartCoroutine(player.GetComponent<CharacterBehaviours>().HarvestAnimation());
                        break;
                    case "Heal":
                        break;
                }
                print(selected.title);
            }

            Destroy(gameObject);
        }
    }

    
}
