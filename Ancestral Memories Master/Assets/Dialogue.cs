using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private Transform dialogueBox;

    [SerializeField] private string[] lines;
    [SerializeField] private float textSpeed;

    private Player player;

    public bool dialogueActive;

    private int index;

    private GameObject dialogueBoxInstance;

    float distance;
    float distanceThreshold = 20;

    private bool outOfRange;

    private IEnumerator CheckPlayerInRange()
    {
        while (dialogueActive)
        {
            Debug.Log(distance);
            distance = Vector3.Distance(player.transform.root.position, transform.root.position);

            if (distance >= distanceThreshold)
            {
                outOfRange = true;
                StopAllCoroutines();
                dialogueActive = false;
                Destroy(dialogueBoxInstance);

            } else if (distance <= distanceThreshold)
            {
                outOfRange = false;
            }

            yield return null;
        }

        yield break;
    }

    void Update()
    {
        if (dialogueActive)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (textComponent.text == lines[index])
                {
                    NextLine();
                }
                else
                {
                    clickPromptObject.SetActive(true);
                    StopAllCoroutines();
                    textComponent.text = lines[index];

                }
            }
        }
    }

    private Canvas canvas;
    private Canvas canvasInstance;

    private void Start()
    {
        dialogueBox = transform.Find("DialogueBox");
        canvas = dialogueBox.transform.GetComponentInChildren<Canvas>();
        canvas.enabled = false;
    }

    private GameObject clickPromptObject;

    public void StartDialogue(Dialogue dialogue, Player player)
    {
        /*
        if (!dialogue.transform.root.CompareTag("CampFire"))
        {
            this.player = player;
            //StartCoroutine(CheckPlayerInRange());
        }
        */

        if (dialogueBox != null)
        {
            Debug.Log("Dialogue Started.");

            lines = dialogue.lines;

            dialogueBoxInstance = Instantiate(dialogueBox.gameObject, dialogue.transform.root);
            textComponent = dialogueBoxInstance.transform.GetComponentInChildren<TextMeshProUGUI>();
            canvasInstance = dialogueBoxInstance.transform.GetComponentInChildren<Canvas>();

            clickPromptObject = dialogueBoxInstance.transform.GetComponentInChildren<ClickPrompt>().transform.gameObject;
            clickPromptObject.SetActive(false);

            canvasInstance.enabled = true;

            textComponent.text = string.Empty;

            StartDialogue();

        }
        else if (dialogueBox == null)
        {

            Debug.Log("Dialogue Error: No DialogueBox found in NPC.");
            return;
        }
    }

    void StartDialogue()
    {

        index = 0;
        dialogueActive = true;
        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
       if (index < lines.Length - 1)
       {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            dialogueActive = false;
   
            //canvasInstance.enabled = false;
            Destroy(dialogueBoxInstance);
        }
    }

    IEnumerator TypeLine()
    {

        clickPromptObject.SetActive(false);

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        clickPromptObject.SetActive(true);


    }
}







// Tutorial: Published by BMo - https://www.youtube.com/watch?v=8oTYabhj248&t=6s - Mar 19 2021