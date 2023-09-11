using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine; // Needed for Application.dataPath
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Qkmaxware.Phonetics
{
    public class IPA
    {
        private Dictionary<string, string> dictionary;
        private Dictionary<string, IPASymbol> ipaSymbols = new Dictionary<string, IPASymbol>();

        public class IPASymbol
        {
            public string Symbol { get; set; }
            public IPACategory Category { get; set; }
            public int VolumeDuck { get; set; }
            public int TimeModification { get; set; }
            public int PitchAdjustment { get; set; }
            public int VowelQuality { get; set; }

            // Formants
            public int F1 { get; set; } = 0; // default to 0
            public int F2 { get; set; } = 0; // default to 0
            public int F3 { get; set; } = 0; // default to 0
            public int F4 { get; set; } = 0; // default to 0
            public int F5 { get; set; } = 0; // default to 0

            // ... Other properties and methods for the symbol
        }


        public enum IPACategory
        {
            Vowel,
            Consonant,
            Diacritic,
            Suprasegmental,
            Unrecognized
        }

        public HashSet<string> vowels = new HashSet<string>
        {
            "a", "e", "i", "o", "u", "ɑ", "æ", "ɛ", "ɚ", "ɪ", "ɔ", "ʊ", "ʌ"
        };

        public HashSet<string> diacritics = new HashSet<string>
        {
            "̩", "̯", "̪", "̺", "̻",
            "̼", "̟", "̠", "˔", "˕",
            "̤", "̰", "̝", "̞", "̘",
            "̙", "˞", "̜", "̹", "̈",
            "̽", "˕", "˔", "˗", "˓",
            "˖", "˚", "ʼ", "̚", "˳",
            "̴", "̘", "̻", "̏", "ˑ", "ˤ"
        };

        public HashSet<string> suprasegmentals = new HashSet<string>
        {
            "ˈ", "ˌ", "ː", "|", "‖",
            ".", "↘", "↗", "ʍ"
        };

        public HashSet<string> consonants = new HashSet<string>
        {
            "p", "b", "t", "d", "ʈ", "ɖ", "c", "ɟ", "k", "ɡ", "q", "ɢ", "ʔ",
            "m", "ɱ", "n", "ɳ", "ɲ", "ŋ", "ɴ",
            "ʙ", "r", "ʀ",
            "ⱱ", "ɾ", "ɽ",
            "ɸ", "β", "f", "v", "θ", "ð", "s", "z", "ʃ", "ʒ", "ʂ", "ʐ", "ç", "ʝ", "x", "ɣ", "χ", "ʁ", "ħ", "ʕ", "h", "ɦ",
            "ɹ", "ɻ", "j", "ɰ",
            "l", "ɭ", "ʎ", "ʟ",
            "ɬ", "ɮ",
            "ɺ",
            "ʋ", "ɥ",
            "ʦ", "ʣ", "ʧ", "ʤ", "ʨ", "ʥ"
        };

        public HashSet<string> clicks = new HashSet<string>
        {
            "ʘ",
            "ǀ",
            "ǃ",
            "ǂ",
            "ǁ"
        };

        public HashSet<string> implosives = new HashSet<string>
        {
            "ɓ",
            "ɗ",
            "ʄ",
            "ɠ",
            "ʛ"
        };

        public HashSet<string> ejectives = new HashSet<string>
        {
            "pʼ", "tʼ", "kʼ", "qʼ", "sʼ", "ʃʼ", "cʼ", "ʧʼ", "ʦʼ", "χʼ"
        };


        private HashSet<string> unrecognizedSymbols = new HashSet<string>();

        public IPA()
        {
            this.dictionary = new Dictionary<string, string>();

            // Change this path to point to the StreamingAssets folder
            string sourceFilePath = Path.Combine(Application.streamingAssetsPath, "CMU.in.IPA.txt");

            // Destination file path
            string destinationFilePath = Path.Combine(Application.persistentDataPath, "CMU.in.IPA.txt");

            // Check if the file already exists in the destination, only copy if it doesn't
            if (!File.Exists(destinationFilePath))
            {
                File.Copy(sourceFilePath, destinationFilePath);
            }

            // Now read from the persistent data path
            foreach (string line in File.ReadAllLines(destinationFilePath))
            {
                var parts = line.Split(',', 2);
                if (parts.Length == 2)
                {
                    dictionary[parts[0].Trim()] = parts[1].Trim();
                    ExtractSymbols(parts[1].Trim());
                }
            }

            WriteSymbolsToFile();
            GenerateFormantFile();
        }



        private void ExtractSymbols(string transcription)
        {
            int i = 0;
            while (i < transcription.Length)
            {
                bool symbolFound = false;
                int windowSize = 2; // assuming 2 is the maximum symbol length, adjust if needed
                while (windowSize > 0)
                {
                    if (i + windowSize <= transcription.Length) // Ensure we don't go out of bounds
                    {
                        string currentSubstring = transcription.Substring(i, windowSize);
                        if (!char.IsWhiteSpace(currentSubstring[0]) && ipaSymbols.ContainsKey(currentSubstring))
                        {
                            var symbol = new IPASymbol { Symbol = currentSubstring };
                            AssignCategory(symbol); // Refactored category assignment into a method for cleanliness
                            ipaSymbols[currentSubstring] = symbol;
                            i += windowSize; // jump ahead by window size
                            symbolFound = true;
                            break;
                        }
                    }
                    windowSize--;
                }

                if (!symbolFound)
                {
                    string charStr = transcription[i].ToString();
                    if (!char.IsWhiteSpace(transcription[i]) && !ipaSymbols.ContainsKey(charStr))
                    {
                        var symbol = new IPASymbol { Symbol = charStr };
                        AssignCategory(symbol);
                        ipaSymbols[charStr] = symbol;
                    }
                    else if (!char.IsWhiteSpace(transcription[i])) // if the symbol is not whitespace and not in ipaSymbols
                    {
                        unrecognizedSymbols.Add(charStr); // add the unrecognized symbol to the set
                    }
                    i++;
                }
            }
        }

        private void WriteSymbolsToFile()
        {
            string outputFilePath = Path.Combine(Application.persistentDataPath, "IPAindex.json");

            // Sort the symbols by category
            var sortedIpaSymbols = ipaSymbols
                .OrderBy(item => item.Value.Category)
                .ToDictionary(item => item.Key, item => item.Value);

            var jsonContent = JsonConvert.SerializeObject(sortedIpaSymbols, new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
            {
                new StringEnumConverter()
            },
                Formatting = Formatting.Indented
            });

            File.WriteAllText(outputFilePath, jsonContent);
        }

        public void GenerateFormantFile()
        {
            string formantFilePath = Path.Combine(Application.persistentDataPath, "IPAFormants.txt");

            // Read the JSON file and deserialize it back into a dictionary
            string inputFilePath = Path.Combine(Application.persistentDataPath, "IPAindex.json");
            if (!File.Exists(inputFilePath))
            {
                throw new FileNotFoundException($"The file {inputFilePath} was not found.");
            }

            string jsonContent = File.ReadAllText(inputFilePath);
            Dictionary<string, IPASymbol> deserializedIpaSymbols = JsonConvert.DeserializeObject<Dictionary<string, IPASymbol>>(jsonContent);

            List<string> lines = new List<string>();
            foreach (var entry in deserializedIpaSymbols)
            {
                string symbolStr = entry.Value.Symbol;
                if (formantDictionary.ContainsKey(symbolStr))
                {
                    var line = $"{symbolStr}: {string.Join(",", formantDictionary[symbolStr])}";
                    lines.Add(line);
                }
            }

            File.WriteAllLines(formantFilePath, lines);
        }

        public List<int[]> GetFormants(string ipaString)
        {
            List<int[]> formantFrequencies = new List<int[]>();

            for (int i = 0; i < ipaString.Length; i++)
            {
                char currentChar = ipaString[i];
                string potentialDiphthong = (i < ipaString.Length - 1) ? currentChar.ToString() + ipaString[i + 1] : null;

                bool found = false;

                foreach (var category in Formants)
                {
                    var soundTypeDictionary = category.Value;

                    // Check for diphthongs first
                    if (potentialDiphthong != null && soundTypeDictionary.ContainsKey(potentialDiphthong))
                    {
                        formantFrequencies.Add(soundTypeDictionary[potentialDiphthong]);
                        i++; // Skip next character
                        found = true;
                        break; // No need to check further
                    }

                    // Check for single character symbols
                    string currentSymbol = currentChar.ToString();
                    if (soundTypeDictionary.ContainsKey(currentSymbol))
                    {
                        formantFrequencies.Add(soundTypeDictionary[currentSymbol]);
                        found = true;
                        break; // No need to check further
                    }
                }

                // If the symbol wasn't found in any category, you can decide whether you want to handle it in a special way or ignore it.
                if (!found)
                {
                    // Handle unknown symbol if needed
                }
            }

            return formantFrequencies;
        }


        public static Dictionary<string, Dictionary<string, int[]>> Formants = new Dictionary<string, Dictionary<string, int[]>>
        {
            // Monophthongal vowels
            ["Monophthongal vowels"] = new Dictionary<string, int[]>
            {
                {"i", new int[] {270, 2290, 3010}},
                {"ɪ", new int[] {400, 2200, 2900}},
                {"e", new int[] {400, 2300, 3000}},
                {"ɛ", new int[] {600, 1800, 2500}},
                {"æ", new int[] {650, 1700, 2400}},
                {"ɑ", new int[] {700, 1150, 2600}},
                {"ɔ", new int[] {500, 1000, 2500}},
                {"o", new int[] {450, 800, 2600}},
                {"u", new int[] {300, 800, 2400}},
                {"ʊ", new int[] {400, 900, 2500}},
                {"ʌ", new int[] {600, 1200, 2400}},
                {"ɝ", new int[] {500, 1500, 2500}},
                {"ə", new int[] {500, 1500, 2500}}
            },

            // Diphthongs
            ["Diphthongs"] = new Dictionary<string, int[]>
            {
                {"aɪ", new int[] {650, 1850, 2850}},
                {"aʊ", new int[] {750, 1250, 2500}},
                {"oʊ", new int[] {450, 800, 2400}},
                {"eɪ", new int[] {400, 2300, 3000}},
                {"ɔɪ", new int[] {490, 1250, 2390}}
            },

            // Plosives (stops)
            ["Plosives"] = new Dictionary<string, int[]>
            {
                {"p", new int[] {900, 2200, 3450}},
                {"b", new int[] {700, 2100, 3400}},
                {"t", new int[] {1650, 4200, 5500}},
                {"d", new int[] {1600, 4000, 5200}},
                {"k", new int[] {1400, 3800, 6000}},
                {"g", new int[] {1100, 3000, 4000}}
            },

            // Affricates
            ["Affricates"] = new Dictionary<string, int[]>
            {
                {"ʧ", new int[] {2300, 3400, 5000}},
                {"ʤ", new int[] {2100, 2900, 4400}}
            },

            // Fricatives
            ["Fricatives"] = new Dictionary<string, int[]>
            {
                {"f", new int[] {760, 1300, 3000}},
                {"v", new int[] {570, 840, 2410}},
                {"θ", new int[] {1800, 2600, 3400}},
                {"ð", new int[] {1400, 2050, 2850}},
                {"s", new int[] {2200, 5900, 8100}},
                {"z", new int[] {2400, 4600, 6500}},
                {"ʃ", new int[] {2300, 3900, 5800}},
                {"ʒ", new int[] {2200, 3700, 5400}}
            },

            // Nasals
            ["Nasals"] = new Dictionary<string, int[]>
            {
                {"m", new int[] {250, 2300, 3300}},
                {"n", new int[] {250, 1750, 2500}},
                {"ŋ", new int[] {450, 1500, 2100}}
            },

            // Liquids and glides
            ["Liquids and glides"] = new Dictionary<string, int[]>
        {
                {"l", new int[] {450, 1450, 2600}},
                {"ɹ", new int[] {300, 1400, 2600}},
                {"j", new int[] {250, 2200, 3000}},
                {"w", new int[] {300, 900, 2500}}
            },

            // Approximants
            ["Approximants"] = new Dictionary<string, int[]>
            {
                {"ɻ", new int[] {450, 1800, 2400}},  // retroflex approximant
                {"ɰ", new int[] {300, 2200, 3400}}  // voiced velar approximant
            }
        };



        private int ExtractFormantValue(string formantsStr, string formantName)
        {
            string pattern = $"{formantName}: (\\d+)";
            Match match = Regex.Match(formantsStr, pattern);
            if (match.Success && match.Groups.Count > 1)
            {
                string valueStr = match.Groups[1].Value;
                if (int.TryParse(valueStr, out int value))
                {
                    return value;
                }
            }
            return 0; // Default value if not found or not a valid integer
        }

        private void AssignCategory(IPASymbol symbol)
        {
            string charStr = symbol.Symbol;

            symbol.Category = charStr switch
            {
                var s when vowels.Contains(s) => IPACategory.Vowel,
                var s when diacritics.Contains(s) => IPACategory.Diacritic,
                var s when suprasegmentals.Contains(s) => IPACategory.Suprasegmental,
                var s when consonants.Contains(s) => IPACategory.Consonant,
                var s when clicks.Contains(s) => IPACategory.Consonant, // classify clicks as consonants
                var s when implosives.Contains(s) => IPACategory.Consonant, // classify implosives as consonants
                var s when ejectives.Contains(s) => IPACategory.Consonant, // classify ejectives as consonants
                _ => IPACategory.Unrecognized
            };

            if (symbol.Category == IPACategory.Unrecognized)
            {
                Debug.LogWarning($"Unrecognized symbol: {charStr}");
            }
        }

        public string EnglishToIPA(string text)
        {
            var builder = new StringBuilder();
            string[] words = Regex.Split(text, @"([\s\p{P}])"); // Split on spaces or punctuation

            foreach (var match in words)
            {
                var lower = match.ToLower();
                if (dictionary.ContainsKey(lower))
                {
                    builder.Append(dictionary[lower]);
                }
                else
                {
                    builder.Append(lower);
                }
            }
            return builder.ToString();
        }

        private Dictionary<string, string[]> formantDictionary = new Dictionary<string, string[]>
        {
            { "a", new[] { "800", "1300", "2800" } },   // Mock values for example
            { "ɑ", new[] { "700", "1150", "2600" } },   // General American 'father'
            { "æ", new[] { "650", "1700", "2400" } },   // General American 'cat'
            { "e", new[] { "400", "2300", "3000" } },   // Close to 'face'
            { "ɛ", new[] { "600", "1800", "2500" } },   // General American 'bet'
            { "ɚ", new[] { "500", "1500", "2500" } },   // R-colored 'nurse', this is a rough estimate
            { "i", new[] { "300", "2400", "3000" } },   // General American 'fleece'
            { "ɪ", new[] { "400", "2200", "2900" } },   // General American 'kit'
            { "o", new[] { "450", "800", "2600" } },    // Close to 'goat'
            { "ɔ", new[] { "500", "1000", "2500" } },   // General American 'thought'
            { "u", new[] { "300", "800", "2400" } },    // General American 'goose'
            { "ʊ", new[] { "400", "900", "2500" } },    // General American 'foot'
            { "ʌ", new[] { "600", "1200", "2400" } }    // General American 'strut'
        };
    }
}
