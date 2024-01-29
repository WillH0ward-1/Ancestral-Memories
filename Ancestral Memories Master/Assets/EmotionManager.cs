using UnityEngine;
using System;
using System.Collections.Generic;

public class EmotionManager : MonoBehaviour
{
    public event Action<DialogueLines.Emotions> OnEmotionChanged;

    private List<DialogueLines.Emotions> emotionPool = new List<DialogueLines.Emotions>();
    private System.Random random = new System.Random();

    public DialogueLines.Emotions CurrentEmotion
    {
        get
        {
            if (emotionPool.Count > 0)
            {
                // Randomly select an emotion from the pool
                int index = random.Next(emotionPool.Count);
                return emotionPool[index];
            }
            return default(DialogueLines.Emotions); // Return a default value if the pool is empty
        }
    }

    public void AddEmotion(DialogueLines.Emotions newEmotion)
    {
        if (!emotionPool.Contains(newEmotion))
        {
            emotionPool.Add(newEmotion);
            OnEmotionChanged?.Invoke(CurrentEmotion); // Invoke with a randomly selected emotion
        }
    }

    public void RemoveEmotion(DialogueLines.Emotions emotion)
    {
        if (emotionPool.Contains(emotion))
        {
            emotionPool.Remove(emotion);
            OnEmotionChanged?.Invoke(CurrentEmotion); // Invoke with a new randomly selected emotion
        }
    }

    public void ClearEmotionPool()
    {
        emotionPool.Clear();
    }
}
