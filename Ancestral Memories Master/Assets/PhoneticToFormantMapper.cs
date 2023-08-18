using System.Collections.Generic;
using Phonix;

public class PhoneticToFormantMapper
{
    private readonly DoubleMetaphone _generator = new DoubleMetaphone();

    // A mock-up dictionary. You'd fill this with real data.
    private Dictionary<string, (double F1, double F2, double F3)> phonemeToFormant = new Dictionary<string, (double, double, double)>
    {
        // For example:
        { "CH", (2700, 2290, 3010) },
        // Add mappings for other phonemes
    };

    public Dictionary<string, (double F1, double F2, double F3)[]> MapPhoneticToFormant(List<string> vocabulary)
    {
        Dictionary<string, (double F1, double F2, double F3)[]> wordToFormantMapping = new Dictionary<string, (double, double, double)[]>();

        foreach (var word in vocabulary)
        {
            var phoneticKeys = _generator.BuildKeys(word);
            var formantList = new List<(double, double, double)>();

            foreach (var key in phoneticKeys)
            {
                if (phonemeToFormant.TryGetValue(key, out var formant))
                {
                    formantList.Add(formant);
                }
            }

            wordToFormantMapping[word] = formantList.ToArray();
        }

        return wordToFormantMapping;
    }

    public void SavePhoneticToFormantMappingToFile(Dictionary<string, (double F1, double F2, double F3)[]> mapping)
    {
        // Your logic for saving this mapping to a file
    }
}
