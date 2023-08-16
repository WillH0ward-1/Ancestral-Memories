using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FMOD.Studio;
using System.Runtime.InteropServices;
using FMODUnity;
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

    [SerializeField] private EventReference dialogueEventPath;
    [SerializeField] private EventReference dialogue3DEventPath;
    [SerializeField] private EventReference phonemePath;

    FMOD.Studio.EVENT_CALLBACK callbackDelegate;
    FMOD.Studio.EVENT_CALLBACK callbackDelegate3D;

    private int conversationIndex;
    private GameObject dialogueBoxInstance;

    [SerializeField] private string characterName;
    [SerializeField] private string characterType;
    private bool useRandomDialogue;

    [SerializeField] private float distance;
    [SerializeField] private float minDistance = 0f;
    [SerializeField] private float maxDistance = 50f;
    public bool outOfRange;

    public Player player;
    private GodAudioSFX godAudioManager;

    private Transform godPrefab;
    private GodRangeController godRangeSettings;

    private EventReference DialogueDuckSnapshot;

    public DialogueLines dialogueLines;
    public DialogueLines.Emotions currentEmotion = DialogueLines.Emotions.Neutral;  // Set default emotion to Neutral

    private LanguageGenerator languageGenerator;

    [SerializeField] private EmotionManager emotionManager;

    private AICharacterStats characterStats;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        characterStats = transform.GetComponentInChildren<AICharacterStats>();
        dialogueLines = FindObjectOfType<DialogueLines>();
        languageGenerator = FindObjectOfType<LanguageGenerator>();
        languageGenerator.evolutionFraction = characterStats.evolution;
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
        // Check if EventReference fields are null
        if (dialogueEventPath.Path == null || dialogueEventPath.Path == "")
            Debug.LogError("Dialogue 2D Event Path is not set in " + gameObject.name);
        if (dialogue3DEventPath.Path == null || dialogue3DEventPath.Path == "")
            Debug.LogError("Dialogue 3D Event Path is not set in " + gameObject.name);
        if (phonemePath.Path == null || phonemePath.Path == "")
            Debug.LogError("Dialogue Phoneme Path is not set in " + gameObject.name);
    }

    void Start()
    {
        GameObject dialogueBoxPrefab = Resources.Load("Dialogue/DialogueBox") as GameObject;
        if (dialogueBoxPrefab != null)
        {
            dialogueBox = Instantiate(dialogueBoxPrefab, transform.position, Quaternion.identity);
            canvas = dialogueBox.GetComponentInChildren<Canvas>();
            canvas.enabled = false;
            callbackDelegate = new EVENT_CALLBACK(ProgrammerCallBack.ProgrammerInstCallback);
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

    public void StartDialogue()
    {
        if (dialogueBox == null)
        {
            Debug.LogError("Dialogue Error: " + transform.gameObject.name + " does not contain a dialogue box.");
            return;
        }

        if (Enum.TryParse(characterName, true, out DialogueLines.CharacterNames parsedName) &&
            Enum.TryParse(characterType, true, out DialogueLines.CharacterTypes parsedType))
        {
            lines = dialogueLines.GetDialogue(parsedName, parsedType, currentEmotion).ToArray();

            if (lines.Length == 0 || (lines.Length == 1 && lines[0] == "No dialogue available for this combination."))
            {
                Debug.LogError("No dialogues found for the specified combination.");
                return; // Exit the method if no dialogues are found for the given combination
            }

            translationFunction = GetTranslationFunction(characterName);
        }
        else
        {
            Debug.LogError("Invalid character name or type specified.");
            return; // Exit the method if the character name or type is invalid
        }

        Debug.Log("Dialogue Started.");

        dialogueBoxInstance = Instantiate(dialogueBox);
        dialogueBoxInstance.transform.SetParent(transform, false);

        textComponent = dialogueBoxInstance.GetComponentInChildren<TextMeshProUGUI>();
        OnTextComponentChanged?.Invoke(textComponent); // This line sends the event regardless of the value of textComponent.

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

    private Func<string, string> GetTranslationFunction(string characterName)
    {
        if (characterName.Equals("Neanderthal", StringComparison.OrdinalIgnoreCase))
        {
            return languageGenerator.TranslateToNeanderthal;
        }
        else if (characterName.Equals("MidSapien", StringComparison.OrdinalIgnoreCase))
        {
            return languageGenerator.TranslateToMidSapien;
        }
        else if (characterName.Equals("Sapien", StringComparison.OrdinalIgnoreCase))
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

    IEnumerator TypeLine()
    {
        isLineComplete = false;
        string originalLine = lines[conversationIndex];
        string translatedLine = translationFunction(originalLine);
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(originalLine);

        int originalIndex = 0;
        int translatedIndex = 0;

        while (translatedIndex < translatedLine.Length)
        {
            if (lineInterrupted)
            {
                // Exit coroutine if the line was interrupted
                yield break;
            }

            if (!transform.CompareTag("Animal"))
            {
                GetDialogueAudio(characterName, conversationName, characterType, conversationIndex);
            }

            clickPromptObject.SetActive(false);

            if (originalIndex < originalLine.Length)
            {
                stringBuilder[originalIndex] = translatedLine[translatedIndex];
                originalIndex++;
            }
            else
            {
                stringBuilder.Append(translatedLine[translatedIndex]);
            }

            if (usePhonemes)
            {
                TriggerPhoneme(characterName, characterType);
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

            //dialogueInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

            dialogueInstance.start();
            dialogueInstance.release();

            StartCoroutine(UpdateDistance(dialogueInstance));

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

            var phonemeInstance = RuntimeManager.CreateInstance(phonemePath);

            GCHandle stringHandle = GCHandle.Alloc(fullKey, GCHandleType.Pinned);
            phonemeInstance.setUserData(GCHandle.ToIntPtr(stringHandle));
            phonemeInstance.setCallback(callbackDelegate);

            RuntimeManager.AttachInstanceToGameObject(phonemeInstance, transform);

            phonemeInstance.start();
            phonemeInstance.release();
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
                string indexPart = randomFileNameWithoutExtension.Replace(characterName + "-" + characterType + "-" + PhonemeIdentifier, "");
                return baseKey + indexPart;
            }
        }
        return null;
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