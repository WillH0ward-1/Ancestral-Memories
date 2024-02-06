using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InteractableManager : MonoBehaviour
{
    public enum InteractableAction
    {
        PlayMusic,
        Reflect,
        Look,
        Pray,
        Dance,
        Harvest,
        Talk,
        Eat,
        Drink,
        Sleep,
        InstructHarvest,
        InstructHunt,
        InstructBuild,
        Enter,
        Plant
    }

    [System.Serializable]
    public class InteractableOption
    {
        public InteractableAction action;
        public Color color;
        public Sprite sprite;
    }

    [SerializeField]
    private List<InteractableAction> availableActions;

    private Dictionary<InteractableAction, Color> actionColors = new Dictionary<InteractableAction, Color>
{
    {InteractableAction.PlayMusic, Color.blue},
    {InteractableAction.Reflect, Color.cyan},
    {InteractableAction.Look, Color.grey},
    {InteractableAction.Pray, Color.magenta},
    {InteractableAction.Dance, Color.red},
    {InteractableAction.Harvest, Color.green},
    {InteractableAction.Talk, Color.yellow},
    {InteractableAction.Eat, new Color(1f, 0.647f, 0f)}, // Orange color for eating
    {InteractableAction.Drink, Color.blue},
    {InteractableAction.Sleep, Color.white},
    {InteractableAction.InstructHarvest, Color.green},
    {InteractableAction.InstructHunt, new Color(0.545f, 0.271f, 0.075f)}, // Brown color for hunting
    {InteractableAction.InstructBuild, Color.yellow},
    {InteractableAction.Enter, Color.grey},
    {InteractableAction.Plant, Color.magenta}
};


    private Interactable interactable;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        InitInteractable();
    }

    private void InitInteractable()
    {
        if (interactable != null)
        {
            interactable.options = availableActions.Select(action => new Interactable.Action
            {
                title = action.ToString(),
                color = actionColors[action],
                sprite = LoadSpriteFromResources(action.ToString())
            }).ToArray();
        }
        else
        {
            Debug.LogError("Interactable component not found on the GameObject.");
        }
    }

    private Sprite LoadSpriteFromResources(string actionName)
    {
        string spritePath = "MenuUI/" + actionName;
        Sprite sprite = Resources.Load<Sprite>(spritePath);
        if (sprite == null)
        {
            Debug.LogWarning($"Sprite not found for action {actionName} at path: {spritePath}");
        }
        return sprite;
    }

    public void AddOption(InteractableAction action)
    {
        if (interactable != null)
        {
            // Check if the action doesn't already exist in the options
            if (!interactable.options.Any(opt => opt.title == action.ToString()))
            {
                // Add a new action to the options
                var newAction = new Interactable.Action()
                {
                    title = action.ToString(),
                    color = actionColors[action],
                    sprite = LoadSpriteFromResources(action.ToString())
                };

                var optionsList = interactable.options.ToList();
                optionsList.Add(newAction);
                interactable.options = optionsList.ToArray();
            }
        }
    }


    public void RemoveOption(InteractableAction action)
    {
        if (interactable != null)
        {
            // Remove the action from the options if it exists
            interactable.options = interactable.options.Where(opt => opt.title != action.ToString()).ToArray();
        }
    }

}
