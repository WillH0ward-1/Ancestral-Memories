using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class LanguageGenerator : MonoBehaviour
{
    public enum Consonants { P, B, T, D, K, G, F, V, S, Z, SH, ZH, M, N, NG }
    public enum Vowels { A, E, I, O, U }

    private Dictionary<string, string> EnglishToNeanderthalDictionary = new Dictionary<string, string>();
    private Dictionary<string, string> EnglishToMidSapienDictionary = new Dictionary<string, string>();
    private Dictionary<string, string> EnglishToSapienDictionary = new Dictionary<string, string>();

    private AICharacterStats characterStats;

    private void Start()
    {
        InitializeDictionary(EnglishToNeanderthalDictionary, 2);
        InitializeDictionary(EnglishToMidSapienDictionary, 3);
        InitializeDictionary(EnglishToSapienDictionary, 4);
    }

    private void InitializeDictionary(Dictionary<string, string> dictionary, int syllableMultiplier)
    {
        string path = Path.Combine(Application.dataPath, "EveryWord.txt");
        string[] englishWords = File.ReadAllLines(path);

        foreach (string englishWord in englishWords)
        {
            string trimmedEnglishWord = englishWord.Trim();
            string fictionalWord = CreateWord(trimmedEnglishWord.Length / syllableMultiplier);
            dictionary[trimmedEnglishWord] = fictionalWord;
        }
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

    public float evolutionFraction;

    public string Translate(string englishInput, Dictionary<string, string> dictionary, Func<string, string, string> grammarRules)
    {
        string[] tokens = TokenizeEnglish(englishInput.ToLower());
        string output = "";

        foreach (string token in tokens)
        {
            if (EnglishToNeanderthalDictionary.TryGetValue(token, out string neanderthalWord) &&
                EnglishToMidSapienDictionary.TryGetValue(token, out string midSapienWord) &&
                EnglishToSapienDictionary.TryGetValue(token, out string sapienWord))
            {
                // Blend words based on evolutionary fraction
                string blendedWord = BlendWords(neanderthalWord, midSapienWord, sapienWord);

                // Apply grammar rules based on provided rules function
                blendedWord = grammarRules(token, blendedWord);

                output += blendedWord + " ";
            }
            else
            {
                output += CreateWord(token.Length / 3) + " "; // Handle unknown words
            }
        }

        return output.Trim();
    }

    private string BlendWords(string neanderthalWord, string midSapienWord, string sapienWord)
    {
        // Linear interpolation between words based on evolutionary fraction
        if (evolutionFraction < 0.5f)
        {
            float localFraction = evolutionFraction * 2; // Normalize to range [0,1]
            return StringLerp(neanderthalWord, midSapienWord, localFraction);
        }
        else
        {
            float localFraction = (evolutionFraction - 0.5f) * 2; // Normalize to range [0,1]
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
        if (englishWord.StartsWith("th"))
        {
            midSapienWord = "th" + midSapienWord;
        }
        return midSapienWord;
    }

    private string SapienGrammarRules(string englishWord, string sapienWord)
    {
        if (englishWord.Contains("ing"))
        {
            sapienWord += "inga";
        }
        return sapienWord;
    }

    private string CreateWord(int syllableCount)
    {
        string word = "";
        for (int i = 0; i < syllableCount; i++)
        {
            Consonants consonant = (Consonants)Random.Range(0, System.Enum.GetNames(typeof(Consonants)).Length);
            Vowels vowel = (Vowels)Random.Range(0, System.Enum.GetNames(typeof(Vowels)).Length);
            word += CreateSyllable(consonant, vowel);
        }
        return word;
    }

    private string CreateSyllable(Consonants consonant, Vowels vowel)
    {
        return consonant.ToString().ToLower() + vowel.ToString().ToLower();
    }

    private string[] TokenizeEnglish(string englishInput)
    {
        return englishInput.Split(' ');
    }

    public void OnTranslateButtonPressed(string englishInput)
    {
        string translatedText = TranslateToNeanderthal(englishInput);
        Debug.Log(translatedText);
    }
}
