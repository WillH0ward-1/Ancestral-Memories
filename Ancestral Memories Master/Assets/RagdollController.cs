using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Transform[] bones;
    [SerializeField] private HumanAI humanAI;
    private Animator animator;

    [SerializeField] private float fallThresholdAngle = 60f; // in degrees
    [SerializeField] private float staticFriction = 1000f;
    [SerializeField] private float dynamicFriction = 1000f;
    [SerializeField] private float bounciness = 0f;
    [SerializeField] private float drag = 1000f;
    [SerializeField] private float angularDrag = 1000f;

    public bool isRagdollActive;

    private NavMeshAgent agent;

    private PhysicMaterial physicsMaterial;

    int waterLayer;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        bones = GetComponentsInChildren<Transform>();
        humanAI = GetComponentInChildren<HumanAI>();
        agent = GetComponentInChildren<NavMeshAgent>();

        physicsMaterial = new PhysicMaterial();
        physicsMaterial.staticFriction = staticFriction;
        physicsMaterial.dynamicFriction = dynamicFriction;
        physicsMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
        physicsMaterial.bounciness = bounciness;

        waterLayer = LayerMask.NameToLayer("Water");

        foreach (var bone in bones)
        {
            var collider = bone.GetComponent<Collider>();
            if (collider != null)
            {
                collider.material = physicsMaterial;
                //collider.providesContacts = false;
                //collider.enabled = false;

            }

            var rigidbody = bone.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.mass *= 16;
                rigidbody.isKinematic = true;
                rigidbody.drag = drag;
                rigidbody.angularDrag = angularDrag;
                rigidbody.interpolation = RigidbodyInterpolation.None;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rigidbody.useGravity = false;
                rigidbody.detectCollisions = true; // enable collision detection

                //rigidbody.maxLinearVelocity 
            }
        }
    }

    private void Start()
    {
        StartCoroutine(TriggerRagdollTest());
    }

    private IEnumerator TriggerRagdollTest()
    {
        yield return new WaitForSeconds(5);

        EnableRagdoll();
    }

    public void EnableRagdoll()
    {

        humanAI.StartCoroutine(humanAI.StopAllBehaviours());

        foreach (var bone in bones)
        {
            var collider = bone.GetComponent<Collider>();
            if (collider != null)
            {
                collider.providesContacts = true;
                collider.enabled = true;
            }

            var rigidbody = bone.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = false;
                rigidbody.useGravity = true;
            }

            isRagdollActive = true;
        }

        Debug.Log("Ragdoll!");

    }

    public bool KnockedOut = false;

    public float KOtimer= 10f;
    public float minKOtime = 5f;
    public float maxKOtime = 10f;

    private IEnumerator KnockOutBuffer()
    {
        yield return new WaitForSeconds(Random.Range(3, 5));
        /*
        foreach (var bone in bones)
        {
            var rigidbody = bone.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;
                rigidbody.detectCollisions = false; 
            }
        }
        */

        KnockedOut = true;
        KOtimer = Random.Range(minKOtime, maxKOtime) * 10;

        Debug.Log("Knocked Out!");

        while (KOtimer >= 0)
        {
            KOtimer--;

            yield return null;
        }

        if (KnockedOut || isRagdollActive)
        {
            DisableRagdoll();
        }

        KnockedOut = false;

        yield return null;
    }

    [SerializeField] private Transform raycastTransform;
    [SerializeField] private LayerMask groundLayerMask;

    public void DisableRagdoll()
    {
        Debug.Log("Getting Up!");
        bool isFacingUpward = false;

        // Perform a raycast downwards from the specified transform to check if the character is facing upward or downward
        RaycastHit hit;
        if (Physics.Raycast(raycastTransform.position, Vector3.down, out hit, Mathf.Infinity, groundLayerMask))
        {
            var groundNormal = hit.normal;
            var angle = Vector3.Angle(groundNormal, Vector3.up);
            if (angle < 90)
            {
                Debug.Log("Facing Upwards!");
                isFacingUpward = true;
            }
            else
            {
                Debug.Log("Facing Downwards!");
            }
        }

        if (isFacingUpward)
        {
            humanAI.StartCoroutine(humanAI.GetUp(true));
        }
        else
        {
            humanAI.StartCoroutine(humanAI.GetUp(false));
        }

        isRagdollActive = false;
    }

}
