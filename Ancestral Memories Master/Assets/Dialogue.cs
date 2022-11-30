using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private GameObject dialogueBox;

    [SerializeField] private string[] lines;
    [SerializeField] private float textSpeed;

    public bool dialogueIsActive;

    private int index;

    private IEnumerator WaitForSkip()
    {
        while (dialogueIsActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (textComponent.text == lines[index])
                {
                    NextLine();
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

    public void StartDialogue()
    {

        Debug.Log("Dialogue Started.");
        dialogueBox = Instantiate(dialogueBox, transform);

        textComponent = dialogueBox.transform.GetComponentInChildren<TextMeshProUGUI>();

        textComponent.text = string.Empty;

        dialogueIsActive = true;

        index = 0;

        StartCoroutine(TypeLine());
        StartCoroutine(WaitForSkip());
    }

    void NextLine()
    {
       if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());

        } else
        {
            dialogueIsActive = false;
            dialogueBox.SetActive(false);
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
