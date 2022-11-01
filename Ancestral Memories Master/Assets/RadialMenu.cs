using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour
{

    public RadialButton buttonPrefab;
    public RadialButton selected;

    public GameObject hitObject;
    public GameObject player;

    public List<RadialButton> buttons;
    

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

            buttons.Add(newButton);
        }

    }

    private void Update()
    {
        if (!player.GetComponent<CharacterBehaviours>().behaviourIsActive)
        {
            if (Input.GetMouseButtonUp(1))
            {
                if (selected) {

                    switch (selected.title)
                    {
                        case "Pray":
                            Pray();
                            break;
                        case "Look":
                            break;
                        case "Reflect":
                            break;
                        case "Dance":
                            break;
                        case "Harvest":
                            Harvest();
                            break;
                        case "Heal":
                            break;
                    }

                    print(selected.title);

                    StartCoroutine(DestroyBuffer());

                } else if (!selected)
                {
                    Destroy(gameObject);
                }
            }

        } else
        {
            HideButtons();
            return;
        }
    }

    public void HideButtons()
    {
        foreach (RadialButton b in buttons)
        {
            b.gameObject.SetActive(false);
        }
    }

    public IEnumerator DestroyBuffer()
    {
        yield return new WaitUntil(() => player.GetComponent<CharacterBehaviours>().behaviourIsActive == false);
        Destroy(gameObject);
        yield break;
    }

    public void Pray()
    {
        StartCoroutine(player.GetComponent<CharacterBehaviours>().PrayerAnimation());
    }

    public void Harvest()
    {
        StartCoroutine(player.GetComponent<CharacterBehaviours>().HarvestAnimation());
    }
}
