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

    public GameObject lastHit;
    public RaycastHit rayHit;

    public AreaManager areaManager;

    [SerializeField] private TextMeshProUGUI tmp;

    [SerializeField] private float hoverAmplitude = 5.0f;
    [SerializeField] private float hoverFrequency = 1.0f;

    [SerializeField] private float popDuration = 0.25f; // Duration of the pop effect
    [SerializeField] private AnimationCurve popCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Customize this curve in the inspector

    private void Awake()
    {
        selectionText = "";
        tmp = transform.GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SpawnButtons(Interactable obj, GameObject lastHit, RaycastHit hit)
    {
        this.lastHit = lastHit;
        rayHit = hit;

        StartCoroutine(SpawnAndAnimateButtons(obj)); // This coroutine will handle button creation and animations
    }

    private IEnumerator SpawnAndAnimateButtons(Interactable obj)
    {
        for (int i = 0; i < obj.options.Length; i++)
        {
            RadialButton newButton = Instantiate(buttonPrefab);
            newButton.transform.SetParent(transform, false);

            float theta = (2 * Mathf.PI / obj.options.Length) * i;
            float xPos = Mathf.Sin(theta);
            float yPos = Mathf.Cos(theta);

            newButton.transform.localPosition = new Vector3(xPos, yPos, 0f) * 150f;
            newButton.transform.localScale = Vector3.zero;
            newButton.circle.color = obj.options[i].color;
            newButton.icon.sprite = obj.options[i].sprite;
            newButton.title = obj.options[i].title;
            newButton.myMenu = this;

            buttons.Add(newButton);

            float hoverOffset = Random.Range(0f, 2f * Mathf.PI);
            StartCoroutine(HoverButton(newButton, hoverOffset));
            //StartCoroutine(PulseButton(newButton));

            if (i == 0)
            {
                StartCoroutine(PopButton(newButton));
            }
            else
            {
                float delay = Random.Range(0.25f, 0.5f);
                yield return new WaitForSeconds(delay);
                StartCoroutine(PopButton(newButton));
            }
        }
    }

    private IEnumerator PulseButton(RadialButton button)
    {
        while (true)
        {
            float pulseScale = 1 + Random.Range(-0.05f, 0.05f); // Random factor within a small range
            float pulseDuration = Random.Range(0.5f, 1.5f); // Random duration for the pulse
            Vector3 originalScale = button.transform.localScale;
            Vector3 targetScale = originalScale * pulseScale;

            float time = 0;
            while (time < pulseDuration)
            {
                time += Time.deltaTime;
                button.transform.localScale = Vector3.Lerp(originalScale, targetScale, time / pulseDuration);
                yield return null;
            }

            // Reverse the pulsing
            time = 0;
            while (time < pulseDuration)
            {
                time += Time.deltaTime;
                button.transform.localScale = Vector3.Lerp(targetScale, originalScale, time / pulseDuration);
                yield return null;
            }

            yield return null;
        }
    }

    private IEnumerator PopButton(RadialButton button)
    {
        float time = 0;

        while (time < popDuration)
        {
            time += Time.deltaTime;
            float scale = popCurve.Evaluate(time / popDuration);
            button.transform.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }

        button.transform.localScale = Vector3.one; // Ensure it ends at the correct scale
    }


    private IEnumerator HoverButton(RadialButton button, float offset)
    {
        Vector3 originalPosition = button.transform.localPosition;
        while (true) // loop indefinitely
        {
            float y = hoverAmplitude * Mathf.Sin(hoverFrequency * Time.time + offset);
            button.transform.localPosition = originalPosition + new Vector3(0, y, 0);
            yield return null;
        }
    }

    public bool walkingToward = false;

    private void Update()
    {
        tmp.text = selectionText;

        if (Input.GetMouseButtonUp(1) && !behaviours.behaviourIsActive)
        {
            CleanUpButtons(); // Clean up buttons when mouse is released

            if (selected)
            {
                Debug.Log("Selected " + selected.title + " !");

                if (selected.title == "Enter")
                {
                    if (!behaviours.psychModeIncoming)
                    {
                        Debug.Log("Entering Portal! Hitobject = " + lastHit + ".");
                        areaManager.StartCoroutine(areaManager.EnterPortal(lastHit));
                    }
                }
                else
                {
                    Debug.Log("Walking Toward.");
                    behaviours.WalkToward(lastHit, selected.title, rayHit);
                }
            }
            else
            {
                Debug.Log("Not Selected!");
            }
        }
    }

    private void CleanUpButtons()
    {
        foreach (RadialButton button in buttons)
        {
            if (button != null)
            {
                Destroy(button.gameObject); // Optionally, you can choose to hide them instead
            }
        }
        buttons.Clear(); // Clear the list of buttons

        Destroy(gameObject);
    }


    private void DestroyMenu()
    {

    }

    public void HideButtons()
    {
        foreach (RadialButton button in buttons)
        {
            if (button != null)
            {
                button.icon.enabled = false;
                Image image = button.GetComponent<Image>();
                image.enabled = false;
            }
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