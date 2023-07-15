using SimplestarGame.Splash;
using UnityEngine;

namespace SimplestarGame.Wave
{
    [RequireComponent(typeof(Rigidbody))]
    public class WaveInterferer : MonoBehaviour
    {
        [SerializeField] WaveType waveType = WaveType.Point;
        [SerializeField, Range(0.01f, 1f)] float radius = 0.2f;
        [SerializeField, Range(0.01f, 20f)] float length = 0.5f;

        private Vector3 velocity;
        private Vector3 lastPoint;
        private int waterLayer;

        private readonly string SphereYName = "SphereY";
        private readonly string Sphere_YName = "Sphere_Y";
        private Transform sphereY = null;
        private Transform sphere_Y = null;

        void Start()
        {
            lastPoint = transform.position;
            waterLayer = LayerMask.NameToLayer("Water");
            InitSphereYs();
        }

        void OnValidate()
        {
            if (waveType != WaveType.Point)
            {
                InitSphereYs();
                if (null == sphereY || null == sphere_Y)
                {
                    waveType = WaveType.Point;
                }
                else
                {
                    float r = Mathf.Max(0.01f, radius);
                    sphereY.localScale = sphere_Y.localScale = new Vector3(1f, r / length, 1f);
                    transform.localScale = new Vector3(r, length, r);
                }
            }
            else
            {
                float r = Mathf.Max(0.01f, radius);
                transform.localScale = Vector3.one * radius;
            }
        }

        void InitSphereYs()
        {
            foreach (Transform child in transform)
            {
                if (SphereYName == child.name)
                {
                    sphereY = child;
                }
                if (Sphere_YName == child.name)
                {
                    sphere_Y = child;
                }
            }
        }

        void FixedUpdate()
        {
            velocity = (transform.position - lastPoint) / Time.deltaTime;
            lastPoint = transform.position;
        }

        void OnTriggerEnter(Collider other)
        {
            var waveComputer = other.gameObject.GetComponent<WaveSimulator>();
            if (null == waveComputer)
            {
                return;
            }
            if (0 < Vector3.Dot(velocity, other.transform.up))
            {
                return;
            }
            var normalizedVelocity = velocity.normalized;
            if (Physics.Raycast(transform.position - velocity * 0.5f, normalizedVelocity, out RaycastHit hit, Vector3.Distance(Vector3.zero, velocity), (1 << waterLayer)))
            {
                switch (waveType)
                {
                    case WaveType.Point:
                        {
                            waveComputer.AddWavePoint(hit.point, radius, velocity);
                        }
                        break;
                    case WaveType.Line:
                        {
                            waveComputer.AddWaveLine(new Vector3(sphereY.position.x, hit.point.y, sphereY.position.z),
                                new Vector3(sphere_Y.position.x, hit.point.y, sphere_Y.position.z),
                                radius, velocity);
                        }
                        break;
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            var waveComputer = other.gameObject.GetComponent<WaveSimulator>();
            if (null == waveComputer)
            {
                return;
            }
            if (0 > Vector3.Dot(velocity, other.transform.up))
            {
                return;
            }
            var normalizedVelocity = velocity.normalized;
            if (Physics.Raycast(transform.position + velocity * 0.5f, -normalizedVelocity, out RaycastHit hit, Vector3.Distance(Vector3.zero, velocity), (1 << waterLayer)))
            {
                switch (waveType)
                {
                    case WaveType.Point:
                        {
                            waveComputer.AddWavePoint(hit.point, radius, velocity);
                        }
                        break;
                    case WaveType.Line:
                        {
                            waveComputer.AddWaveLine(new Vector3(sphereY.position.x, hit.point.y, sphereY.position.z),
                                new Vector3(sphere_Y.position.x, hit.point.y, sphere_Y.position.z),
                                radius, velocity);
                        }
                        break;
                }
            }
        }

        public enum WaveType
        {
            Point = 0,
            Line,
        }
    }
}
