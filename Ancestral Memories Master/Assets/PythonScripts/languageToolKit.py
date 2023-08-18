import string
import os

try:
    import nltk
    from nltk.corpus import cmudict
    print("NLTK imported successfully.")
except ImportError:
    print("Please install NLTK library: pip install nltk")
    exit()


# Ensure CMU dictionary is downloaded
try:
    cmu_dict = cmudict.dict()
except LookupError:
    nltk.download('cmudict')
    cmu_dict = cmudict.dict()

def word_to_phonetic(word):
    """Convert word to its phonetic transcription using CMU dict."""
    if word in cmu_dict:
        return cmu_dict[word][0]  # Take the first pronunciation variant
    else:
        return None

def get_syllable_count(phonemes):
    return sum(1 for phoneme in phonemes if phoneme[-1].isdigit())

def get_stress_pattern(phonemes):
    return [phoneme[-1] for phoneme in phonemes if phoneme[-1].isdigit()]

def categorize_phonemes(phonemes):
    vowels = []
    consonants = []
    # You might need to adjust this categorization based on your needs
    for phoneme in phonemes:
        if phoneme in ["AA", "AE", "AH", "AO", "AW", "AY", "EH", "ER", "EY", "IH", "IY", "OW", "OY", "UH", "UW"]:
            vowels.append(phoneme)
        else:
            consonants.append(phoneme)
    return vowels, consonants

def main():
    base_path = os.path.dirname(os.path.abspath(__file__))
    every_word_path = os.path.join(base_path, "LanguageGen", "CharResources", "EveryWord.txt")
    output_path = os.path.join(base_path, "LanguageGen", "CharResources", "PhoneticTranscriptions.txt")
    additional_output_path = os.path.join(base_path, "LanguageGen", "CharResources", "PhoneticAdditionalInfo.txt")

    with open(every_word_path, 'r') as f:
        words = f.read().split()

    phonetics = {}
    additional_info = {}

    for word in words:
        word_clean = word.lower().strip(string.punctuation)
        phonetic = word_to_phonetic(word_clean)
        if phonetic:
            phonetics[word] = phonetic
            additional_info[word] = {
                'syllable_count': get_syllable_count(phonetic),
                'stress_pattern': get_stress_pattern(phonetic),
                'vowels': categorize_phonemes(phonetic)[0],
                'consonants': categorize_phonemes(phonetic)[1],
                # Add more info as needed
            }

    with open(output_path, 'w') as f:
        for word, phon in phonetics.items():
            f.write(f"{word}={','.join(phon)}\n")

    with open(additional_output_path, 'w') as f:
        for word, info in additional_info.items():
            f.write(f"{word}={info}\n")

if __name__ == "__main__":
    main()
