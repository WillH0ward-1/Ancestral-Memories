using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class LanguageDictionary : MonoBehaviour
{

    private VocabularyManager vocabularyManager;

    public Dictionary<string, string> EnglishToNeanderthalDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> EnglishToMidSapienDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> EnglishToSapienDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

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

    void Start()
    {
        vocabularyManager = FindObjectOfType<VocabularyManager>();

        string[] englishWords = vocabularyManager.Vocabulary.ToArray();

        InitializeDictionary(englishWords, EnglishToNeanderthalDictionary, 2);
        InitializeDictionary(englishWords, EnglishToMidSapienDictionary, 3);
        InitializeDictionary(englishWords, EnglishToSapienDictionary, 4);
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

    private string CreateSyllable(Consonants consonant, Vowels vowel)
    {
        return consonant.ToString().ToLower() + vowel.ToString().ToLower();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
