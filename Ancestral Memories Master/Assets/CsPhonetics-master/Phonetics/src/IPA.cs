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
    public class IPA : JsonConverter
    {
        private Dictionary<string, string> dictionary;
        private Dictionary<string, IPASymbol> ipaSymbols = new Dictionary<string, IPASymbol>();

        private static readonly string ipaIndexRawPath = Path.Combine(Application.dataPath, "LanguageGen", "CharResources");

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

        private HashSet<string> vowels = new HashSet<string>
        {
            "a", "e", "i", "o", "u", "ɑ", "æ", "ɛ", "ɚ", "ɪ", "ɔ", "ʊ", "ʌ"
        };

        private HashSet<string> diacritics = new HashSet<string>
        {
            "̩", "̯", "̪", "̺", "̻",
            "̼", "̟", "̠", "˔", "˕",
            "̤", "̰", "̝", "̞", "̘",
            "̙", "˞", "̜", "̹", "̈",
            "̽", "˕", "˔", "˗", "˓",
            "˖", "˚", "ʼ", "̚", "˳",
            "̴", "̘", "̻", "̏", "ˑ", "ˤ"
        };

        private HashSet<string> suprasegmentals = new HashSet<string>
        {
            "ˈ", "ˌ", "ː", "|", "‖",
            ".", "↘", "↗", "ʍ"
        };

        private HashSet<string> consonants = new HashSet<string>
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

        private HashSet<string> clicks = new HashSet<string>
        {
            "ʘ",
            "ǀ",
            "ǃ",
            "ǂ",
            "ǁ"
        };

        private HashSet<string> implosives = new HashSet<string>
        {
            "ɓ",
            "ɗ",
            "ʄ",
            "ɠ",
            "ʛ"
        };

        private HashSet<string> ejectives = new HashSet<string>
        {
            "pʼ", "tʼ", "kʼ", "qʼ", "sʼ", "ʃʼ", "cʼ", "ʧʼ", "ʦʼ", "χʼ"
        };


        private HashSet<string> unrecognizedSymbols = new HashSet<string>();

        public IPA()
        {
            this.dictionary = new Dictionary<string, string>();

            string filePath = Path.Combine(Application.dataPath, "CsPhonetics-master", "Phonetics", "data", "CMU.in.IPA.txt");

            foreach (string line in File.ReadAllLines(filePath))
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
            string outputFilePath = Path.Combine(ipaIndexRawPath, "IPAindex.json");

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
            string formantFilePath = Path.Combine(ipaIndexRawPath, "IPAFormants.txt");

            // Read the JSON file and deserialize it back into a dictionary
            string inputFilePath = Path.Combine(ipaIndexRawPath, "IPAindex.json");
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

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IPASymbol symbol = (IPASymbol)value;

            switch (symbol.Category)
            {
                case IPACategory.Consonant:
                    symbol.VolumeDuck = 1;
                    symbol.TimeModification = 2;
                    symbol.PitchAdjustment = 3;
                    symbol.VowelQuality = 0; // Consonants don't have vowel quality
                    symbol.F1 = 1000; 
                    symbol.F2 = 2000; 
                    symbol.F3 = 3000; 
                    symbol.F4 = 0;
                    symbol.F5 = 0; 
                    break;

                case IPACategory.Vowel:
                    symbol.VolumeDuck = 5;
                    symbol.TimeModification = 6;
                    symbol.PitchAdjustment = 7;
                    symbol.VowelQuality = 10;
                    symbol.F1 = 500;
                    symbol.F2 = 1500;
                    symbol.F3 = 2500; 
                    symbol.F4 = 3500; 
                    symbol.F5 = 4500; 
                    break;

                case IPACategory.Diacritic:
                    symbol.VolumeDuck = 0; 
                    symbol.TimeModification = 0; 
                    symbol.PitchAdjustment = 0; 
                    symbol.VowelQuality = 0; 
                    symbol.F1 = 0; 
                    symbol.F2 = 0; 
                    symbol.F3 = 0; 
                    symbol.F4 = 0; 
                    symbol.F5 = 0; 
                    break;

                case IPACategory.Suprasegmental:
                    symbol.VolumeDuck = 2; 
                    symbol.TimeModification = 1; 
                    symbol.PitchAdjustment = 1; 
                    symbol.VowelQuality = 0; // Suprasegmentals don't have vowel quality
                    symbol.F1 = 1000; 
                    symbol.F2 = 2000; 
                    symbol.F3 = 3000; 
                    symbol.F4 = 0;
                    symbol.F5 = 0; 
                    break;

                case IPACategory.Unrecognized:
                    symbol.VolumeDuck = 0;
                    symbol.TimeModification = 0;
                    symbol.PitchAdjustment = 0;
                    symbol.VowelQuality = 0;
                    symbol.F1 = 0; 
                    symbol.F2 = 0;
                    symbol.F3 = 0; 
                    symbol.F4 = 0; 
                    symbol.F5 = 0;
                    break;

                default:
                    break;
            }


            JObject symbolObject = new JObject
            {
                ["Symbol"] = symbol.Symbol,
                ["Category"] = JToken.FromObject(symbol.Category, serializer),
                ["VolumeDuck"] = symbol.VolumeDuck,
                ["TimeModification"] = symbol.TimeModification,
                ["PitchAdjustment"] = symbol.PitchAdjustment,
                ["VowelQuality"] = symbol.VowelQuality,
                ["Formants"] = $"F1: {symbol.F1}, F2: {symbol.F2}, F3: {symbol.F3}, F4: {symbol.F4}, F5: {symbol.F5}"
            };

            symbolObject.WriteTo(writer);
        }


        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject jsonObject = JObject.Load(reader);

                string symbol = jsonObject["Symbol"].ToString();
                IPACategory category = jsonObject["Category"].ToObject<IPACategory>();
                int volumeDuck = (int)jsonObject["VolumeDuck"];
                int timeModification = (int)jsonObject["TimeModification"];
                int pitchAdjustment = (int)jsonObject["PitchAdjustment"];
                int vowelQuality = (int)jsonObject["VowelQuality"];

                // Extract formants
                string formantsStr = jsonObject["Formants"].ToString();
                int f1 = ExtractFormantValue(formantsStr, "F1");
                int f2 = ExtractFormantValue(formantsStr, "F2");
                int f3 = ExtractFormantValue(formantsStr, "F3");
                int f4 = ExtractFormantValue(formantsStr, "F4");
                int f5 = ExtractFormantValue(formantsStr, "F5");

                // Create and return IPASymbol object
                IPASymbol ipaSymbol = new IPASymbol
                {
                    Symbol = symbol,
                    Category = category,
                    VolumeDuck = volumeDuck,
                    TimeModification = timeModification,
                    PitchAdjustment = pitchAdjustment,
                    VowelQuality = vowelQuality,
                    F1 = f1,
                    F2 = f2,
                    F3 = f3,
                    F4 = f4,
                    F5 = f5
                };

                return ipaSymbol;
            }

            throw new JsonSerializationException("Unexpected token type: " + reader.TokenType);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPASymbol);
        }
    }
}
