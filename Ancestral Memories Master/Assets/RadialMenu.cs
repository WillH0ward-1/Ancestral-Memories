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

    private CharacterBehaviours behaviours;

    public List<RadialButton> buttons;

    public GameObject hitObject;

    public void SpawnButtons(Interactable obj, GameObject lastHit)
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

        hitObject = lastHit;

    }

    private void Start()
    {
        behaviours = player.GetComponent<CharacterBehaviours>();
        playerWalk = player.GetComponent<PlayerWalk>();
    }

    private void Update()
    {
            if (Input.GetMouseButtonUp(1))
            {
                if (selected)
                {
                    WalkToward(hitObject.transform.position, this);

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

    public IEnumerator ChooseBehaviour(RadialButton selected)
    {
        switch (selected.title)
        {
            case "Pray":
                Pray();
                break;
            case "Look":
                Look();
                break;
            case "Reflect":
                Reflect();
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

        yield break;
    }

    public void HideButtons()
    {
        foreach (RadialButton button in buttons)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void WalkToward(Vector3 hitDestination, RadialMenu radialMenu)
    {
        StartCoroutine(playerWalk.WalkToObject(hitDestination, radialMenu));
    }

    void SetStopDistance()
    {
        player.GetComponent<PlayerWalk>().agent.stoppingDistance = 25f;
    }

    public void Pray()
    {
        if (!player.GetComponent<CharacterBehaviours>().behaviourIsActive)
        {
            StartCoroutine(behaviours.PrayerAnimation());
        } else
        {
            return;
        }
    }

    public void Look()
    {

    }

    public void Harvest()
    {
        playerWalk.agent.stoppingDistance = 5f;
        StartCoroutine(behaviours.HarvestAnimation());
    }
    public void Reflect()
    {

    }

    public IEnumerator DestroyBuffer()
    {

        yield return new WaitUntil(() => behaviours.behaviourIsActive == false);

        Destroy(gameObject);
        yield break;
        
    }
}