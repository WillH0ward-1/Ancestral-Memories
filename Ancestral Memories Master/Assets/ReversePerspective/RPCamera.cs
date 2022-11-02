using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class RPCamera : MonoBehaviour {

	[Tooltip("objects located here stay the same size on perspective distortion")]
    public Transform pivot = null;

	[Tooltip("zero for parallel projection, negative for reverse perspective")]
	[Range(-10.0f, 10.0f)]
    public float perspective = 0.0f;

	public float distance;

	public Camera rpCam;

	private float m_m00;
	private float m_m11;

    void Reset()
    {
        rpCam = GetComponent<Camera>();
        if (!rpCam.orthographic) {
            rpCam.orthographic = true;
        }

		UpdateProjection(true);
    }

    void Start()
    {
        rpCam = GetComponent<Camera>();

        if (!rpCam.orthographic) {
            throw new System.Exception("Camera projection should be orthographic");
        }

		UpdateProjection(true);
    }


    void OnValidate() // to not set projection on each Update
	{
		rpCam = GetComponent<Camera>();

		UpdateProjection(true);
	}

	public void UpdateProjection(bool prepare)
    {
		if (prepare) {
			PrepareProjection();
		}

		float distance = GetCameraDistance();

		Matrix4x4 matrix = GetProjectionMatrix(-perspective * 0.01f, -distance); // convert 'perspective' from percent

        rpCam.projectionMatrix = matrix;
    }

	private float GetCameraDistance() {
		if (pivot != null) {
			return (pivot.position - this.transform.position).magnitude;
		} else {
			return this.transform.position.magnitude;
		}
	}

	private void PrepareProjection()
	{
		m_m00 = 1f / rpCam.orthographicSize / rpCam.aspect;
		m_m11 = 1f / rpCam.orthographicSize;
	}

	private Matrix4x4 GetProjectionMatrix(float perspective, float distance)
	{
		//  sx   0    0      0
		//  0   sy    0      0
		//  0    0   -0.001  p
		//  0    0    p      1 - d*p
		// 
		//  X =         sx * x  / (1 + p*(z-d))
		//  Z = (p - 0.001 * z) / (1 + p*(z-d))
		// 
		// Here is a discontinuity of projected Z coordinate near p==0
		//  http://www.wolframalpha.com/input/?i=plot+(p+-+0.001*z)%2F(1%2Bp*(z-30))+p%3D-0.5..0.5+z%3D-3..3
		// So Z-buffer may glitch in that place.
		// Workaround is to play with 'perspective' and 'distance' values.

		var m = new Matrix4x4();
		m.m00 =  m_m00;
		m.m11 =  m_m11;
		m.m22 = -0.000001f; m.m23 = perspective;
		m.m32 =  perspective;         m.m33 = 1.0f - distance * perspective;

		return m;
	}


	void OnDrawGizmosSelected()
	{
		Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

		Gizmos.matrix = this.transform.localToWorldMatrix;
		Gizmos.color = new Color(0.6f, 0.6f, 0.9f, 0.5f);

		float d = GetCameraDistance();  
		float p = perspective * 0.01f;
		float s = rpCam.orthographicSize;
		float n = rpCam.nearClipPlane;
		float f = rpCam.farClipPlane;
		float a = rpCam.aspect;
		float nx = s * (1.0f + p * (n-d));
		float fx = s * (1.0f + p * (f-d));

		var points = new [] {
			new Vector3( nx, nx/a, n),
			new Vector3(-nx, nx/a, n),
			new Vector3(-nx,-nx/a, n),
			new Vector3( nx,-nx/a, n),
			new Vector3( fx, fx/a, f),
			new Vector3(-fx, fx/a, f),
			new Vector3(-fx,-fx/a, f),
			new Vector3( fx,-fx/a, f),
			new Vector3(  s,  s/a, d),
			new Vector3( -s,  s/a, d),
			new Vector3( -s, -s/a, d),
			new Vector3(  s, -s/a, d),
		};

		var lines = new [] {
			0,1, 1,2, 2,3, 3,0,
			0,4, 1,5, 2,6, 3,7,
			4,5, 5,6, 6,7, 7,4,
			8,9, 9,10, 10,11, 11,8,
		};
		for (int i = 0; i < lines.Length; i += 2) {
			Gizmos.DrawLine(points[lines[i]], points[lines[i+1]]);
		}

		Gizmos.matrix = oldGizmosMatrix;
	}




}
