using System;
using UnityEngine;

public class Faith : Human

{

    [SerializeField] protected float currentFaith;

    [SerializeField] protected int minFaith = 0;
    [SerializeField] protected int maxFaith = 100;

    [SerializeField] protected float faithMultiplier = 0.5f;

    [SerializeField] protected bool isFaithless = false;

    [SerializeField] protected Shake earthQuake;

    protected bool shouldRevive = true;

    // Update is called once per frame

    private void Awake()
    {
        currentFaith = maxFaith;
    }

    public float GetFaith()
    {
        float faith = currentFaith;
        return faith;
    }

    public void SetFaith(float value)
    {
        currentFaith = value;
    }

    void Update()
    {

        DepleteFaith(0.1f * faithMultiplier);

        void DepleteFaith(float faith)
        {

            currentFaith -= faith;

            OnFaithChanged?.Invoke((int)currentFaith, maxFaith);

            faithBar.UpdateFaithBar(currentFaith / maxFaith);

            if (currentFaith <= minFaith)
            {
                earthQuake.start = true;

                currentFaith = minFaith;

                isFaithless = true;

                Debug.Log("Player is faithless!");

                //  Trigger chance to be struck down by god / natural disasters
                // Have a vocal cue + thunder rumble 'Heretic!'. First Testament style.

            }
            else
            {
                earthQuake.start = false;

                isFaithless = false;
            }

        }
    }

    public bool CheckFaith()
    {
        if (currentFaith <= 50) {
            shouldRevive = false;
        } else
        {
            shouldRevive = true;
        }

        return shouldRevive;
    }

    public event Action<int, int> OnFaithChanged;

}
