using UnityEngine;

public class LanguageGenerator : MonoBehaviour
{
    public float languageEvolution = 0.5f;  // Ranges from 0 to 1
    [SerializeField] private LanguageGeneratorManager languageGeneratorManager;
    [SerializeField] private FormantSynthesizer formantSynth;
    [SerializeField] private AICharacterStats stats;
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private string humanTag = "Human";

    private void Start()
    {
        // Get the LanguageGeneratorManager instance
        languageGeneratorManager = FindObjectOfType<LanguageGeneratorManager>();
        languageGeneratorManager.languageGenerator = this;
        dialogue = GetComponentInChildren<Dialogue>();
        dialogue.languageGeneratorManager = languageGeneratorManager;
        // Initialize FormantSynthesizer (if needed)
        formantSynth = GetComponentInChildren<FormantSynthesizer>();

        if (transform.CompareTag(humanTag))
        {
            stats = GetComponent<AICharacterStats>();
        }
        
    }

    private void Update()
    {
        if (stats != null)
        {
            languageEvolution = stats.evolution;
        }
    }

    public string TranslateByEvolutionFactor(string englishInput)
    {
        string neanderthalWord = languageGeneratorManager.TranslateToNeanderthal(englishInput);
        string midSapienWord = languageGeneratorManager.TranslateToMidSapien(englishInput);
        string sapienWord = languageGeneratorManager.TranslateToSapien(englishInput);

        return BlendWords(neanderthalWord, midSapienWord, sapienWord);
    }

    public string BlendWords(string neanderthalWord, string midSapienWord, string sapienWord)
    {
        if (languageEvolution <= 0.33f)
        {
            float localFraction = languageEvolution / 0.33f;  // Normalize to range [0,1]
            return languageGeneratorManager.StringLerp(neanderthalWord, midSapienWord, localFraction);
        }
        else if (languageEvolution <= 0.66f)
        {
            float localFraction = (languageEvolution - 0.33f) / 0.33f;  // Normalize to range [0,1]
            return languageGeneratorManager.StringLerp(midSapienWord, sapienWord, localFraction);
        }
        else
        {
            float localFraction = (languageEvolution - 0.66f) / 0.33f;  // Normalize to range [0,1]
            return languageGeneratorManager.StringLerp(midSapienWord, sapienWord, localFraction);
        }
    }

    public void TranslateAndSpeak(string englishInput)
    {
        string translatedText = TranslateByEvolutionFactor(englishInput);
        Debug.Log(translatedText);

        //formantSynth.Speak(translatedText);
    }
}
