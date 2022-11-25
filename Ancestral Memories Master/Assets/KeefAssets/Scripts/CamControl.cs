using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    [Header("Camera Control")]
    [Header("========================================================================================================================")]

    public RPCamera rpCamera;
    private Camera cam;
    public Camera movementCam;
    public bool cinematicActive = false;
    public Vector3 cameraOffset;

    [Header("========================================================================================================================")]
    [Header("Camera Target Attributes")]

    public CharacterClass player;
    private Transform camTarget;
    private bool camFollowTarget = true;
    private CharacterBehaviours behaviours;
    [SerializeField] private bool isSpawning = false;

    [Header("========================================================================================================================")]
    [Header("Camera Target/Follow")]

    public float ScrollSpeed = 0.125f;
    public float SmoothSpeed = 0.125f;
    [SerializeField] private Vector3 camFollowOffset;

    [Header("========================================================================================================================")]
    [Header("Target Pespective Zoom")]

    [SerializeField] private float spawnZoom = 0;
    [SerializeField] private float initZoom = 0;
    [SerializeField] private float gameModeZoom = 0;
    [SerializeField] private float actionZoom = 0;
    [SerializeField] private float cutSceneZoom = 0;
    [SerializeField] private float psychedelicZoom = 0;
    [SerializeField] private float frontFaceZoom = 0;
    [SerializeField] private float toRoomZoom = 0;

    [Header("========================================================================================================================")]
    [Header("Target Orthographic Size")]

    [SerializeField] private float spawnOrtho = 0;
    [SerializeField] private float initOrtho = 0;
    [SerializeField] private float gameModeOrtho = 0;
    [SerializeField] private float actionOrtho = 0;
    [SerializeField] private float cutSceneOrtho = 0;
    [SerializeField] private float psychedelicOrtho = 0;
    [SerializeField] private float frontFaceOrtho = 0;
    [SerializeField] private float toRoomOrtho = 0;

    [Header("========================================================================================================================")]
    [Header("Zoom Duration")]

    [SerializeField] private float toSpawnZoomDuration = 30f;
    [SerializeField] private float toActionZoomDuration = 1f;
    [SerializeField] private float toFrontFaceZoomDuration = 1f;
    [SerializeField] private float toGameZoomDuration = 1f;
    [SerializeField] private float toCinematicZoomDuration = 1f;
    [SerializeField] private float toPsychedelicZoomDuration = 60f;
    [SerializeField] private float toNewRoomZoomDuration = 1f;

    [Header("========================================================================================================================")]

    [Header("Camera Rotation")]

    public bool camRotateAround = false; 
    public float RotationSpeed = 1f;
    float RotationSmoothFactor = 0.25f;


    public void Start()
    {

        camFollowTarget = true;
        camRotateAround = true;

        rpCamera.perspective = initZoom;
        cam.orthographicSize = initOrtho;

        camTarget = player.transform;

        ToSpawnZoom();

        cameraOffset = new Vector3(12, 2.5f, -8);
    }

    private Vector3 offset;

    void Awake()
    {
        cam = GetComponent<Camera>();
        behaviours = player.GetComponent<CharacterBehaviours>();
        offset = new Vector3(player.transform.position.x, player.transform.position.y + 8.0f, player.transform.position.z + 7.0f);
    }

    [ExecuteInEditMode]
    void Update()
    {

        rpCamera.UpdateProjection(true);
        //DebugChangeCam();
    }

    void SetCamClipPlane()
    {
        if (behaviours.isPsychdelicMode == true)
        {
            rpCamera.rpCam.farClipPlane = 5000;
            rpCamera.rpCam.nearClipPlane = -500;
        }

        if (cinematicActive == true)
        {
            rpCamera.rpCam.farClipPlane = 5000;
            rpCamera.rpCam.nearClipPlane = -65;
        }

        else if (cinematicActive == false && !behaviours.isPsychdelicMode)
        {
            rpCamera.rpCam.farClipPlane = 300;
            rpCamera.rpCam.nearClipPlane = -65;
        }
    }

    public void ToSpawnZoom()
    {
        isSpawning = true;
        cinematicActive = true; // Level introduction.
        SetCamClipPlane();
        float lerpDuration = toSpawnZoomDuration;
        zoomDestination = spawnZoom;
        orthoDestination = spawnOrtho;
        StartCoroutine(Zoom(lerpDuration, zoomDestination, orthoDestination));
    }

    public void ToActionZoom()
    {
        cinematicActive = true;
        SetCamClipPlane();
        float lerpDuration = toActionZoomDuration;
        zoomDestination = actionZoom;
        orthoDestination = actionOrtho;
        StartCoroutine(Zoom(toActionZoomDuration, zoomDestination, orthoDestination));
    }

    public void ToFrontFaceZoom()
    {
        cinematicActive = true; 
        SetCamClipPlane();
        float lerpDuration = toFrontFaceZoomDuration;
        zoomDestination = frontFaceZoom;
        orthoDestination = frontFaceOrtho;
        StartCoroutine(Zoom(lerpDuration, zoomDestination, orthoDestination));
    }

    public void ToGameZoom()
    {
        cinematicActive = false;
        camTarget = player.transform;
        SetCamClipPlane();
        float lerpDuration = toGameZoomDuration;
        zoomDestination = gameModeZoom;
        orthoDestination = gameModeOrtho;
        StartCoroutine(Zoom(lerpDuration, zoomDestination, orthoDestination));
    }

    public void ToCinematicZoom()
    {
        cinematicActive = true; 
        SetCamClipPlane();
        float lerpDuration = toCinematicZoomDuration;
        zoomDestination = cutSceneZoom;
        orthoDestination = cutSceneOrtho;
        StartCoroutine(Zoom(lerpDuration, zoomDestination, orthoDestination));
    }

    public void ToPsychedelicZoom()
    {
        cinematicActive = false; 
        SetCamClipPlane();
        float lerpDuration = toPsychedelicZoomDuration;
        zoomDestination = psychedelicZoom;
        orthoDestination = psychedelicOrtho;

        StartCoroutine(Zoom(lerpDuration, zoomDestination, orthoDestination));
    }

    public void EnterRoomZoom(GameObject interactedPortal)
    {
        camTarget = interactedPortal.transform;
        cinematicActive = false;
        SetCamClipPlane();
        float lerpDuration = toNewRoomZoomDuration;
        zoomDestination = toRoomZoom;
        orthoDestination = toRoomOrtho;

        StartCoroutine(Zoom(lerpDuration, zoomDestination, orthoDestination));
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

    private IEnumerator RotateAround()
    {
        if (cinematicActive)
        {
            Quaternion camTurnAngle = Quaternion.AngleAxis(Time.deltaTime * RotationSpeed, Vector3.up);
            cameraOffset = camTurnAngle * cameraOffset;

            Vector3 newPos = player.transform.position + cameraOffset;

            transform.position = Vector3.Slerp(transform.position, newPos, RotationSmoothFactor);
            transform.LookAt(player.transform.position);
        }
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
        if (camFollowTarget)
        {
            // Smooth camera follow
            Vector3 desiredPosition = new Vector3(camTarget.position.x, camTarget.position.y + camFollowOffset.y, camTarget.position.z + camFollowOffset.z);
            Vector3 SmoothedPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed);
            transform.position = SmoothedPosition;

        }
        else
        {
            return;
        }
    }

    public float turnSpeed = 4.0f;

    public float currentZoom = 0f;
    public float currentOrthoZoom = 0f;

    private float zoomDestination = 0f;
    private float orthoDestination = 0f;

    public float timeElapsed;

    IEnumerator Zoom(float lerpDuration, float zoomDestination, float orthoDestination)
    {

        float zoomMultiplier = 0;

        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)

        {
            if (behaviours.isPsychdelicMode && Input.GetMouseButton(0))
            {
                zoomMultiplier = 0.1f;
 
            }

            currentOrthoZoom = cam.orthographicSize;
            cam.orthographicSize = Mathf.Lerp(currentOrthoZoom, orthoDestination, timeElapsed / lerpDuration);

            float perspectiveDelay = 2;

            currentZoom = rpCamera.perspective;
            rpCamera.perspective = Mathf.Lerp(currentZoom, zoomDestination, timeElapsed / lerpDuration);
 
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        if (timeElapsed >= lerpDuration)
        {

            if (isSpawning)
            {
                ToGameZoom();
                isSpawning = false;
                yield break;
            }

        } 

    }

    public GameObject defaultCamPosition;

    public IEnumerator MoveCamToPosition(GameObject target, GameObject lookAtTarget, bool returnToDefault, float duration)
    {
        Vector3 newPosition = target.transform.position;
        Vector3 lookTarget = lookAtTarget.transform.position;

        float timeElapsed = 0;

        while (timeElapsed <= duration)
        {
            if (!returnToDefault)
            {
                camFollowTarget = false;
                transform.LookAt(lookTarget);

            }

            cam.transform.position = Vector3.Lerp(cam.transform.position, newPosition, timeElapsed / duration);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        if (timeElapsed >= duration)
        {
            if (returnToDefault)
            {

                StartCoroutine(MoveCamToPosition(defaultCamPosition, null, true, 10f));

                transform.LookAt(player.transform);
                camFollowTarget = true;

                yield break;
            }
        }
    }



    public AreaManager area;

    private IEnumerator ZoomCooldown()
    {
        float duration = 1f;
        yield return new WaitForSeconds(duration);

        ToGameZoom();

        yield break;
    }

}


