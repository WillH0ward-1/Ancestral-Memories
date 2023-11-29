using UnityEngine;

public class Interactable : MonoBehaviour
{
    private OutlineControl outlineControl;

    public Action[] options;

    [System.Serializable]
    public class Action
    {
        public Color color;
        public Sprite sprite;
        public string title;
    }

    void Awake()
    {
        outlineControl = GetComponentInChildren<OutlineControl>();
    }

    public void ToggleOutline(bool state)
    {
        if (outlineControl != null)
            outlineControl.outline.enabled = state;
    }

    public void SpawnMenu(GameObject lastHit, RaycastHit rayHit)
    {
        RadialMenuSpawner.menuInstance.SpawnMenu(this, lastHit, rayHit);
    }
}
