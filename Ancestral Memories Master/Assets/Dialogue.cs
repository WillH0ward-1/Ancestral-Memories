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

    public bool dialogueIsActive;

    private int index;

    private IEnumerator WaitForSkip(GameObject dialogueBoxInstance)
    {
        while (dialogueIsActive)
        {

            if (Input.GetMouseButtonDown(1))
            {
                if (textComponent.text == lines[index])
                {
                    NextLine(dialogueBoxInstance);
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

    private void Start()
    {
        canvas = transform.GetComponentInChildren<Canvas>();
        canvas.enabled = false;
    }

    public void StartDialogue(GameObject other)
    {
        Dialogue dialogue = other.transform.GetComponent<Dialogue>();

        dialogueBox = dialogue.transform.Find("DialogueBox");
 
        Debug.Log("Dialogue Started.");

        lines = dialogue.lines;
        textComponent = dialogueBox.transform.GetComponentInChildren<TextMeshProUGUI>();

        canvas.enabled = true;

        textComponent.text = string.Empty;

        dialogueIsActive = true;

        index = 0;

        StartCoroutine(TypeLine());
        StartCoroutine(WaitForSkip(dialogueBox.gameObject));
    }

    void NextLine(GameObject dialogueBoxInstance)
    {
       if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());

        } else
        {
            dialogueIsActive = false;
            dialogueBoxInstance.SetActive(false);
            //Destroy(dialogueBox);
        }
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}







// Tutorial: Published by BMo - https://www.youtube.com/watch?v=8oTYabhj248&t=6s - Mar 19 2021