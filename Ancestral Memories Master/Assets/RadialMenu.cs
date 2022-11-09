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

    public bool walkingToward = false;

    private void Update()
    {
        if (Input.GetMouseButtonUp(1) && !behaviours.behaviourIsActive)
        {
            if (selected)
            {
                Debug.Log("Selected!");

                HideButtons();
                StartCoroutine(DestroyBuffer());

                if (!walkingToward)
                {
                    behaviours.WalkToward(hitObject.transform.position, this);
                    walkingToward = true;
                    return;
                }
            }

            else if (!selected)
            {
                Debug.Log("Not Selected!");

                Destroy(gameObject);
                return;
            }
            
        }
    }

    public void HideButtons()
    {
        foreach (RadialButton button in buttons)
        {
            //button.gameObject.SetActive(false);
            button.icon.enabled = false;
           
            Image image = button.GetComponent<Image>();
            image.enabled = false;
            
        }
    }

    public IEnumerator DestroyBuffer()
    {

        yield return new WaitUntil(() => playerWalk.reachedDestination);

        Destroy(gameObject);
        yield break;
        
    }
}