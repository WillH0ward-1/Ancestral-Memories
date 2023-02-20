using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadialMenu : MonoBehaviour
{

    public RadialButton buttonPrefab;
    public RadialButton selected;
    public string selectionText;

    public PlayerWalk playerWalk;
    public GameObject player;

    public CharacterBehaviours behaviours;

    public List<RadialButton> buttons;

    public GameObject hitObject;
    public RaycastHit rayHit;

    public AreaManager areaManager;

    [SerializeField] private TextMeshProUGUI tmp;

    private void Awake()
    {
        selectionText = "";
    }

    public void SpawnButtons(Interactable obj, GameObject lastHit, RaycastHit hit)
    {
        hitObject = lastHit;
        rayHit = hit;

        tmp = transform.GetComponentInChildren<TextMeshProUGUI>();
        StartCoroutine(AnimateButtons(obj));
    }

    private IEnumerator AnimateButtons(Interactable obj)
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
            yield return new WaitForSeconds(Random.Range(0.01f, 0.02f));
            
        }
    }

    [SerializeField] private float buttonRevealSpeed;
    [SerializeField] Vector3 targetButtonSize = new(1, 1, 1);

    public void Animate(RadialButton newButton)
    {
        StartCoroutine(newButton.AnimateButtonIn(buttonRevealSpeed, targetButtonSize));
    }

    public bool walkingToward = false;

    private void Update()
    {
        tmp.text = selectionText;

        if (Input.GetMouseButtonUp(1) && !behaviours.behaviourIsActive)
        {
            if (selected)
            {

                Debug.Log("Selected!");

                HideButtons();

                if (selected.title == "Enter")
                {
                    if (!behaviours.psychModeIncoming)
                    {
                        StartCoroutine(PortalDestroyBuffer());
                        StartCoroutine(areaManager.EnterPortal(hitObject));
                        return;
                    }

                    return;
                }
                else
                {
                    StartCoroutine(DestroyBuffer());
                    behaviours.WalkToward(hitObject, selected.title, rayHit);

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

    private void DestroyMenu()
    {

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

        yield return new WaitUntil(() => playerWalk.reachedDestination && !behaviours.behaviourIsActive);
        Destroy(gameObject);
        yield break;
        
    }


    public IEnumerator PortalDestroyBuffer()
    {

        yield return new WaitUntil(() => !areaManager.traversing);
        Destroy(gameObject);
        yield break;

    }
}