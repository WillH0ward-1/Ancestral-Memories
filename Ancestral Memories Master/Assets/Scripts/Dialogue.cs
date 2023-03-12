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

    public EventReference dialogueEventPath;
    public EventReference dialogueTickPath;

    public EventReference dialogue3DEventPath;

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

    public bool useDialogueTick = false;

    public Player player;
    FMOD.Studio.EVENT_CALLBACK callbackDelegate;
    FMOD.Studio.EVENT_CALLBACK callbackDelegate3D;


    private void Awake()
    {
        Debug.Log("Streaming Asset Path:" + Application.streamingAssetsPath);
        if (transform.CompareTag("Campfire"))
        {
            godAudioManager = transform.GetComponent<GodAudioSFX>();
        }
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

            if (transform.CompareTag("Campfire"))
            {
                godAudioManager.StartGodAmbienceFX();
            }

        }
        else if (dialogueBox == null)
        {

            Debug.Log("Dialogue Error: " + transform.gameObject.name + " does not contain a dialogue box.");
            return;
        }
    }

    private EventInstance dialogueInstanceRef;

    private GodAudioSFX godAudioManager;

    public void GetDialogueAudio(string name, string conversationName, string type, int lineIndex)
    {

        if (dialogueActive)
        {
  
            string key = name + "/" + type + "/" + name + "-" + conversationName + "-" + type + "-" + lineIndex;

            Debug.Log(key);

        
            if (transform.CompareTag("Campfire"))
            {
                var dialogueInstance3D = RuntimeManager.CreateInstance(dialogue3DEventPath);
                RuntimeManager.AttachInstanceToGameObject(dialogueInstance3D, player.transform);

                GCHandle stringHandle3D = GCHandle.Alloc(key, GCHandleType.Pinned);
                dialogueInstance3D.setUserData(GCHandle.ToIntPtr(stringHandle3D));
                dialogueInstance3D.setCallback(callbackDelegate3D);

                dialogueInstance3D.start();
                dialogueInstance3D.release();

                StartCoroutine(UpdateDistance(dialogueInstance3D));

            }

            //dialogueInstance.setParameterByNameWithLabel("DialogueActive", "true");

            var dialogueInstance = RuntimeManager.CreateInstance(dialogueEventPath);

            GCHandle stringHandle = GCHandle.Alloc(key, GCHandleType.Pinned);
            dialogueInstance.setUserData(GCHandle.ToIntPtr(stringHandle));
            dialogueInstance.setCallback(callbackDelegate);

            dialogueInstanceRef = dialogueInstance;

            //dialogueInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            dialogueInstance.start();
            dialogueInstance.release();

            StartCoroutine(UpdateDistance(dialogueInstance));

        }
    }

    int maxPhonemes = 23; // Should be max index (contents) of the desired file directory.
    //public string[] phonemes;

    void DialogueTick(string name, string type)
    {
        string PhonemeFolder = "Phonemes";
        string PhonemeIdentifier = "Phoneme";

        if (dialogueActive)
        {
            string key = name + "/" + type + "/" + PhonemeFolder + "/" + name + "-" + type + "-" + PhonemeIdentifier + UnityEngine.Random.Range(0, maxPhonemes);

            Debug.Log(key);

            var dialogueTickInstance = RuntimeManager.CreateInstance(dialogueTickPath);
            //dialogueInstance.setParameterByNameWithLabel("DialogueActive", "true");

            GCHandle stringHandle = GCHandle.Alloc(key, GCHandleType.Pinned);
            dialogueTickInstance.setUserData(GCHandle.ToIntPtr(stringHandle));
            dialogueTickInstance.setCallback(callbackDelegate);

            RuntimeManager.AttachInstanceToGameObject(dialogueTickInstance, transform);

            dialogueTickInstance.start();
            dialogueTickInstance.release();
        }
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
        if (transform.CompareTag("Campfire"))
        {
            godAudioManager.StopGodAmbienceFX();
        }

        StopAllCoroutines();
        dialogueActive = false;
        //dialogueInstanceRef.setParameterByNameWithLabel("DialogueActive", "false");

        Destroy(dialogueBoxInstance);
    }

    IEnumerator TypeLine()
    {
        if (!transform.CompareTag("Animal"))
        {
            GetDialogueAudio(characterName, conversationName, type, index);
        }

        clickPromptObject.SetActive(false);

        foreach (char c in lines[index].ToCharArray())
        {
            if (useDialogueTick)
            {
                DialogueTick(characterName, type);
            }

            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        clickPromptObject.SetActive(true);


    }

}


// Dialogue system tutorial: Published by BMo - https://www.youtube.com/watch?v=8oTYabhj248&t=6s - Mar 19 2021