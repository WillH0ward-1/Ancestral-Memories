using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using FMOD.Studio;
using System.Runtime.InteropServices;
//using FMODUnity;
using System.IO;

using System;

public class Dialogue : MonoBehaviour
{
    public bool dialogueActive;
  
    public DialogueLines.Emotions CurrentEmotion
    {
        get { return currentEmotion; }
    }


    public TextMeshProUGUI textComponent;
    public delegate void TextComponentChanged(TextMeshProUGUI newTextComponent);
    public event TextComponentChanged OnTextComponentChanged;


    [SerializeField] private GameObject dialogueBox;

    public string conversationName = "";

    [SerializeField] private string[] lines;
    [SerializeField] private float textSpeed;

    public bool usePhonemes = false;
    int maxPhonemes;

    const string PhonemeFolder = "Phonemes";
    const string PhonemeIdentifier = "Phoneme";

    /*
    [SerializeField] private EventReference dialogueEventPath;
    [SerializeField] private EventReference dialogue3DEventPath;
    [SerializeField] private EventReference phonemePath;

    FMOD.Studio.EVENT_CALLBACK callbackDelegate;
    FMOD.Studio.EVENT_CALLBACK callbackDelegate3D;
    */

    private int conversationIndex;
    private GameObject dialogueBoxInstance;

    public string characterName;
    public string characterType;
    public string characterGender;

    private bool useRandomDialogue;

    [SerializeField] private float distance;
    [SerializeField] private float minDistance = 0f;
    [SerializeField] private float maxDistance = 50f;
    public bool outOfRange;

    public Player player;
    private GodAudioSFX godAudioManager;

    private Transform godPrefab;
    private GodRangeController godRangeSettings;

    // private EventReference DialogueDuckSnapshot;

    public DialogueLines dialogueLines;
    public DialogueLines.Emotions currentEmotion = DialogueLines.Emotions.Neutral;  // Set default emotion to Neutral

    private LanguageGenerator languageGenerator;

    [SerializeField] private EmotionManager emotionManager;

    private void Awake()
    {
        languageGenerator = GetComponent<LanguageGenerator>();
        formantSynth = GetComponent<FormantSynthesizer>();

        ValidateEvents();

        // Debug.Log("Streaming Asset Path:" + Application.streamingAssetsPath);

        if (transform.CompareTag("Campfire"))
        {
            godAudioManager = transform.GetComponent<GodAudioSFX>();
            godPrefab = transform.Find("GodPrefab");
            if (godPrefab != null)
            {
                GodRangeController godRangeControl = godPrefab.GetComponent<GodRangeController>();
                godRangeControl.dialogue = this;
                godRangeSettings = godRangeControl;
            }
        }
    }

    void ValidateEvents()
    {
        /*
        // Check if EventReference fields are null
        if (dialogueEventPath.ToString() == null || dialogueEventPath.ToString() == "")
            Debug.LogError("Dialogue 2D Event Path is not set in " + gameObject.name);
        if (dialogue3DEventPath.ToString() == null || dialogue3DEventPath.ToString() == "")
            Debug.LogError("Dialogue 3D Event Path is not set in " + gameObject.name);
        if (phonemePath.ToString() == null || phonemePath.ToString() == "")
            Debug.LogError("Dialogue Phoneme Path is not set in " + gameObject.name);
        */
    }

    private Transform head;

    void Start()
    {
        GameObject dialogueBoxPrefab = Resources.Load("Dialogue/DialogueBox") as GameObject;
        if (dialogueBoxPrefab != null)
        {
            dialogueBox = Instantiate(dialogueBoxPrefab, transform.position, Quaternion.identity);
            canvas = dialogueBox.GetComponentInChildren<Canvas>();
            canvas.enabled = false;
            // callbackDelegate = new EVENT_CALLBACK(ProgrammerCallBack.ProgrammerInstCallback);
        }
        else
        {
            Debug.LogError("Dialogue box prefab not found!");
        }

        emotionManager = GetComponent<EmotionManager>();
        if (emotionManager != null)
        {
            emotionManager.OnEmotionChanged += HandleEmotionChanged;
        }
    }

    private void HandleEmotionChanged(DialogueLines.Emotions newEmotion)
    {
        currentEmotion = newEmotion;
    }

    private void OnDestroy()
    {
        if (emotionManager != null)
        {
            emotionManager.OnEmotionChanged -= HandleEmotionChanged;
        }
    }

    string GetRandomDialogue()
    {
        int randomIndex = UnityEngine.Random.Range(0, lines.Length - 1);
        string randomDialogue = lines[randomIndex];
        return randomDialogue;
    }

    private Canvas canvas;
    private Canvas canvasInstance;
    private GameObject clickPromptObject;
    private Func<string, string> translationFunction;

    public enum DialogueType
    {
        IdleDialogue,
        BuildingPromptDialogue,
        ShamanIntroduction,
        ShamanFluteTutorial,
        ShamanFluteTutorialFail,
        ShamanTreeTutorial,
        ShamanHumanTutorial,
        ShamanMushroomTutorial,
        ShamanConclusion

    }

    public void StartDialogue(DialogueType dialogueType)
    {
        if (dialogueBox == null)
        {
            Debug.LogError("Dialogue Error: " + transform.gameObject.name + " does not contain a dialogue box.");
            return;
        }

        DialogueLines.Emotions selectedEmotion;

        if (!Enum.TryParse(characterType, true, out DialogueLines.CharacterTypes parsedType) ||
            !Enum.TryParse(characterGender, true, out DialogueLines.CharacterGenders parsedGender))
        {
            Debug.LogError("Invalid character name or type specified.");
            return; // Exit the method if the character name or type is invalid
        }

        switch (dialogueType)
        {
            case DialogueType.IdleDialogue:
                selectedEmotion = currentEmotion;
                break;
            case DialogueType.BuildingPromptDialogue:
                selectedEmotion = DialogueLines.Emotions.BuildingPrompt;
                break;
            case DialogueType.ShamanIntroduction:
                selectedEmotion = DialogueLines.Emotions.ShamanIntroduction;
                break;
            case DialogueType.ShamanFluteTutorial:
                selectedEmotion = DialogueLines.Emotions.ShamanFluteTutorial;
                break;
            case DialogueType.ShamanFluteTutorialFail:
                selectedEmotion = DialogueLines.Emotions.ShamanFluteTutorialFail;
                break;
            case DialogueType.ShamanTreeTutorial:
                selectedEmotion = DialogueLines.Emotions.ShamanTreeTutorial;
                break;
            case DialogueType.ShamanHumanTutorial:
                selectedEmotion = DialogueLines.Emotions.ShamanHumanTutorial;
                break;
            case DialogueType.ShamanMushroomTutorial:
                selectedEmotion = DialogueLines.Emotions.ShamanMushroomTutorial;
                break;
            case DialogueType.ShamanConclusion:
                selectedEmotion = DialogueLines.Emotions.ShamanConclusion;
                break;
            default:
                Debug.LogError("Invalid dialogue type specified.");
                return;
        }

        lines = dialogueLines.GetDialogue(parsedType, parsedGender, selectedEmotion).ToArray();

        if (lines.Length == 0 || (lines.Length == 1 && lines[0] == "No dialogue available for this combination."))
        {
            Debug.LogError("No dialogues found for the specified combination.");
            return;
        }

        translationFunction = GetTranslationFunction(characterType);

        Debug.Log("Dialogue Started.");

        dialogueBoxInstance = Instantiate(dialogueBox);
        dialogueBoxInstance.transform.SetParent(transform, false);

        textComponent = dialogueBoxInstance.GetComponentInChildren<TextMeshProUGUI>();
        OnTextComponentChanged?.Invoke(textComponent);

        if (textComponent == null)
        {
            Debug.LogError("No TextMeshProUGUI component found in the dialogue box.");
            return;
        }

        canvasInstance = dialogueBoxInstance.GetComponentInChildren<Canvas>();
        if (canvasInstance == null)
        {
            Debug.LogError("No Canvas component found in the dialogue box.");
            return;
        }

        clickPromptObject = dialogueBoxInstance.GetComponentInChildren<ClickPrompt>().gameObject;
        if (clickPromptObject == null)
        {
            Debug.LogError("No ClickPrompt component found in the dialogue box.");
            return;
        }

        clickPromptObject.SetActive(false);
        canvasInstance.enabled = true;

        textComponent.text = string.Empty;

        conversationIndex = 0;
        dialogueActive = true;

        StartCoroutine(ContinuousEvolutionUpdate());
        StartCoroutine(TypeLine());
        StartCoroutine(CheckDialogueProgress());
    }

    private bool lineInterrupted = false;
    private bool check = false;
    private bool isLineComplete;



    IEnumerator ContinuousEvolutionUpdate()
    {
        while (dialogueActive)
        {
            yield return new WaitForSeconds(0.1f); 
        }
    }

    private Func<string, string> GetTranslationFunction(string characterType)
    {
        if (characterType.Equals("Neanderthal", StringComparison.OrdinalIgnoreCase))
        {
            return languageGenerator.TranslateToNeanderthal;
        }
        else if (characterType.Equals("MidSapien", StringComparison.OrdinalIgnoreCase))
        {
            return languageGenerator.TranslateToMidSapien;
        }
        else if (characterType.Equals("Sapien", StringComparison.OrdinalIgnoreCase))
        {
            return languageGenerator.TranslateToSapien;
        } 

        // Default to original lines if character name is not recognized
        return (line) => line;
    }

    string translatedLineRef;

    IEnumerator CheckDialogueProgress()
    {
        while (dialogueActive)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (isLineComplete)
                {
                    NextLine();
                }
                else if (lineInterrupted)
                {
                    // If interrupted a second time while already interrupted, proceed to next line
                    NextLine();
                }
                else
                {
                    // If line typing is interrupted, show the full translated line immediately
                    lineInterrupted = true;
                    textComponent.text = translationFunction(lines[conversationIndex]);
                    clickPromptObject.SetActive(true);
                    isLineComplete = true;
                    StopCoroutine(TypeLine());
                }
            }
            yield return null;
        }
    }

    public bool speak = true;
    private FormantSynthesizer formantSynth;

    public bool isSyllabic = false;
    public VocabularyManager vocabularyManager;

    public float timeBetweenSyllables = 0.1f;
    public float timeBetweenWords = 0.5f;
    public float speakingSpeed = 2f;

    IEnumerator TypeLine()
    {
        isLineComplete = false;
        string originalLine = lines[conversationIndex];
        string translatedLine = translationFunction(originalLine);
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(originalLine);

        int originalIndex = 0;
        int translatedIndex = 0;

        if (speak)
        {
            StartCoroutine(HandleSpeaking(translatedLine));
        }

        while (translatedIndex < translatedLine.Length)
        {
            if (lineInterrupted)
            {
                // Exit coroutine if the line was interrupted
                yield break;
            }

            if (!transform.CompareTag("Animal"))
            {
                GetDialogueAudio(characterType, conversationName, characterGender, conversationIndex);
            }

            clickPromptObject.SetActive(false);

            char currentChar = translatedLine[translatedIndex];

            if (originalIndex < originalLine.Length)
            {
                stringBuilder[originalIndex] = currentChar;
                originalIndex++;
            }
            else
            {
                stringBuilder.Append(currentChar);
            }

            translatedIndex++;
            textComponent.text = stringBuilder.ToString();
            yield return new WaitForSeconds(textSpeed);
        }

        if (originalIndex < originalLine.Length)
        {
            stringBuilder.Length = originalIndex;
            textComponent.text = stringBuilder.ToString();
        }

        isLineComplete = true;
        clickPromptObject.SetActive(true);
    }

    IEnumerator HandleSpeaking(string translatedLine)
    {
        string currentWord = "";
        foreach (char c in translatedLine)
        {
            currentWord += c;
            if (c == ' ' || c == translatedLine[translatedLine.Length - 1])
            {
                formantSynth.Speak(currentWord.Trim());
                float deviation = UnityEngine.Random.Range(-0.5f, 0.5f); // e.g., 5% deviation
                yield return new WaitForSeconds(speakingSpeed * (1 + deviation));
                currentWord = ""; // Reset the current word
            }
        }
    }



    void TriggerPhoneme(string name, string type)
    {
        if (dialogueActive)
        {
            string baseKey = name + "/" + type + "/" + PhonemeFolder + "/" + name + "-" + type + "-" + PhonemeIdentifier;
            string fullKey = GetRandomPhonemePath(baseKey);

            if (string.IsNullOrEmpty(fullKey))
                return;

            phonemeKeyRef = fullKey;
            Debug.Log(fullKey);

            /*
            var phonemeInstance = RuntimeManager.CreateInstance(phonemePath);

            GCHandle stringHandle = GCHandle.Alloc(fullKey, GCHandleType.Pinned);
            phonemeInstance.setUserData(GCHandle.ToIntPtr(stringHandle));
            phonemeInstance.setCallback(callbackDelegate);

            RuntimeManager.AttachInstanceToGameObject(phonemeInstance, transform);

            phonemeInstance.start();
            phonemeInstance.release();
            */

        }
    }



    void NextLine()
    {
        isLineComplete = false;
        lineInterrupted = false;  // Reset this flag too, for proper progression to the next line
        conversationIndex++;

        if (conversationIndex < lines.Length)
        {
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
            godRangeSettings.StopCoroutine(godRangeSettings.UpdateActiveStates());
        }
        StopAllCoroutines();
        dialogueActive = false;
        //dialogueInstanceRef.setParameterByNameWithLabel("DialogueActive", "false");
        Destroy(dialogueBoxInstance);
    }

    [SerializeField] private string dialogueKeyRef;
    [SerializeField] private string phonemeKeyRef;

    public void GetDialogueAudio(string name, string conversationName, string type, int lineIndex)
    {
        if (dialogueActive)
        {
            string key = name + "/" + type + "/" + name + "-" + conversationName + "-" + type + "-" + lineIndex;
            dialogueKeyRef = key;

            Debug.Log(key);


            if (transform.CompareTag("Campfire"))
            {
                /*
                var dialogueInstance3D = RuntimeManager.CreateInstance(dialogue3DEventPath);
                RuntimeManager.AttachInstanceToGameObject(dialogueInstance3D, player.transform);

                GCHandle stringHandle3D = GCHandle.Alloc(key, GCHandleType.Pinned);
                dialogueInstance3D.setUserData(GCHandle.ToIntPtr(stringHandle3D));
                dialogueInstance3D.setCallback(callbackDelegate3D);

                dialogueInstance3D.start();
                dialogueInstance3D.release();

                StartCoroutine(UpdateDistance(dialogueInstance3D));
                */

            }

            //dialogueInstance.setParameterByNameWithLabel("DialogueActive", "true");

            /*
            var dialogueInstance = RuntimeManager.CreateInstance(dialogueEventPath);

            GCHandle stringHandle = GCHandle.Alloc(key, GCHandleType.Pinned);
            dialogueInstance.setUserData(GCHandle.ToIntPtr(stringHandle));
            dialogueInstance.setCallback(callbackDelegate);

            //dialogueInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

            dialogueInstance.start();
            dialogueInstance.release();

            StartCoroutine(UpdateDistance(dialogueInstance));
            */

        }
    }

    string DirectoryRoot = Application.dataPath;
    [SerializeField] private string fullPathRef;

    private string GetRandomPhonemePath(string baseKey)
    {
        string[] pathParts = baseKey.Split('/');
        string phonemeDirectory = Path.Combine(pathParts[0], pathParts[1], pathParts[2]);
        string directoryPath = Path.Combine(DirectoryRoot, "AncestralMemoriesSFX", "DialogueBanks", phonemeDirectory);
        directoryPath = directoryPath.Replace("Assets/", "");
        fullPathRef = directoryPath;

        if (Directory.Exists(directoryPath))
        {
            string[] phonemeFiles = Directory.GetFiles(directoryPath);
            if (phonemeFiles.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, phonemeFiles.Length);
                string randomFileNameWithoutExtension = Path.GetFileNameWithoutExtension(phonemeFiles[randomIndex]);
                string indexPart = randomFileNameWithoutExtension.Replace(characterType + "-" + characterGender + "-" + PhonemeIdentifier, "");
                return baseKey + indexPart;
            }
        }
        return null;
    }

    float newMinDistance;
    float newMaxDistance;

    /*
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
    */

    void Update()
    {
        if (dialogueActive)
        {
            distance = Vector3.Distance(transform.position, player.transform.position);

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
}
// Dialogue system tutorial: Published by BMo - https://www.youtube.com/watch?v=8oTYabhj248&t=6s - Mar 19 2021