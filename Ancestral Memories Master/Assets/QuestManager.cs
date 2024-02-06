using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class QuestManager : MonoBehaviour
{
    public enum Quests
    {
        ShamanIntroduction,
        ShamanTreeTutorial,
        ShamanLightningTutorial,
        ShamanFireTutorial,
        ShamanHumanTutorial,
        ShamanMushroomTutorial,
        ShamanFluteTutorial,
        ShamansConclusion,
        TalkToNeanderthals,
        GainAFollower,
        CreateSettlement,
        GatherWood,
        GatherApples,
        GatherStone,
        CreateHome,
        ConstructTemple,
        FillAllSeatsAtTemple,
        OfferSacrificeAtTemple,
        PlayFluteAtTemple,
        SpeakToGod,
        TakeMushroom,
        SpeakToShamanSpirit,
        GatherCitizensAtTemple,
        TellTripStoryToTribe
        

        // Add more quests here as needed
    }

    public static QuestManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Only call DontDestroyOnLoad when in play mode
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }


    private Quests currentQuest;
    private HashSet<Quests> completedQuests = new HashSet<Quests>();

    void OnEnable()
    {
        SetCurrentQuest(Quests.ShamanIntroduction);
    }

    public Action OnQuestChanged;

    public void SetCurrentQuest(Quests quest)
    {
        if (currentQuest != quest)
        {
            currentQuest = quest;
            OnQuestChanged?.Invoke();
        }
    }


    public Quests GetCurrentQuest()
    {
        return currentQuest;
    }

    public void CompleteQuest(Quests quest)
    {
        if (quest == currentQuest)
        {
            completedQuests.Add(quest);
            AdvanceToNextQuest();
        }
    }

    private void AdvanceToNextQuest()
    {
        Quests lastQuest = (Quests)System.Enum.GetValues(typeof(Quests)).Length - 1;
        if (currentQuest < lastQuest)
        {
            SetCurrentQuest(currentQuest + 1);
        }
        else
        {
            // Handle end of quest line or loop back to the beginning
            // SetCurrentQuest(Quests.ShamanIntroduction); // To loop back
        }
    }

    public bool IsQuestCompleted(Quests quest)
    {
        return completedQuests.Contains(quest);
    }

    public bool IsShamanQuestActive()
    {
        // Return true if the current quest's name starts with "Shaman"
        return currentQuest.ToString().StartsWith("Shaman");
    }


    // Additional methods can be added to manage prerequisites, etc.
}
