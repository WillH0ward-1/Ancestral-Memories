using System;
using UnityEngine;


public class Faith : MonoBehaviour

{
    public FaithBar playerFaith;

    [SerializeField] private float currentFaith;

    public int minFaith = 0;
    public int maxFaith = 100;

    float faithMultiplier = 0.5f;

    public bool playerIsFaithless = false;

    public Shake earthQuake;

    // Update is called once per frame

    private void Awake()
    {
        currentFaith = maxFaith;
    }

    void Update()
    {

        DepleteFaith(0.1f * faithMultiplier);

        void DepleteFaith(float faith)
        {

            currentFaith -= faith;

            OnFaithChanged?.Invoke((int)currentFaith, maxFaith);

            playerFaith.UpdateFaith(currentFaith / maxFaith);

            if (currentFaith <= minFaith)
            {
                earthQuake.start = true;

                currentFaith = minFaith;

                playerIsFaithless = true;

                Debug.Log("Player is faithless!");

                //  Trigger chance to be struck down by god / natural disasters
                // Have a vocal cue + thunder rumble 'Heretic!'. First Testament style.

            }
            else
            {
                earthQuake.start = false;

                playerIsFaithless = false;
            }

        }
    }

    public event Action<int, int> OnFaithChanged;

}
