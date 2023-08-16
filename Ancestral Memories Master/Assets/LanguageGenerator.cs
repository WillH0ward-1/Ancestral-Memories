using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using System.Text;

public class LanguageGenerator : MonoBehaviour
{
    public enum Consonants
    {
        P, B, T, D, K, G, F, V, S, Z,
        SH, ZH, CH, J, M, N, NG, R, L,
        H, W, WH, TH, DH, Y, QU, X
    }

    public enum Vowels
    {
        A, E, I, O, U, Y, AE, AI, AO, AU,
        EI, IA, IE, OI, OU, UA, UI, UE, EE, OO
    }

    private FormantSynthesizer formantSynth;

    private Dictionary<string, string> EnglishToNeanderthalDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> EnglishToMidSapienDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> EnglishToSapienDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    private DialogueLines dialogueLines;

    private VocabularyManager vocabularyManager;
    private AICharacterStats characterStats;

    public float languageEvolution;

    private void Start()
    {
        vocabularyManager = FindObjectOfType<VocabularyManager>();
        dialogueLines = FindObjectOfType<DialogueLines>();
        characterStats = GetComponent<AICharacterStats>();
        formantSynth = GetComponent<FormantSynthesizer>();

        SaveVocabularyToFile();

        // Read the file once and initialize dictionaries
        string path = Path.Combine(Application.dataPath, "EveryWord.txt");
        string[] englishWords = SafeReadAllLines(path); // New method to safely read all lines

        InitializeDictionary(englishWords, EnglishToNeanderthalDictionary, 2);
        InitializeDictionary(englishWords, EnglishToMidSapienDictionary, 3);
        InitializeDictionary(englishWords, EnglishToSapienDictionary, 4);

        if (characterStats != null)
        {
            characterStats.OnEvolutionChanged += HandleEvolutionChanged;
        }
    }

    private void OnDestroy()
    {
        if (characterStats != null)
        {
            characterStats.OnEvolutionChanged -= HandleEvolutionChanged;
        }
    }

    private void HandleEvolutionChanged(float evolutionFraction, float min, float max)
    {
        languageEvolution = evolutionFraction;
    }


    private string[] SafeReadAllLines(string path)
    {
        try
        {
            return File.ReadAllLines(path);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error reading the file: {ex.Message}");
            return new string[0]; // Return empty array on error
        }
    }


    public void SaveVocabularyToFile()
    {
        List<string> vocabulary = dialogueLines.GetVocabulary();
        vocabularyManager.AddVocabulary(vocabulary);
    }

    private void InitializeDictionary(string[] englishWords, Dictionary<string, string> dictionary, int evolutionStage)
    {
        for (int i = 0; i < englishWords.Length; i++)
        {
            string trimmedEnglishWord = englishWords[i].Trim();

            string fictionalWord;
            switch (evolutionStage)
            {
                case 2: // Neanderthal
                    fictionalWord = CreateWordByIndex(i, trimmedEnglishWord.Length / 2);
                    break;
                case 3: // MidSapien
                    fictionalWord = CreateMidSapienWord(trimmedEnglishWord);
                    break;
                case 4: // Sapien
                    fictionalWord = CreateSapienWord(trimmedEnglishWord);
                    break;
                default:
                    fictionalWord = trimmedEnglishWord;
                    break;
            }

            dictionary[trimmedEnglishWord] = fictionalWord;
        }
    }

    private string CreateMidSapienWord(string englishWord)
    {
        // Take the first two characters
        string prefix = englishWord.Substring(0, Math.Min(2, englishWord.Length));

        // Take the last two characters
        string suffix = englishWord.Length > 2 ? englishWord.Substring(englishWord.Length - 2, 2) : "";

        int middleSyllableCount = englishWord.Length > 4 ? englishWord.Length / 3 : 1;
        string middle = CreateWordByIndex(englishWord.GetHashCode(), middleSyllableCount);

        return prefix + middle + suffix;
    }

    private string CreateSapienWord(string englishWord)
    {
        return englishWord;
    }

    private string CreateWordByIndex(int index, int syllableCount)
    {
        // Use the index as the seed for consistent random values.
        Random.InitState(index);

        string word = "";
        for (int i = 0; i < syllableCount; i++)
        {
            Consonants consonant = (Consonants)Random.Range(0, Enum.GetNames(typeof(Consonants)).Length);
            Vowels vowel = (Vowels)Random.Range(0, Enum.GetNames(typeof(Vowels)).Length);
            word += CreateSyllable(consonant, vowel);
        }

        return word;
    }

    public string TranslateToNeanderthal(string englishInput)
    {
        return Translate(englishInput, EnglishToNeanderthalDictionary, NeanderthalGrammarRules);
    }

    public string TranslateToMidSapien(string englishInput)
    {
        return Translate(englishInput, EnglishToMidSapienDictionary, MidSapienGrammarRules);
    }

    public string TranslateToSapien(string englishInput)
    {
        return Translate(englishInput, EnglishToSapienDictionary, SapienGrammarRules);
    }

    private string HandleUnrecognizedWord(string token)
    {
        // For unrecognized words, add a default prefix for now. 
        return "xeno_" + token;
    }

    private string SentenceCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }

    public string Translate(string englishInput, Dictionary<string, string> dictionary, Func<string, string, string> grammarRules)
    {
        string[] tokens = TokenizeEnglish(englishInput);
        StringBuilder output = new StringBuilder();

        bool isFirstWord = true;
        foreach (string token in tokens)
        {
            // Check if the token is whitespace or punctuation
            if (string.IsNullOrWhiteSpace(token) || ".,!?;:-".Contains(token))
            {
                output.Append(token);
                continue; // Skip further processing for this token
            }

            string translatedWordNeanderthal, translatedWordMidSapien, translatedWordSapien;
            dictionary.TryGetValue(token, out translatedWordNeanderthal); // Might be null, be cautious when using
            EnglishToMidSapienDictionary.TryGetValue(token, out translatedWordMidSapien); // Might be null, be cautious when using
            EnglishToSapienDictionary.TryGetValue(token, out translatedWordSapien); // Might be null, be cautious when using

            string blendedWord = BlendWords(translatedWordNeanderthal ?? "", translatedWordMidSapien ?? "", translatedWordSapien ?? "");

            if (isFirstWord)
            {
                isFirstWord = false; // Reset it immediately as all other checks depend on it
                if (!string.IsNullOrEmpty(blendedWord))
                {
                    // Apply grammar rules based on provided rules function
                    blendedWord = grammarRules(token, blendedWord);
                    output.Append(SentenceCase(blendedWord));
                }
                else
                {
                    output.Append(SentenceCase(HandleUnrecognizedWord(token)));
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(blendedWord))
                {
                    // Apply grammar rules based on provided rules function
                    blendedWord = grammarRules(token, blendedWord);
                    output.Append(" " + blendedWord);
                }
                else
                {
                    output.Append(" " + HandleUnrecognizedWord(token));
                }
            }
        }

        return output.ToString();
    }

    public string TranslateByEvolutionFactor(string englishInput)
    {
        string neanderthalWord = TranslateToNeanderthal(englishInput);
        string midSapienWord = TranslateToMidSapien(englishInput);
        string sapienWord = TranslateToSapien(englishInput);

        return BlendWords(neanderthalWord, midSapienWord, sapienWord);
    }

    private string BlendWords(string neanderthalWord, string midSapienWord, string sapienWord)
    {
        if (languageEvolution <= 0.33f)
        {
            float localFraction = languageEvolution / 0.33f;  // Normalize to range [0,1]
            return StringLerp(neanderthalWord, midSapienWord, localFraction);
        }
        else if (languageEvolution <= 0.66f)
        {
            float localFraction = (languageEvolution - 0.33f) / 0.33f;  // Normalize to range [0,1]
            return StringLerp(midSapienWord, sapienWord, localFraction);
        }
        else
        {
            float localFraction = (languageEvolution - 0.66f) / 0.33f;  // Normalize to range [0,1]
            return StringLerp(midSapienWord, sapienWord, localFraction);
        }
    }


    // Interpolates two strings based on fraction
    private string StringLerp(string a, string b, float fraction)
    {
        int sharedLength = Mathf.Min(a.Length, b.Length);
        string result = "";

        // Interpolate over the shared length
        for (int i = 0; i < sharedLength; i++)
        {
            result += Random.Range(0f, 1f) < fraction ? b[i] : a[i];
        }

        // Append the remaining characters from the longer string
        if (a.Length > sharedLength)
        {
            result += a.Substring(sharedLength);
        }
        else if (b.Length > sharedLength)
        {
            result += b.Substring(sharedLength);
        }

        return result;
    }


    private string NeanderthalGrammarRules(string englishWord, string neanderthalWord)
    {
        if (englishWord.EndsWith("s") && englishWord != "is")
        {
            neanderthalWord += "ru";
        }

        return neanderthalWord;
    }

    private string MidSapienGrammarRules(string englishWord, string midSapienWord)
    {
        return midSapienWord;
    }

    private string SapienGrammarRules(string englishWord, string sapienWord)
    {

        return sapienWord;
    }

    private string CreateSyllable(Consonants consonant, Vowels vowel)
    {
        return consonant.ToString().ToLower() + vowel.ToString().ToLower();
    }

    private string[] TokenizeEnglish(string englishInput)
    {
        // Split by spaces and punctuation, but include the punctuation in the result
        var tokens = System.Text.RegularExpressions.Regex.Split(englishInput, @"(?<=[.!?,:;\-\s])|(?=[.!?,:;\-\s])");

        // Filter out whitespace tokens
        return tokens.Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();
    }

    public void OnTranslateButtonPressed(string englishInput)
    {
        string translatedText = TranslateToNeanderthal(englishInput);
        Debug.Log(translatedText);
    }

    public void TranslateAndSpeak(string englishInput)
    {
        string translatedText = TranslateByEvolutionFactor(englishInput);
        Debug.Log(translatedText);

        formantSynth.Speak(translatedText);
    }
}
