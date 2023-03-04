using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FMOD.Studio;
using System.Runtime.InteropServices;
using FMODUnity;
using System;

public class Dialogue : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private Transform dialogueBox;

    [SerializeField] private string[] lines;
    [SerializeField] private float textSpeed;

    public string conversationName = "";

    public EventReference eventPath;

    public bool dialogueActive;

    private int index;

    private GameObject dialogueBoxInstance;

    [SerializeField] private string characterName;
    [SerializeField] private string type;

    private bool useRandomDialogue;

    [SerializeField] private float distance;
    [SerializeField] private float minDistance = 0f;
    [SerializeField] private float maxDistance = 50f;

    public bool outOfRange;

    public Player player;
    FMOD.Studio.EVENT_CALLBACK callbackDelegate;

    private void Awake()
    {
        Debug.Log("Streaming Asset Path:" + Application.streamingAssetsPath);
    }

    void Start()
    {
        dialogueBox = transform.Find("DialogueBox");
        canvas = dialogueBox.transform.GetComponentInChildren<Canvas>();
        canvas.enabled = false;
        callbackDelegate = new EVENT_CALLBACK(ProgrammerCallBack.ProgrammerInstCallback);

        // Debug.Log("Streaming Asset Path:" + Application.streamingAssetsPath);
    }

    string GetRandomDialogue()
    {
        int randomIndex = UnityEngine.Random.Range(0, lines.Length - 1);
        string randomDialogue = lines[randomIndex];
        return randomDialogue;
    }

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

            if (distance <= maxDistance)
            {
                outOfRange = false;
            }
            else if (distance >= maxDistance)
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

            index = 0;
            dialogueActive = true;

            StartCoroutine(TypeLine());

        }
        else if (dialogueBox == null)
        {

            Debug.Log("Dialogue Error: " + transform.gameObject.name + " does not contain a dialogue box.");
            return;
        }
    }

    private EventInstance dialogueInstanceRef;

    public void GetDialogueAudio(string name, string conversationName, string type, int lineIndex)
    {
        if (dialogueActive)
        {
            string key = name + "/" + type + "/" + name + "-" + conversationName + "-" + type + "-" + lineIndex;

            Debug.Log(key);

            var dialogueInstance = RuntimeManager.CreateInstance(eventPath);
            //dialogueInstance.setParameterByNameWithLabel("DialogueActive", "true");

            StartCoroutine(UpdateDistance(dialogueInstance));

            // int numberStartIndex = filePath.LastIndexOfAny("0123456789".ToCharArray()) + 1;
            // string numberString = filePath.Substring(numberStartIndex);

            /*
            if (int.TryParse(numberString, out int foundIndex))
            {
                if (lineIndex == foundIndex)
                {
            */
            //Debug.Log("Found index: " + foundIndex);

            // Pin the key string in memory and pass a pointer through the user data
            GCHandle stringHandle = GCHandle.Alloc(key, GCHandleType.Pinned);
            dialogueInstance.setUserData(GCHandle.ToIntPtr(stringHandle));
            dialogueInstance.setCallback(callbackDelegate);
            dialogueInstanceRef = dialogueInstance;

            //dialogueInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            dialogueInstance.start();
            dialogueInstance.release();
        }
        /*
                else if (lineIndex != foundIndex)
                {
                    Debug.Log("Incorrect index found at: " + foundIndex);
                }
            }

        }
        */
    }

    float newMinDistance;
    float newMaxDistance;

    public IEnumerator UpdateDistance(EventInstance dialogueInstance)
    {
        var t = Mathf.InverseLerp(minDistance, maxDistance, distance);
        float output = Mathf.Lerp(newMinDistance, newMaxDistance, t);

        while (dialogueActive)
        {
            dialogueInstance.setParameterByName("DistanceFromSpeaker", output);
            yield return null;
        }

        yield break;
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
        //dialogueInstanceRef.setParameterByNameWithLabel("DialogueActive", "false");

        Destroy(dialogueBoxInstance);
    }

    void GetAudio()
    {
        GetDialogueAudio(characterName, conversationName, type, index);
    }

    IEnumerator TypeLine()
    {
        GetAudio();

        clickPromptObject.SetActive(false);

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        clickPromptObject.SetActive(true);


    }

}


// Dialogue system tutorial: Published by BMo - https://www.youtube.com/watch?v=8oTYabhj248&t=6s - Mar 19 2021