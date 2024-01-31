using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Phonix;

public class DialogueLines : MonoBehaviour
{
    public enum CharacterTypes
    {
        Neanderthal,
        MidSapien,
        Sapien,
        Deer,
        Shaman
    }

    public enum CharacterGenders
    {
        Male,
        Female
    }

    public enum Emotions
    {
        Neutral,
        Joy,
        Fear,
        Curiosity,
        Contentment,
        Alertness,
        Sadness,
        Hungry,
        Praise,
        SeasonsSpring,
        SeasonsSummer,
        SeasonsAutumn,
        SeasonsWinter,
        Insane,
        BuildingPrompt,
        ShamanIntroduction,
        ShamanFluteTutorial,
        ShamanFluteTutorialFail,
        ShamanTreeTutorial,
        ShamanHumanTutorial,
        ShamanMushroomTutorial,
        ShamanConclusion
    }

    public List<string> sharedInsaneLines = new List<string>
    {
        "Moon shouts! Loud... too loud!",
        "Stones... they move? They watch?",
        "Fire lies! Cold, not hot!",
        "Clouds... chasing me?",
        "Birds' songs... no, screams!",
        "Why sun run away? Hide?",
        "Water talks back. Angry water!",
        "Wind has face. I see it!"
    };

    public List<string> buildingPromptLines = new List<string>
    {
        "You want make new place?",
        "You think, we make. What need?",
        "Vision talk? We follow.",
        "Together bind branches, stones?",
        "Earth provide, we make?",
        "What spirits have said? We make?",
        "Stones we place. What we make?",
        "Vision you have? What need?"
    };

    public Dictionary<(CharacterTypes, CharacterGenders), Dictionary<Emotions, List<string>>> conversations =
    new Dictionary<(CharacterTypes, CharacterGenders), Dictionary<Emotions, List<string>>>();

    public VocabularyManager vocabularyManager;

    private void Awake()
    {
        InitializeDialogues();

        foreach (CharacterTypes type in Enum.GetValues(typeof(CharacterTypes)))
        {
            foreach (CharacterGenders gender in Enum.GetValues(typeof(CharacterGenders)))
            {
                if (!conversations.ContainsKey((type, gender)))
                {
                    conversations[(type, gender)] = new Dictionary<Emotions, List<string>>();
                }
                conversations[(type, gender)][Emotions.Insane] = new List<string>(sharedInsaneLines);

                conversations[(type, gender)][Emotions.BuildingPrompt] = new List<string>(buildingPromptLines);
            }
        }

        SaveVocabularyToFile();
        SavePhoneticBreakdownToFile();
    }

    public void SaveVocabularyToFile()
    {
        List<string> vocabulary = GetVocabulary();
        vocabularyManager.AddVocabulary(vocabulary);
        vocabularyManager.SaveVocabularyToFile(); // this line saves vocabulary to a file
    }

    private readonly DoubleMetaphone _generator = new DoubleMetaphone();

    public void SavePhoneticBreakdownToFile()
    {
        List<string> vocabulary = GetVocabulary();

        Dictionary<string, string[]> phoneticData = new Dictionary<string, string[]>();
        IPAGenerator ipaGenerator = new IPAGenerator();

        foreach (var word in vocabulary)
        {
            var transcription = ipaGenerator.TranscribeToIPA(word);
            string[] phonemes = SplitIntoPhonemes(transcription); // This method needs to be defined
            phoneticData[word] = phonemes;
        }

        // Save the phonetic breakdown to a file via VocabularyManager
        vocabularyManager.SavePhoneticBreakdownToFile(phoneticData);
    }

    private string[] SplitIntoPhonemes(string transcription)
    {
        // Logic to split the transcription into individual phonemes
        // You might need a sophisticated approach, as simple string split might not work
        // due to combined characters in IPA. For now, a placeholder:
        return transcription.Split(' '); // Assumes phonemes are space-separated
    }


    public List<string> GetVocabulary()
    {
        HashSet<string> vocabulary = new HashSet<string>();

        foreach (var charDialogues in conversations)
        {
            foreach (var emotionDialogues in charDialogues.Value)
            {
                foreach (string dialogue in emotionDialogues.Value)
                {
                    // Split string on space, punctuation etc. and add each word to the hash set.
                    foreach (string word in dialogue.Split(new[] { ' ', '.', '!', '?', ',', ';', ':' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        vocabulary.Add(word.ToLower().Trim()); // convert to lower case to ensure uniqueness regardless of case.
                    }
                }
            }
        }

        List<string> sortedVocabulary = vocabulary.ToList();
        sortedVocabulary.Sort();

        return sortedVocabulary;
    }

    public List<string> GetDialogue(CharacterTypes characterType, CharacterGenders characterGender, Emotions emotion)
    {
        if (emotion == Emotions.BuildingPrompt)
        {
            // If emotion is BuildingPrompt, return a single random line from buildingPromptLines
            int randomIndex = UnityEngine.Random.Range(0, buildingPromptLines.Count);
            return new List<string> { buildingPromptLines[randomIndex] };
        }

        if (conversations.TryGetValue((characterType, characterGender), out var emotionDialogues) &&
            emotionDialogues.TryGetValue(emotion, out var dialogue))
        {
            return dialogue;
        }

        return new List<string> { "No dialogue available for this combination." };
    }


    private void InitializeDialogues()
    {
        // Neanderthal Male
        conversations[(CharacterTypes.Neanderthal, CharacterGenders.Male)] = new Dictionary<Emotions, List<string>>
        {
            { Emotions.Neutral, new List<string> { "Day end. Fire needed.", "Berries good. Eat soon." } },
            { Emotions.Joy, new List<string> { "Little one strong, grow fast.", "Cave warm. Tribe safe." } },
            { Emotions.Fear, new List<string> { "Sky dark. Storm scary.", "Beast noise. Need sharp stick." } },
            { Emotions.Curiosity, new List<string> { "Bright stone in river.", "New tree. Fruit look good." } },
            { Emotions.Contentment, new List<string> { "Tribe safe. Good hunt today.", "Bird sound nice. Sky clear." } },
            { Emotions.Alertness, new List<string> { "Noise near. Protect family!", "Saw something. Ready spear." } },
            { Emotions.Sadness, new List<string> { "Sad. Body feel heavy.", "Miss old leader. He strong." } },
            { Emotions.Hungry, new List<string> { "Stomach growls. Need hunt deer.", "Need food. Apples and mushrooms?" } },
            { Emotions.Praise, new List<string> { "You fast! Good chase.", "Fire warm. You did well." } },
            { Emotions.SeasonsSpring, new List<string> { "New plant. Air fresh.", "Little animals play. Fun watch." } },
            { Emotions.SeasonsSummer, new List<string> { "Sun hot. River cool.", "Berries everywhere. Gather!" } },
            { Emotions.SeasonsAutumn, new List<string> { "Trees orange and red. Look nice.", "Stack food. Winter come." } },
            { Emotions.SeasonsWinter, new List<string> { "Cold! Need big fire.", "Hope sun come back soon." } },
        };
        // Neanderthal Female
        conversations[(CharacterTypes.Neanderthal, CharacterGenders.Female)] = new Dictionary<Emotions, List<string>>
        {
            { Emotions.Neutral, new List<string> { "Sun sets. Night near.", "Berries collected. Ready for eat." } },
            { Emotions.Joy, new List<string> { "Little one learn walk.", "Safe cave, warm fire." } },
            { Emotions.Fear, new List<string> { "Cold wind. Storm come.", "Hear growl in dark." } },
            { Emotions.Curiosity, new List<string> { "See shiny thing in water.", "Strange smell from plant." } },
            { Emotions.Contentment, new List<string> { "Cave safe. All full belly.", "Hear song of bird. Peaceful." } },
            { Emotions.Alertness, new List<string> { "Hear rustle. Maybe hide.", "Shadow move. Be ready." } },
            { Emotions.Sadness, new List<string> { "Sad. Makes body heavy.", "Remember old friend." } },
            { Emotions.Hungry, new List<string> { "Gather apples. Fill bellies.", "Deer tracks. Hunt for meat." } },
            { Emotions.Praise, new List<string> { "You make sharp tool!", "You keep fire alive. Good job!" } },
            { Emotions.SeasonsSpring, new List<string> { "Flower smell nice.", "Hear chirp of baby birds." } },
            { Emotions.SeasonsSummer, new List<string> { "Hot day. Seek shade.", "Time of many fruits." } },
            { Emotions.SeasonsAutumn, new List<string> { "Leaves color change. Pretty.", "Prepare for cold time." } },
            { Emotions.SeasonsWinter, new List<string> { "Wrap in fur. Stay close.", "Hope for early spring." } },
        };
        // MidSapien Male dialogues
        conversations[(CharacterTypes.MidSapien, CharacterGenders.Male)] = new Dictionary<Emotions, List<string>>
        {
            { Emotions.Neutral, new List<string> { "Sky changes colors.", "River murmurs to us." } },
            { Emotions.Joy, new List<string> { "Tribe dances with spirit.", "Today's hunt brings plenty." } },
            { Emotions.Fear, new List<string> { "Dark shadows move.", "Night's sounds bring unease." } },
            { Emotions.Curiosity, new List<string> { "What's beyond those hills?", "Stars hold tales?" } },
            { Emotions.Contentment, new List<string> { "Tribe safe with fire's warmth.", "Stories echo tonight." } },
            { Emotions.Alertness, new List<string> { "Bushes stirred. Be wary.", "Strange scent on the wind." } },
            { Emotions.Sadness, new List<string> { "Tribe grieves for the lost.", "Sacred tree has fallen." } },
            { Emotions.Hungry, new List<string> { "Insides grumbling. Very hungry. Want food.", "Deer meat good. Hunger hurting." } },
            { Emotions.Praise, new List<string> { "Your spear flies true!", "Your song touches souls." } },
            { Emotions.SeasonsSpring, new List<string> { "New life begins to sprout.", "Earth awakens once more." } },
            { Emotions.SeasonsSummer, new List<string> { "Sun's embrace warms all.", "Nights rich with stories." } },
            { Emotions.SeasonsAutumn, new List<string> { "Leaves dress in fire.", "Tribe gathers for harvest." } },
            { Emotions.SeasonsWinter, new List<string> { "Snow silences the land.", "We share tales and warmth." } }
        };
        // MidSapien Female dialogues
        conversations[(CharacterTypes.MidSapien, CharacterGenders.Female)] = new Dictionary<Emotions, List<string>>
        {
            { Emotions.Neutral, new List<string> { "World spins, stars guide.", "Night's canvas lit by stars." } },
            { Emotions.Joy, new List<string> { "Tribe rejoices for new life.", "Rain's dance is a blessing." } },
            { Emotions.Fear, new List<string> { "Sky's dark signs trouble.", "A distant cry warns us." } },
            { Emotions.Curiosity, new List<string> { "Cave's shadows hide mysteries.", "Do other lands echo our songs?" } },
            { Emotions.Contentment, new List<string> { "Tribe's bond is our strength.", "We give thanks for earth's gifts." } },
            { Emotions.Alertness, new List<string> { "New faces near. Friend or foe?", "Watchful eyes guard the night." } },
            { Emotions.Sadness, new List<string> { "River's pull took one away.", "Elder songs stir my soul." } },
            { Emotions.Hungry, new List<string> { "Apples dwindle. Seek elsewhere.", "Deers are plenty. To hunt or not?" } },
            { Emotions.Praise, new List<string> { "Your touch heals wounds.", "Your vision guides us to springs." } },
            { Emotions.SeasonsSpring, new List<string> { "Life stirs in the ground.", "Young voices join our songs." } },
            { Emotions.SeasonsSummer, new List<string> { "Breezes carry old chants.", "Land pulses with life." } },
            { Emotions.SeasonsAutumn, new List<string> { "Earth prepares for sleep.", "Baskets brim with harvest." } },
            { Emotions.SeasonsWinter, new List<string> { "Land rests beneath snow.", "Stories ward off the chill." } }
        };
        // Sapien Male dialogues
        conversations[(CharacterTypes.Sapien, CharacterGenders.Male)] = new Dictionary<Emotions, List<string>>
        {
            { Emotions.Neutral, new List<string> { "The cosmos hums its eternal song.", "Nature holds secrets, deep and profound." } },
            { Emotions.Joy, new List<string> { "The dance of the stars fills my heart.", "Wisdom shared, lights another's path." } },
            { Emotions.Fear, new List<string> { "Shadows grow when ignorance thrives.", "Eclipses, while transient, dim the soul's light." } },
            { Emotions.Curiosity, new List<string> { "What speaks the wind, rustling through leaves?", "Questions are keys to wisdom's gates." } },
            { Emotions.Contentment, new List<string> { "In harmony, the universe sings.", "With balance, life finds its rhythm." } },
            { Emotions.Alertness, new List<string> { "Change rustles on the horizon.", "Every ripple in the pond has a tale." } },
            { Emotions.Sadness, new List<string> { "Even the wise lament what's lost.", "Transience, the eternal dance." } },
            { Emotions.Hungry, new List<string> { "Starvation is nigh. The sacred deer, tempting me.", "Hunger pains. Must find food soon." } },
            { Emotions.Praise, new List<string> { "Your insight pierces the veil!", "In your reflections, truth is mirrored." } },
            { Emotions.SeasonsSpring, new List<string> { "Life stirs from winter's dream.", "Awakening, the world reblooms." } },
            { Emotions.SeasonsSummer, new List<string> { "The sun shares its radiant tales.", "Nature's symphony at its crescendo." } },
            { Emotions.SeasonsAutumn, new List<string> { "Falling leaves, wisdom distilled.", "Time's dance, gold and crimson." } },
            { Emotions.SeasonsWinter, new List<string> { "Nature's pause, a deep introspection.", "Silence, yet every snowflake whispers." } }
        };
        // Sapien Female dialogues
        conversations[(CharacterTypes.Sapien, CharacterGenders.Female)] = new Dictionary<Emotions, List<string>>
        {
            { Emotions.Neutral, new List<string> { "Every dusk, a story's end. Every dawn, a new tale.", "The river of existence flows, unceasing." } },
            { Emotions.Joy, new List<string> { "Light dances in every heart.", "In unity, the universe rejoices." } },
            { Emotions.Fear, new List<string> { "Darkness, a canvas for the soul's light.", "Unknown paths, veiled in mystery." } },
            { Emotions.Curiosity, new List<string> { "Whispers of ancient trees, tales untold.", "Beyond the horizon, knowledge awaits." } },
            { Emotions.Contentment, new List<string> { "In the present, the universe resides.", "Serenity, the gift of understanding." } },
            { Emotions.Alertness, new List<string> { "Winds shift, bearing new omens.", "The fire's flicker tells of changes." } },
            { Emotions.Sadness, new List<string> { "Grief, the echo of love's song.", "Every ending, a new beginning's shadow." } },
            { Emotions.Hungry, new List<string> { "My hunger calls. Must respect deer. Seek apples instead.", "Mushrooms in shade. Nature's bounty." } },
            { Emotions.Praise, new List<string> { "Your wisdom, a beacon for us all.", "In your words, the ancients speak." } },
            { Emotions.SeasonsSpring, new List<string> { "From slumber, the world emerges.", "Every bud, a promise reborn." } },
            { Emotions.SeasonsSummer, new List<string> { "Life's dance, joyous and unbridled.", "In warmth, nature's heart beats strong." } },
            { Emotions.SeasonsAutumn, new List<string> { "Harvest of thoughts, rich and deep.", "Echoes of time in rustling leaves." } },
            { Emotions.SeasonsWinter, new List<string> { "A time for reflection, beneath snow's blanket.", "Dreams weave futures in winter's night." } }
        };

        // Deer Male dialogues
        conversations[(CharacterTypes.Deer, CharacterGenders.Male)] = new Dictionary<Emotions, List<string>>
        {
            { Emotions.Neutral, new List<string> { "Water is clear.", "Forest is silent." } },
            { Emotions.Joy, new List<string> { "The sun kisses the meadow.", "A breeze dances through the antlers." } },
            { Emotions.Fear, new List<string> { "Scent of danger.", "Must protect the herd." } },
            { Emotions.Curiosity, new List<string> { "New blossoms in the glade.", "What's that rustling in the bushes?" } },
            { Emotions.Contentment, new List<string> { "The meadow is peaceful.", "Sun is warm, grass is green." } },
            { Emotions.Alertness, new List<string> { "Ears twitch at a distant sound.", "Every shadow could be a threat." } },
            { Emotions.Sadness, new List<string> { "Lost a fawn to the river's flow.", "Silent woods mourn with me." } },
            { Emotions.Hungry, new List<string> { "Grass low. Search new meadow.", "Berries? Sweet and filling." } },
            { Emotions.Praise, new List<string> { "The elder stag stands tall.", "His wisdom guides us." } },
            { Emotions.SeasonsSpring, new List<string> { "New life stirs the forest.", "Birdsong heralds new beginnings." } },
            { Emotions.SeasonsSummer, new List<string> { "Lush fields and cool streams.", "Nature in full splendor." } },
            { Emotions.SeasonsAutumn, new List<string> { "Leaves fall, preparing for the cold.", "The forest wears a golden cloak." } },
            { Emotions.SeasonsWinter, new List<string> { "Snow blankets the earth.", "A time of rest and reflection." } }
        };
        // Deer Female dialogues
        conversations[(CharacterTypes.Deer, CharacterGenders.Female)] = new Dictionary<Emotions, List<string>>
        {
            { Emotions.Neutral, new List<string> { "Grazing, always vigilant.", "The forest whispers its stories." } },
            { Emotions.Joy, new List<string> { "Fawn plays in the meadow.", "Berries are abundant." } },
            { Emotions.Fear, new List<string> { "Heard a Neanderthal's footsteps.", "Wolves howling nearby." } },
            { Emotions.Curiosity, new List<string> { "A butterfly's delicate dance.", "New scents brought by the wind." } },
            { Emotions.Contentment, new List<string> { "Safe among the herd.", "Protected by the grove's embrace." } },
            { Emotions.Alertness, new List<string> { "Something lurks in the shadows.", "Must shield the young ones." } },
            { Emotions.Sadness, new List<string> { "Empty nest, a fawn's journey begins.", "The forest feels the ache of absence." } },
            { Emotions.Hungry, new List<string> { "Fawns nibble scarce grass.", "Forage deeper. Find sustenance." } },
            { Emotions.Praise, new List<string> { "The doe leads with grace.", "Her elegance captivates us all." } },
            { Emotions.SeasonsSpring, new List<string> { "Fresh buds signal hope.", "Nature wakes from slumber." } },
            { Emotions.SeasonsSummer, new List<string> { "Days of plenty and warmth.", "Nights under the watchful moon." } },
            { Emotions.SeasonsAutumn, new List<string> { "Gathering for the winter ahead.", "Nature's lullaby in rustling leaves." } },
            { Emotions.SeasonsWinter, new List<string> { "Huddled together, we brave the cold.", "Dreaming of spring's promise." } }
        };

        conversations[(CharacterTypes.Shaman, CharacterGenders.Male)] = new Dictionary<Emotions, List<string>>
        {
            { Emotions.ShamanIntroduction, new List<string> { "So, it is you who my wisdom shall be passed down to. The spirits have spoken. Who am I to question their wisdom?",
                                                              "There is much for you to learn. Now, walk with me." } },
            { Emotions.ShamanHumanTutorial, new List<string> { "Take care of the tribe. They hurt, they hunger, they need faith to be strong." } },
            { Emotions.ShamanTreeTutorial, new List<string> { "These trees have lived here far longer than you and I. For that, they deserve respect.",
                                                              "The rain nourishes their roots and leaves, without rain, they shall not bear fruit." } },
            { Emotions.ShamanMushroomTutorial, new List<string> { "The sacred mushroom. It is your teacher, you are it's student.",
                                                                  "Throughout Spring, Summer and Autumn, rain shall bring them forth, they retreat in the winter." } },
             { Emotions.ShamanFluteTutorial, new List<string> { "I will teach you how to play the sacred flute, it shall lift our spirits.",
                                                                "Now, listen closely."} },
             { Emotions.ShamanFluteTutorialFail, new List<string> { "Why did you stop? You started off pretty good! Let's try again...",
                                                                "Now, listen closely."} },
            { Emotions.ShamanConclusion, new List<string> { "Now you've got it! You learn quickly.",
                                                             "Do not take what I have taught you for granted.",
                                                             "My work here is done. Now, I must leave you.",
                                                             "Goodbye... Shaman.", } },
        };
    }
}
