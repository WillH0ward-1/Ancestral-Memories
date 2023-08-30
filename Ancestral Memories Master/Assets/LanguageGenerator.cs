using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using System.Text;

public class LanguageGenerator : MonoBehaviour
{
    private LanguageDictionary languageDictionary;

    private FormantSynthesizer formantSynth;

    private AICharacterStats characterStats;

    public float languageEvolution;

    private void Start()
    {
        characterStats = GetComponent<AICharacterStats>();
        formantSynth = GetComponent<FormantSynthesizer>();
        languageDictionary = FindObjectOfType<LanguageDictionary>();

        // Read the file once and initialize dictionaries

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

    public string TranslateToNeanderthal(string englishInput)
    {
        return Translate(englishInput, languageDictionary.EnglishToNeanderthalDictionary, NeanderthalGrammarRules);
    }

    public string TranslateToMidSapien(string englishInput)
    {
        return Translate(englishInput, languageDictionary.EnglishToMidSapienDictionary, MidSapienGrammarRules);
    }

    public string TranslateToSapien(string englishInput)
    {
        return Translate(englishInput, languageDictionary.EnglishToSapienDictionary, SapienGrammarRules);
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
            languageDictionary.EnglishToMidSapienDictionary.TryGetValue(token, out translatedWordMidSapien); // Might be null, be cautious when using
            languageDictionary.EnglishToSapienDictionary.TryGetValue(token, out translatedWordSapien); // Might be null, be cautious when using

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

    private string[] TokenizeEnglish(string englishInput)
    {
        // Split by spaces and punctuation, but include the punctuation in the result
        var tokens = System.Text.RegularExpressions.Regex.Split(englishInput, @"(?<=[.!?,:;\-\s])|(?=[.!?,:;\-\s])");

        // Filter out whitespace tokens
        return tokens.Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();
    }

    public void TranslateAndSpeak(string englishInput)
    {
        string translatedText = TranslateByEvolutionFactor(englishInput);
        Debug.Log(translatedText);

        formantSynth.Speak(translatedText);
    }
}
