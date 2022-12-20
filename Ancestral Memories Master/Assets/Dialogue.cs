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

    private Player thisPlayer;

    public bool dialogueIsActive;

    private int index;

    private GameObject dialogueBoxInstance;

    float distance;
    float distanceThreshold = 20;

    private bool outOfRange;

    private IEnumerator CheckPlayerInRange()
    {
        while (dialogueIsActive)
        {
            Debug.Log(distance);
            distance = Vector3.Distance(thisPlayer.transform.root.position, transform.root.position);

            if (distance >= distanceThreshold)
            {
     
                outOfRange = true;
                StopAllCoroutines();
                dialogueIsActive = false;
                Destroy(dialogueBoxInstance);

            } else if (distance <= distanceThreshold)
            {
                outOfRange = false;
            }

            yield return null;
        }

        yield break;
    }

    private IEnumerator WaitForSkip()
    {
        while (dialogueIsActive)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (textComponent.text == lines[index])
                {
                    NextLine(lines);
                }
                else
                {
                    StopAllCoroutines();
                    textComponent.text = lines[index];
                }
            }
            yield return null;
        }

        yield break;
    }

    private Canvas canvas;
    private Canvas canvasInstance;

    private void Start()
    {
        dialogueBox = transform.Find("DialogueBox");
        canvas = dialogueBox.transform.GetComponentInChildren<Canvas>();
        canvas.enabled = false;
    }

    public void StartDialogue(Dialogue dialogue, Player player)
    {
        if (!dialogue.transform.root.CompareTag("CampFire"))
        {
            thisPlayer = player;
            StartCoroutine(CheckPlayerInRange());
        }

        if (dialogueBox != null)
        {
            Debug.Log("Dialogue Started.");

            lines = dialogue.lines;

            dialogueBoxInstance = Instantiate(dialogueBox.gameObject, dialogue.transform.root);
            textComponent = dialogueBoxInstance.transform.GetComponentInChildren<TextMeshProUGUI>();
            canvasInstance = dialogueBoxInstance.transform.GetComponentInChildren<Canvas>();
            canvasInstance.enabled = true;

            textComponent.text = string.Empty;

            dialogueIsActive = true;

            index = 0;

            StartCoroutine(TypeLine());
        }
        else if (dialogueBox == null)
        {

            Debug.Log("Dialogue Error: No DialogueBox found in NPC.");
            return;
        }
    }

    void NextLine(string[] lines)
    {
       if (index < lines.Length - 1)
       {

            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            dialogueIsActive = false;
            Destroy(dialogueBoxInstance);
        }
    }

    IEnumerator TypeLine()
    {
        StartCoroutine(WaitForSkip());

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}







// Tutorial: Published by BMo - https://www.youtube.com/watch?v=8oTYabhj248&t=6s - Mar 19 2021