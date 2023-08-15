using UnityEngine;
using System;

public class EmotionManager : MonoBehaviour
{
    public event Action<DialogueLines.Emotions> OnEmotionChanged;

    private DialogueLines.Emotions currentEmotion;

    public DialogueLines.Emotions CurrentEmotion
    {
        get { return currentEmotion; }
        set
        {
            if (currentEmotion != value)
            {
                currentEmotion = value;
                OnEmotionChanged?.Invoke(currentEmotion);
            }
        }
    }

    private void Start()
    {
        // Set default emotion, if necessary
        currentEmotion = DialogueLines.Emotions.Joy;
    }

    public void SetEmotion(DialogueLines.Emotions newEmotion)
    {
        CurrentEmotion = newEmotion;
    }
}
