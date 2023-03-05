using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using System.Linq;
using FMOD.Studio;
using System.Runtime.InteropServices;

public class PlayFlute : MonoBehaviour
{
    [SerializeField] private float distance;

    [SerializeField] private GameObject attenuationObject;

    [SerializeField] private Player player;
    [SerializeField] private CharacterBehaviours behaviours;
    [SerializeField] private PlayerSoundEffects playerSFX;

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

    /*
    private void Awake()
    {
        //attenuationObject = transform.root.gameObject;
    }
    */

    private Vector2 screenCenter;
 
    private void Awake()
    {
        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    FMOD.Studio.EVENT_CALLBACK callbackDelegate;

    private void Start()
    {
        callbackDelegate = new EVENT_CALLBACK(ProgrammerCallBack.ProgrammerInstCallback);
    }

    public void EnableFluteControl()
    {
        StartCoroutine(CastRayToGround());
    }

    [SerializeField] float tValRef;
    [SerializeField] float fluteScaleOutputRef;

    float minNoteIndex = 0;
    float maxNoteIndex;

    [SerializeField] private int noteIndex;

    [SerializeField] private string currentNote;

    [SerializeField] private string[] noteBank;

    public IEnumerator CastRayToGround()
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        maxInterval = initInterval; // Play note immediately on first press

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

            /*
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit rayHit, Mathf.Infinity, targetLayer))
            {
            */

            // Vector3 attenuationObjectPos = attenuationObject.transform.position;

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

            //playerSFX.fluteEventRef.setParameterByName("Mode", targetFluteScale);

            player.GainFaith(faithFactor);

            yield return null;
        }

        StopFluteSound();
        yield break;


    }

    public MusicManager musicManager;
    public EventReference eventPath;

    string instrumentFileRootName = "Instrument";
    string instrument = "Player-Flute";


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

        RESULT timeLinePos = instrumentInstance.getTimelinePosition(out int timelinePosition);
        instrumentInstance.getDescription(out EventDescription desc);

        float clipLength = (float)desc.getLength(out int length);

        float duration = Random.Range(clipLength / 2, clipLength);

        float timelineSeconds = timelinePosition / 1000f;

        if (timeLinePos == RESULT.OK && timelineSeconds >= duration - 0.1)
        {
            instrumentInstance.setTimelinePosition(0);
            //instrumentInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

    }

    EventInstance instanceRef;

    void StopFluteSound()
    {
        fluteActive = false;
        instanceRef.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        StartCoroutine(CastRayToGround());
    }

    public void StopAll()
    {
        StopFluteSound();
        StopAllCoroutines();
    }
}