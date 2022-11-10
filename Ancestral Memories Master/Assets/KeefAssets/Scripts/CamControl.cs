using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public CharacterClass player;

    public Transform target;

    private bool MoveCamera = true;

    public float ScrollSpeed = 0.125f;
    public float SmoothSpeed = 0.125f;

    [SerializeField] private Vector3 camFollowOffset;

    public bool cinematicActive = false;

    [SerializeField] private float maxZoom = 4;
    [SerializeField] private float gameModeZoom = 0;
    [SerializeField] private float cutSceneZoom = 0;
    [SerializeField] private float psychedelicZoom = -1;
    [SerializeField] private float spawnZoom = -6;
    [SerializeField] private float minZoom = 6;

    [SerializeField] private bool SpawnCam = false;
    [SerializeField] private bool GameCam = false;
    [SerializeField] private bool CutsceneCam = false;
    [SerializeField] private bool PsychedelicCam = false;

    [SerializeField] private bool playerSpawning = false;

    [SerializeField] private float InitPerspective = 4;

    string cutscene = ("");

    public RPCamera rpCamera;
    public Camera hiddenCam;

    public bool RotateAroundPlayer = false; // Enable after psychedelic ingestion
    public float RotationSpeed = 5f;

    public Vector3 cameraOffset;
    // Update is called once per frame

    [ExecuteInEditMode]
    void Update()
    {

        rpCamera.UpdateProjection(true);

        //DebugChangeCam();
    }

    float SmoothFactor = 0.25f;


    public void Start()
    {
        RotateAroundPlayer = false;

        rpCamera.perspective = spawnZoom;

        ToSpawnZoom();

        cameraOffset = new Vector3(12, 2.5f, -8);
    }

    void SetCamClipPlane()
    {
        if (cinematicActive == true)
        {
            rpCamera.rpCam.farClipPlane = 5000;
            rpCamera.rpCam.nearClipPlane = -65;
        }

        else if (cinematicActive == false)
        {
            rpCamera.rpCam.farClipPlane = 300;
            rpCamera.rpCam.nearClipPlane = -65;
        }
    }

    /*
     * WIP switch cams for debugging/editing.
     * 
    public void DebugChangeCam()
    {
        if (SpawnCam)
        {
            StopAllCoroutines();
            ToSpawnZoom();
        }
        if (GameCam)
        {
            StopAllCoroutines();
            ToGameZoom();
        }
        if (CutsceneCam)
        {
            StopAllCoroutines();
            ToCutsceneZoom();
        }
        if (PsychedelicCam)
        {
            StopAllCoroutines();
            ToPsychedelicZoom();
        } else
        {
            return;
        }
    }
    */

    public void ToSpawnZoom()
    {
        playerSpawning = true;
        //StartCoroutine(RotateCamera());
        cinematicActive = true; // Level introduction.
        SetCamClipPlane();
        lerpDuration = 4f;
        zoomDestination = minZoom;
        StartCoroutine(Zoom(lerpDuration, zoomDestination));
    }

    public void ToGameZoom()
    {
        cinematicActive = false; // Level introduction.
        //StartCoroutine(UserZoomBuffer());
        SetCamClipPlane();
        lerpDuration = 1f;
        zoomDestination = gameModeZoom;
        StartCoroutine(Zoom(lerpDuration, zoomDestination));
    }

    public void ToCutsceneZoom()
    {
        cinematicActive = true; // Level introduction.
        SetCamClipPlane();
        lerpDuration = 1f;
        zoomDestination = cutSceneZoom;
        StartCoroutine(Zoom(lerpDuration, zoomDestination));
    }

    public void ToPsychedelicZoom()
    {
    //    cinematicActive = true; // Level introduction.
        SetCamClipPlane();
        lerpDuration = 10f;
        zoomDestination = cutSceneZoom;
        StartCoroutine(Zoom(lerpDuration, zoomDestination));
    }

    [SerializeField] private float rotationSpeedMultiplier = 1f;
    [SerializeField] float rotationDuration = 1;

    IEnumerator UserZoomBuffer()
    {
        float zoomBuffer = 4f;
        yield return new WaitForSeconds(zoomBuffer); // buffer until player can take control of the scrollbar.
        //AllowUserZoom();
        yield break;
    }
    /*

    IEnumerator AllowUserZoom()
    {
        while (!cinematicActive == true)
        {
            if (MainCam.orthographic)
            {
                MainCam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed; // Use this when using Orthographic Camera.
            }
            else
            {
                MainCam.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed; // Otherwise, use this (Perspective Camera).
            }

            if (MainCam.orthographicSize >= maxZoom)
            {
                MainCam.orthographicSize = maxZoom;
            }

            if (MainCam.orthographicSize <= minZoom)
            {
                MainCam.orthographicSize = minZoom;
            }

            continue;
        }

        yield return null;

    }
    */

    public void FixedUpdate()
    {

        if (MoveCamera)
        {
            // Smooth camera follow.

            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y + camFollowOffset.y, target.position.z + camFollowOffset.z);
            Vector3 SmoothedPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed);
            transform.position = SmoothedPosition;

        }

        if (RotateAroundPlayer)
        {
            Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse ScrollWheel") * RotationSpeed, Vector3.up);
            cameraOffset = camTurnAngle * cameraOffset;

            Vector3 newPos = player.transform.position + cameraOffset;

            transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);
            transform.LookAt(player.transform.position);
        }



    }

    public float turnSpeed = 4.0f;

    private Vector3 offset;

    void Awake()
    {
        offset = new Vector3(player.transform.position.x, player.transform.position.y + 8.0f, player.transform.position.z + 7.0f);
    }

    // Camera 'zoom' into target.

    public float currentZoom = 0f;
    private float zoomDestination = 0f;

    public float timeElapsed;
    public float lerpDuration = 5;
 

    IEnumerator Zoom(float lerpDuration, float zoomDestination)

    {

        float timeElapsed = 0;

        while (timeElapsed < lerpDuration)

        {

            currentZoom = rpCamera.perspective;

            rpCamera.perspective = Mathf.Lerp(currentZoom, zoomDestination, timeElapsed / lerpDuration);

            timeElapsed += Time.deltaTime;

            yield return null;

        }
        if (timeElapsed >= lerpDuration)
        {
            StartCoroutine(ZoomCooldown());
        }
    }

    IEnumerator ZoomCooldown()
    {
        float duration = 1f;
        yield return new WaitForSeconds(duration);

        ToGameZoom();

        if (playerSpawning)
        {
            playerSpawning = false;
        }
        yield break;
    }

}


