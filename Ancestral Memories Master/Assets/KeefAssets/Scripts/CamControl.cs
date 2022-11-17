using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    public CharacterClass player;
    private CharacterBehaviours behaviours;

    private Transform camTarget;

    private bool camFollowTarget = true;

    public float ScrollSpeed = 0.125f;
    public float SmoothSpeed = 0.125f;

    [SerializeField] private Vector3 camFollowOffset;

    public bool cinematicActive = false;



    [SerializeField] private float spawnZoom = 0;
    [SerializeField] private float initZoom = 0;
    [SerializeField] private float gameModeZoom = 0;
    [SerializeField] private float actionZoom = 0;
    [SerializeField] private float cutSceneZoom = 0;
    [SerializeField] private float psychedelicZoom = 0;


    [SerializeField] private float spawnOrtho = 0;
    [SerializeField] private float initOrtho = 0;
    [SerializeField] private float gameModeOrtho = 0;
    [SerializeField] private float actionOrtho = 0;
    [SerializeField] private float cutSceneOrtho = 0;
    [SerializeField] private float psychedelicOrtho = 0;


    [SerializeField] private bool SpawnCam = false;
    [SerializeField] private bool GameCam = false;
    [SerializeField] private bool CutsceneCam = false;
    [SerializeField] private bool PsychedelicCam = false;

    [SerializeField] private bool isSpawning = false;

    //string cutscene = ("");

    public RPCamera rpCamera;
    private Camera cam;
    public Camera movementCam;

    public bool camRotateAround = false; // Enable after psychedelic ingestion
    public float RotationSpeed = 1f;

    public Vector3 cameraOffset;
    // Update is called once per frame


    float SmoothFactor = 0.25f;


    public void Start()
    {
        camFollowTarget = true;
        camRotateAround = false;

        rpCamera.perspective = initZoom;
        cam.orthographicSize = initOrtho;

        camTarget = player.transform;

        ToSpawnZoom();

        cameraOffset = new Vector3(12, 2.5f, -8);
    }

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

    public float lerpDuration = 0f;

    public void ToSpawnZoom()
    {
        isSpawning = true;
        //StartCoroutine(RotateCamera());
        cinematicActive = true; // Level introduction.
        SetCamClipPlane();
        lerpDuration = 4f;
        zoomDestination = spawnZoom;
        orthoDestination = spawnOrtho;
        StartCoroutine(Zoom(lerpDuration, zoomDestination, orthoDestination));
    }

    public void ToActionZoom()
    {
        cinematicActive = true; // Level introduction.
        //StartCoroutine(RotateAround());
        //StartCoroutine(UserZoomBuffer());
        SetCamClipPlane();
        lerpDuration = 1f;
        zoomDestination = actionZoom;
        orthoDestination = actionOrtho;
        StartCoroutine(Zoom(lerpDuration, zoomDestination, orthoDestination));
    }

    public void ToGameZoom()
    {
        //RotateAroundPlayer = true;
        cinematicActive = false;
        //StartCoroutine(UserZoomBuffer());
        camTarget = player.transform;
        SetCamClipPlane();
        lerpDuration = 1f;
        zoomDestination = gameModeZoom;
        orthoDestination = gameModeOrtho;
        StartCoroutine(Zoom(lerpDuration, zoomDestination, orthoDestination));
    }

    public void ToCutsceneZoom()
    {
        cinematicActive = true; 
        SetCamClipPlane();
        lerpDuration = 1f;
        zoomDestination = cutSceneZoom;
        orthoDestination = cutSceneOrtho;
        StartCoroutine(Zoom(lerpDuration, zoomDestination, orthoDestination));
    }

    public void ToPsychedelicZoom()
    {
        cinematicActive = false; 
        SetCamClipPlane();
        lerpDuration = 60000f;
        zoomDestination = psychedelicZoom;
        orthoDestination = psychedelicOrtho;

        StartCoroutine(Zoom(lerpDuration, zoomDestination, orthoDestination));
    }

    public void EnterRoomZoom(GameObject interactedPortal)
    {
        camTarget = interactedPortal.transform;
        cinematicActive = false;
        SetCamClipPlane();
        lerpDuration = 1f;
        zoomDestination = 9;
        orthoDestination = 1;

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

            transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);
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
        return;
    }

    public float turnSpeed = 4.0f;

    private Vector3 offset;

    bool camZoomComplete = false;

    // Camera 'zoom' into target.

    public float currentZoom = 0f;
    public float currentOrthoZoom = 0f;

    private float zoomDestination = 0f;
    private float orthoDestination = 0f;

    public float timeElapsed;

    IEnumerator Zoom(float lerpDuration, float zoomDestination, float orthoDestination)
    {
      

        float zoomMultiplier = 1;

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
 
            timeElapsed += Time.deltaTime * zoomMultiplier;

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

    public GameObject targetPostion;

    public IEnumerator FlipCam(GameObject target, float duration)
    {
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, target.transform.position, timeElapsed / duration);

            cam.transform.LookAt(player.transform);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        if (timeElapsed > duration)
        {
            yield break;
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


