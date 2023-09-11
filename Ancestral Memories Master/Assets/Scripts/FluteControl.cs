using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using System.Linq;
using FMOD.Studio;
using System.Runtime.InteropServices;

public class FluteControl : MonoBehaviour
{
    [SerializeField] private float distance;

    [SerializeField] private GameObject attenuationObject;

    public Player player;
    public CharacterBehaviours behaviours;
    public AudioSFXManager playerSFX;

    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask targetLayer;

    public bool fluteActive = false;

    [SerializeField] private float targetFluteScale;

    [SerializeField] private float minFluteScale = 0;
    [SerializeField] private float maxFluteScale = 6;

    [SerializeField] private float minDistance = 0;
    [SerializeField] private float maxDistance = 100;

    bool isPlaying = false;

    [SerializeField] private float interval = 0;
    private float initInterval = 0;
    private float trigThreshold = 0.1f;

    [SerializeField] private float maxInterval = 0.1f;

    [SerializeField] private float faithFactor = 0.25f;

    public MapObjGen mapObjGen;

    /*
    private void Awake()
    {
        //attenuationObject = transform.root.gameObject;
    }
    */


    private Vector2 screenCenter;

    public bool fluteModeActive = false;

    private void Awake()
    {
        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        playerSFX = GetComponentInChildren<AudioSFXManager>();
    }

    public void InitializeFlute()
    {

    }

    FMOD.Studio.EVENT_CALLBACK callbackDelegate;

    private void Start()
    {
        callbackDelegate = new EVENT_CALLBACK(ProgrammerCallBack.ProgrammerInstCallback);
    }

    public void EnableFluteControl()
    {
        fluteModeActive = true;
        StartCoroutine(CastRayToScreen());
    }

    [SerializeField] float tValRef;
    [SerializeField] float fluteScaleOutputRef;

    float minNoteIndex = 0;
    float maxNoteIndex;

    [SerializeField] private int noteIndex;

    [SerializeField] private string currentNote;

    [SerializeField] private string[] noteBank;

    public IEnumerator CastRayToScreen()
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        maxInterval = initInterval; // Play note immediately on first press
        StartCoroutine(ModifyFaithOverTime());
        while (Input.GetMouseButton(0))
        {
            noteBank = musicManager.notesToUse;

            if (Input.GetAxis("Mouse X") != 0 && Input.GetAxis("Mouse Y") != 0)
            {
                interval = 0;
                maxInterval = trigThreshold;
                while (interval <= maxInterval)
                {
                    interval += Time.deltaTime;
                    yield return null;
                }
            }

            distance = Vector2.Distance(screenCenter, Input.mousePosition);
            var t = Mathf.InverseLerp(minDistance, maxDistance, distance);
            maxNoteIndex = musicManager.notesToUse.Length - 1;
            noteIndex = Mathf.RoundToInt(Mathf.Lerp(minNoteIndex, maxNoteIndex, t));

            string note = musicManager.notesToUse[noteIndex];

            if (currentNote != note && fluteActive)
            {
                currentNote = note;
                StopFluteSound();
                PlayFluteSound(note);
            }

            if (Input.GetMouseButton(0) && !fluteActive && interval >= maxInterval)
            {
                PlayFluteSound(note);
            }

            yield return null;
        }

        StopFluteSound();
        StopCoroutine(ModifyFaithOverTime());
        yield break;
    }


    public IEnumerator ModifyFaithOverTime()
    {
        while (Input.GetMouseButton(0))
        {
            player.FaithModify(faithFactor / 100);

            foreach (AICharacterStats stats in mapObjGen.allHumanStats)
            {
                stats.FaithModify(faithFactor / 100);
            }

            yield return null;
        }
    }


    public MusicManager musicManager;
    public EventReference eventPath;

    string instrumentFileRootName = "Instrument";
    string instrument = "PlayerFlute";


    public float minDuration = 5f;
    public float maxDuration = 15f;

    void PlayFluteSound(string note)
    {
        fluteActive = true;

        string key = instrumentFileRootName + "/" + instrument + "/" + note;

        UnityEngine.Debug.Log(key);

        EventInstance instrumentInstance = RuntimeManager.CreateInstance(eventPath);
        RuntimeManager.AttachInstanceToGameObject(instrumentInstance, transform, playerSFX.rigidBody);
        instanceRef = instrumentInstance;

        UnityEngine.Debug.Log(instrumentInstance);

        GCHandle stringHandle = GCHandle.Alloc(key, GCHandleType.Pinned);
        instrumentInstance.setUserData(GCHandle.ToIntPtr(stringHandle));
        instrumentInstance.setCallback(callbackDelegate);

        instrumentInstance.start();
        instrumentInstance.release();

        // Start the new coroutine
        StartCoroutine(ModifyFaithOverTime());
    }


    EventInstance instanceRef;

    void StopFluteSound()
    {
        fluteActive = false;
        instanceRef.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        // Stop the coroutine
        StopCoroutine(ModifyFaithOverTime());

        StartCoroutine(CastRayToScreen());
    }

    public void StopAll()
    {
        if (fluteModeActive)
        {
            fluteModeActive = false;
        }
        StopFluteSound();
        StopAllCoroutines();
    }
}

