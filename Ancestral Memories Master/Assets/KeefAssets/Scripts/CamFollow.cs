using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{

    public Transform target;

    public float ScrollSpeed = 0.125f;
    public float SmoothSpeed = 0.125f;

    [SerializeField]
    private float defaultCamZoom = 35;

    [SerializeField]
    private Camera MainCam;

    [SerializeField]
    private RPCamera RPCam;

    [SerializeField] private float maxZoom = 32;

    [SerializeField] private float gameModeZoom = 32;

    [SerializeField] private float spawnZoomDistance = 1000;

    [SerializeField] private float minZoom = 6;

    public bool cinematicActive = false;

    public Vector3 camFollowOffset;

    public bool MoveCamera = true;

    public float cineZoomMultiplier = 0.125f;

    private float camLerp;

    public bool gameStarted = true;

    public bool playerSpawning = true;

    public CharacterClass player;

    string cutscene = ("");

    float camCooldown = 0f;

    bool useMainCam = true;

    // Update is called once per frame

    public void Start()
    {

        MainCam = GetComponent<Camera>();
        RPCam = GetComponent<RPCamera>();


        useMainCam = true;
        SwitchCams();

        RPCam.perspective = 0;

        playerSpawning = true;

        cinematicActive = false; // Level introduction.

        gameStarted = true;

        MainCam.orthographicSize = spawnZoomDistance;

        ToSpawnZoom();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            useMainCam ^= true;
            SwitchCams();
        }
    }

    public void SwitchCams()
    {
        if (useMainCam)
        {
            RPCam.enabled = false;

            currentZoom = MainCam.orthographicSize;
        }
        else
        {
            RPCam.enabled = true;

            currentZoom = RPCam.perspective;
        }
    }

    void SetCamClipPlane()
    {
        if (cinematicActive == true)
        {
            MainCam.farClipPlane = 5000;
            MainCam.nearClipPlane = -5000;
        }

        else if (cinematicActive == false)
        {
            MainCam.farClipPlane = 300;
            MainCam.nearClipPlane = -300;
        }
    }

    public void ToSpawnZoom()
    {
        StartCoroutine(RotateCamera());
        cinematicActive = true; // Level introduction.
        SetCamClipPlane();
        lerpDuration = 4f;
        zoomDestination = minZoom;
        camCooldown = 1f;
        StartCoroutine(Zoom(lerpDuration, zoomDestination, camCooldown));
    }

    public void ToGameCamera()
    {
        cinematicActive = false; // Level introduction.
        StartCoroutine(UserZoomBuffer());
        lerpDuration = 1f;
        zoomDestination = gameModeZoom;
        camCooldown = 1f;
        StartCoroutine(Zoom(lerpDuration, zoomDestination, camCooldown));
    }

    public void ToCutsceneZoom()
    {
        cinematicActive = true; // Level introduction.
        SetCamClipPlane();
        lerpDuration = 1f;
        zoomDestination = minZoom;
        camCooldown = 1f;
        StartCoroutine(Zoom(lerpDuration, zoomDestination, camCooldown));
    }

    [SerializeField] private float rotationSpeedMultiplier = 1f;
    [SerializeField] float rotationDuration = 1;

    IEnumerator RotateCamera()
    {
        while (cinematicActive)
        {

            float currentRotation = MainCam.transform.eulerAngles.y;
            float endRotation = currentRotation + 360.0f;

            float timeElapsed = 0;

            while (timeElapsed < rotationDuration)

            {
                timeElapsed += Time.deltaTime;

                float yRotation = Mathf.Lerp(currentRotation, endRotation, timeElapsed / rotationDuration * rotationSpeedMultiplier) % 360.0f;

                transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation,
                transform.eulerAngles.z);

                yield return null;

            }

        } if (!cinematicActive)
        {
            ResetRotation();
            yield break;
        }
       
    }

    IEnumerator ResetRotation()
    {
        float lerpDuration = 5f;

        float x = 15f;
        float y = 306f;
        float z = 0f;

        Quaternion rotationDestination = Quaternion.Euler(x, y, z);

        Quaternion currentRotation = MainCam.transform.rotation;

        float timeElapsed = 0;

        while (timeElapsed < lerpDuration)

        {
            MainCam.transform.rotation = Quaternion.Lerp(currentRotation, rotationDestination, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;

            yield return null;

        }

        if (timeElapsed >= lerpDuration)
        {
            yield break;
        }
    }

    IEnumerator UserZoomBuffer()
    {
        float zoomBuffer = 4f;
        yield return new WaitForSeconds(zoomBuffer); // buffer until player can take control of the scrollbar.
        AllowUserZoom();
        yield break;
    }

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

    public void FixedUpdate()
    {

        if (MoveCamera == true)
        {
            // Smooth camera follow.

            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y + camFollowOffset.y, target.position.z + camFollowOffset.z);
            Vector3 SmoothedPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed);
            transform.position = SmoothedPosition;

        }
    }

    // Camera 'zoom' into target.

    public float currentZoom;
    private float zoomDestination;

    public float timeElapsed;
    public float lerpDuration = 5;
 

    IEnumerator Zoom(float lerpDuration, float zoomDestination, float camCooldown)

    {

        float timeElapsed = 0;

        while (timeElapsed < lerpDuration)

        {
            if (useMainCam)
            {
                MainCam.orthographicSize = Mathf.Lerp(currentZoom, zoomDestination, timeElapsed / lerpDuration);
            } else
            {
                RPCam.perspective = Mathf.Lerp(currentZoom, zoomDestination, timeElapsed / lerpDuration);
            }

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

        ToGameCamera();
        yield break;
    }

}





