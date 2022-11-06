using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour
{

    public RadialButton buttonPrefab;
    public RadialButton selected;

    public PlayerWalk playerWalk;
    public GameObject player;
    public GameObject hitObject;

    private CharacterBehaviours behaviours;

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

    private void Start()
    {
        behaviours = player.GetComponent<CharacterBehaviours>();
        playerWalk = player.GetComponent<PlayerWalk>();
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
                    }

                    print(selected.title);
                    HideButtons();
                    StartCoroutine(DestroyBuffer());

                }
                else if (!selected)
                {
                    Destroy(gameObject);
                    return;
                }
            }

        } 
    }

    public void HideButtons()
    {
        foreach (RadialButton b in buttons)
        {
            b.gameObject.SetActive(false);
        }
    }

    public void WalkToward()
    {
        StartCoroutine(playerWalk.WalkToObject(hitObject.transform.position));
    }

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