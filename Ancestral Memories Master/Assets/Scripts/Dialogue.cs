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
    [SerializeField] private float distanceThreshold = 50f;

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
        callbackDelegate = new FMOD.Studio.EVENT_CALLBACK(DialogueEventCallback);
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

            Debug.Log("Dialogue Error: " + transform.gameObject.name + " does not contain a dialogue box.");
            return;
        }
    }

    void StartDialogue()
    {
        index = 0;
        dialogueActive = true;

        StartCoroutine(TypeLine());
    }

    public void GetDialogueAudio(string name, string conversationName, string type, int lineIndex)
    {
        if (dialogueActive)
        {
            string key = name + "/" + type + "/" + name + "-" + conversationName + "-" + type + "-" + lineIndex;

            Debug.Log(key);

            var dialogueInstance = RuntimeManager.CreateInstance(eventPath);

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

    // Code from FMOD examples https://www.fmod.com/docs/2.02/unity/examples-programmer-sounds.html

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT DialogueEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr stringPtr;
        instance.getUserData(out stringPtr);

        // Get the string object
        GCHandle stringHandle = GCHandle.FromIntPtr(stringPtr);
        String key = stringHandle.Target as String;

        switch (type)
        {
            case FMOD.Studio.EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                {
                    FMOD.MODE soundMode = FMOD.MODE.LOOP_NORMAL | FMOD.MODE.CREATECOMPRESSEDSAMPLE | FMOD.MODE.NONBLOCKING;
                    var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));

                    if (key.Contains("."))
                    {
                        FMOD.Sound dialogueSound;
                        var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(Application.streamingAssetsPath + "/" + key, soundMode, out dialogueSound);
                        if (soundResult == FMOD.RESULT.OK)
                        {
                            parameter.sound = dialogueSound.handle;
                            parameter.subsoundIndex = -1;
                            Marshal.StructureToPtr(parameter, parameterPtr, false);
                        }
                    }
                    else
                    {
                        FMOD.Studio.SOUND_INFO dialogueSoundInfo;
                        var keyResult = FMODUnity.RuntimeManager.StudioSystem.getSoundInfo(key, out dialogueSoundInfo);
                        if (keyResult != FMOD.RESULT.OK)
                        {
                            break;
                        }
                        FMOD.Sound dialogueSound;
                        var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(dialogueSoundInfo.name_or_data, soundMode | dialogueSoundInfo.mode, ref dialogueSoundInfo.exinfo, out dialogueSound);
                        if (soundResult == FMOD.RESULT.OK)
                        {
                            parameter.sound = dialogueSound.handle;
                            parameter.subsoundIndex = dialogueSoundInfo.subsoundindex;
                            Marshal.StructureToPtr(parameter, parameterPtr, false);
                        }
                    }
                    break;
                }
            case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                {
                    var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));
                    var sound = new FMOD.Sound(parameter.sound);
                    sound.release();

                    break;
                }
            case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                {
                    // Now the event has been destroyed, unpin the string memory so it can be garbage collected
                    stringHandle.Free();

                    break;
                }
        }
        return FMOD.RESULT.OK;
    }
}


// Tutorial: Published by BMo - https://www.youtube.com/watch?v=8oTYabhj248&t=6s - Mar 19 2021