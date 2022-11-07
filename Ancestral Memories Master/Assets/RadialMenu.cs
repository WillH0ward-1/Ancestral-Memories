using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour
{

    public RadialButton buttonPrefab;
    [System.NonSerialized] public RadialButton selected;

    public PlayerWalk playerWalk;
    public GameObject player;

    public CharacterBehaviours behaviours;
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
        if (!behaviours.behaviourIsActive)
        {
            if (Input.GetMouseButtonUp(1))
            {
                if (selected)
                {
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
                        case "Plant Seed":
                            break;
                        case "Enter":
                            EnterRoom();
                            break;
                    }

                    HideButtons();
                    print(selected.title);
                    StartCoroutine(DestroyBuffer());

                }
                else
                {
                    Destroy(gameObject);
                }
            }
        } 
    }

    public void EnterRoom()
    {
        behaviours.StartCoroutine(behaviours.WalkIntoRoom());
    }

    public void HideButtons()
    {
        foreach (RadialButton b in buttons)
        {
            b.gameObject.SetActive(false);
        }
    }

    //  public void WalkToward()
    // {
    //     StartCoroutine(playerWalk.WalkToObject());
    // }

    public void Pray()
    {
        player.GetComponent<PlayerWalk>().agent.stoppingDistance = 25f;
        StartCoroutine(behaviours.PrayerAnimation());
    }

    public void Harvest()
    {
        playerWalk.agent.stoppingDistance = 5f;
        StartCoroutine(behaviours.HarvestAnimation());
    }

    public IEnumerator DestroyBuffer()
    {

        yield return new WaitUntil(() => behaviours.behaviourIsActive == false);

        Destroy(gameObject);
        yield break;
        
    }
}