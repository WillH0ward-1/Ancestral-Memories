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

    public string speakerName;

    public string conversationName;

    public EventReference eventPath;

    private void Awake()
    {

    }
    public bool dialogueActive;

    private int index;

    private GameObject dialogueBoxInstance;

    private bool useRandomDialogue;

    [SerializeField] private float distance;
    [SerializeField] private float distanceThreshold = 50f;

    public bool outOfRange;

    public Player player;
    private EVENT_CALLBACK callbackDelegate;

    void Start()
    { 

        dialogueBox = transform.Find("DialogueBox");
        canvas = dialogueBox.transform.GetComponentInChildren<Canvas>();
        canvas.enabled = false;

        callbackDelegate = new EVENT_CALLBACK(FMODDialogueCallback);
    }

    string GetRandomDialogue()
    {
        int randomIndex = UnityEngine.Random.Range(0, lines.Length - 1);
        string randomDialogue = lines[randomIndex];
        return randomDialogue;
    }

    private void FMODStartDialogueEvent(string key)
    {
        var instance = RuntimeManager.CreateInstance(eventPath);
        GCHandle stringHandle = GCHandle.Alloc(key, GCHandleType.Pinned);

        instance.setUserData(GCHandle.ToIntPtr(stringHandle));
        instance.setCallback(callbackDelegate);

        Transform tf = gameObject.GetComponent<Transform>(); 
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        RuntimeManager.AttachInstanceToGameObject(instance, tf, rb);

        instance.start();
        instance.release(); // immediately release
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

        /*
        string key = speakerName + "-"
                   + phraseNumberAsString + "-"
                   + firstWordLowercase;
        */

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        clickPromptObject.SetActive(true);


    }

    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]

    static FMOD.RESULT FMODDialogueCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        EventInstance instance = new EventInstance(instancePtr);

        IntPtr stringPtr;
        instance.getUserData(out stringPtr);

        GCHandle stringHandle = GCHandle.FromIntPtr(stringPtr);
        String key = stringHandle.Target as String;

        switch (type)
        {
            case EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                {
                    FMOD.MODE soundMode = FMOD.MODE.LOOP_NORMAL | FMOD.MODE.CREATECOMPRESSEDSAMPLE | FMOD.MODE.NONBLOCKING;
                    var parameter = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));

                    if (key.Contains(".")) 
                    {
                        var soundResult = RuntimeManager.CoreSystem.createSound(Application.streamingAssetsPath + "/" + key, soundMode, out FMOD.Sound dialogueSound);
                        if (soundResult == FMOD.RESULT.OK)
                        {
                            parameter.sound = dialogueSound.handle;
                            parameter.subsoundIndex = -1;
                            Marshal.StructureToPtr(parameter, parameterPtr, false);
                        }
                    }
                    else
                    {
                        var keyResult = RuntimeManager.StudioSystem.getSoundInfo(key, out SOUND_INFO dialogueSoundInfo);

                        if (keyResult != FMOD.RESULT.OK)
                        {
                            break;
                        }

                        var soundResult = RuntimeManager.CoreSystem.createSound(dialogueSoundInfo.name_or_data, soundMode | dialogueSoundInfo.mode, ref dialogueSoundInfo.exinfo, out FMOD.Sound dialogueSound);
                        if (soundResult == FMOD.RESULT.OK)
                        {
                            parameter.sound = dialogueSound.handle;
                            parameter.subsoundIndex = dialogueSoundInfo.subsoundindex;
                            Marshal.StructureToPtr(parameter, parameterPtr, false);
                        }
                    }

                    break;
                }

            case EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                {
                    var parameter = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));
                    var sound = new FMOD.Sound(parameter.sound);
                    sound.release();
                    break;
                }

            case EVENT_CALLBACK_TYPE.DESTROYED:
                {
                    stringHandle.Free();
                    break;
                }
        }

        return FMOD.RESULT.OK;
    }
}







// Tutorial: Published by BMo - https://www.youtube.com/watch?v=8oTYabhj248&t=6s - Mar 19 2021