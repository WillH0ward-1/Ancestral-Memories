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
    private Camera ZoomCamera;

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

    // Update is called once per frame

    public void Start()
    {

        playerSpawning = true;

        cinematicActive = false; // Level introduction.

        gameStarted = true;

        ZoomCamera.orthographicSize = spawnZoomDistance;

        ToSpawnZoom();
    }

    void SetCamClipPlane()
    {
        if (cinematicActive == true)
        {
            ZoomCamera.farClipPlane = 5000;
            ZoomCamera.nearClipPlane = -5000;
        }

        else if (cinematicActive == false)
        {
            ZoomCamera.farClipPlane = 300;
            ZoomCamera.nearClipPlane = -300;
        }
    }

    public void ToSpawnZoom()
    {
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
            if (ZoomCamera.orthographic)
            {
                ZoomCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed; // Use this when using Orthographic Camera.
            }
            else
            {
                ZoomCamera.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed; // Otherwise, use this (Perspective Camera).
            }

            if (ZoomCamera.orthographicSize >= maxZoom)
            {
                ZoomCamera.orthographicSize = maxZoom;
            }

            if (ZoomCamera.orthographicSize <= minZoom)
            {
                ZoomCamera.orthographicSize = minZoom;
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
        currentZoom = ZoomCamera.orthographicSize;

        float timeElapsed = 0;

        while (timeElapsed < lerpDuration)

        {
            ZoomCamera.orthographicSize = Mathf.Lerp(currentZoom, zoomDestination, timeElapsed / lerpDuration);
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





