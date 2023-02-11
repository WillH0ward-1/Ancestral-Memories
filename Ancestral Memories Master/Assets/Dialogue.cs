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

    public bool dialogueActive;

    private int index;

    private GameObject dialogueBoxInstance;

    [SerializeField] private float distance;
    [SerializeField] private float distanceThreshold = 50f;

    public bool outOfRange;

    public Player player;

    void Update()
    {
        if (dialogueActive)
        {
            distance = Vector3.Distance(transform.position, player.transform.position);

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

            if (distance <= distanceThreshold)
            {
                outOfRange = false;
            }
            else if (distance >= distanceThreshold)
            {
                outOfRange = true;

                if (!transform.CompareTag("Campfire"))
                {
                    StopDialogue();
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

    public void StartDialogue(Dialogue dialogue, Player playerRef)
    {
        /*
        if (!dialogue.transform.root.CompareTag("CampFire"))
        {
            this.player = player;
            //StartCoroutine(CheckPlayerInRange());
        }
        */
        player = playerRef;

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
            StopDialogue();
        }
    }

    void StopDialogue()
    {
        StopAllCoroutines();
        dialogueActive = false;
        Destroy(dialogueBoxInstance);
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