using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RPCamera))]
public class RPCameraController : MonoBehaviour
{
	public RPCamera rPCamera = null;

	private Camera m_camera = null;

	void Start() {
		if (rPCamera == null) throw new UnityException ("Perspective Camera not set");
		if (rPCamera.pivot == null) throw new UnityException ("Pivot not set");

		m_camera = GetComponent<Camera>();
		
	}


	void Update() {


		bool prepareProjection = true;
		bool updateProjection  = true;

		if (prepareProjection || updateProjection) {
			rPCamera.UpdateProjection(prepareProjection);
		}

    }

}


public class SmoothValue {
	public float target = 0.0f;
	private float value = 0.0f;
	private float velocity = 0.1f;
	public SmoothValue(float value, float velocity) {
		this.value = this.target = value;
		this.velocity = velocity;
	}
	public bool IsMoving() {
		return Mathf.Abs(target - value) > 0.000001f;  // !!! make configurable
	}
	public float GetValue() {
		value = Mathf.Lerp(value, target, velocity * Time.deltaTime);
		return value;
	}
}

