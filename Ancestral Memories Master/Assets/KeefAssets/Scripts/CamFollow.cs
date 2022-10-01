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

    [SerializeField] private float spawnZoom = 1000;

    [SerializeField] private float minZoom = 6;

    public bool cinematicActive = false;

    public Vector3 camFollowOffset;

    public bool MoveCamera = true;

    public float cineZoomMultiplier = 0.125f;

    private float camLerp;

    public bool gameStarted = true;

    public bool playerSpawning = true;

    public CharacterClass player;


    // Update is called once per frame

    public void Start()
    {

        playerSpawning = true;

        cinematicActive = true; // Level introduction.

        gameStarted = true;

        if (playerSpawning == true)

        {
            ZoomCamera.orthographicSize = spawnZoom;

        } else
        {
            ZoomCamera.orthographicSize = maxZoom;
        }

        TriggerInterval();
    }

    public void TriggerInterval()

    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(4f); // wait for 'x' event to finish before ending cinematic mode.
        cinematicActive = false;
    }

    public void SpawnZoom()
    {
        // Wait for game to load here
        SetCamClipPlane();
        ZoomIn();
    }

    void SetCamClipPlane()
    {
        if (cinematicActive == true)
        {
            ZoomCamera.farClipPlane = 5000;
            ZoomCamera.nearClipPlane = -5000;
        }

        if (cinematicActive == false)
        {
            ZoomCamera.farClipPlane = 300;
            ZoomCamera.nearClipPlane = -300;
        }
    }

    public void Update()
    {
        if (playerSpawning == true)
        {
            SpawnZoom();
        }

        if (cinematicActive == true)
        {
            // Activate Cinematic Mode

            ZoomIn();

        } else if (cinematicActive == false)
        {
            ZoomOut();
            UserZoom();
        }
    }

    public void UserZoom()

    {
        StartCoroutine(UserZoomBuffer());
    }

    IEnumerator UserZoomBuffer()
    {
        yield return new WaitForSeconds(4f); // buffer until player can take control of the scrollbar.
        AllowUserZoom();
    }

    public void AllowUserZoom()
    {
        if (!cinematicActive == true)
        {
            if (ZoomCamera.orthographic)
            {
                ZoomCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed; // Use this when using Orthographic Camera.
            }
            else
            {
                ZoomCamera.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed; // Otherwise, use this (Perspective Camera).
            }

            if (ZoomCamera.orthographicSize > maxZoom)
            {
                ZoomCamera.orthographicSize = maxZoom;
            }

            if (ZoomCamera.orthographicSize < minZoom)
            {
                ZoomCamera.orthographicSize = minZoom;
            }
        }
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

    public float currentZoomVal;
    private float endZoomVal;

    public float timeElapsed;
    public float lerpDuration = 5;

    IEnumerator LerpToZoom()
    {
        currentZoomVal = ZoomCamera.orthographicSize;

        if (cinematicActive == true)

        {
            endZoomVal = minZoom;

        } else if (cinematicActive == false)

        {
            endZoomVal = maxZoom;
        } 

        float timeElapsed = 0;

        while (timeElapsed < lerpDuration)

        {
            ZoomCamera.orthographicSize = Mathf.Lerp(currentZoomVal, endZoomVal, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;

            yield return null;
        } 
    }

    public void ZoomIn()

    {
        StartCoroutine(LerpToZoom());
    }

    public void ZoomOut()

    {
        StartCoroutine(LerpToZoom());
    }
}





