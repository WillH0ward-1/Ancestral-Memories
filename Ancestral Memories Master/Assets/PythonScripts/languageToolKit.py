import string

try:
    import nltk
    from nltk.corpus import cmudict
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

def main():
    every_word_path = "../EveryWord.txt"
    output_path = "../PhoneticTranscriptions.txt"

    with open(every_word_path, 'r') as f:
        words = f.read().split()

    phonetics = {}
    for word in words:
        word_clean = word.lower().strip(string.punctuation)
        phonetic = word_to_phonetic(word_clean)
        if phonetic:
            phonetics[word] = phonetic

    with open(output_path, 'w') as f:
        for word, phon in phonetics.items():
            f.write(f"{word}={','.join(phon)}\n")

if __name__ == "__main__":
    main()
