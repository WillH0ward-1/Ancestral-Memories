using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Transform[] bones;
    [SerializeField] private HumanAI humanAI;
    private Animator animator;

    [SerializeField] private float massMultiplier = 25f;
    [SerializeField] private float staticFriction = 1000f;
    [SerializeField] private float dynamicFriction = 1000f;
    [SerializeField] private float bounciness = 0f;
    [SerializeField] private float drag = 1000f;
    [SerializeField] private float angularDrag = 1000f;

    public bool isRagdollActive;

    private NavMeshAgent agent;

    private PhysicMaterial physicsMaterial;

    int waterLayer;

    private GameObject hips;

    private AnimationClip StandUpFromFrontClip;
    private AnimationClip StandUpFromBackClip;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == HumanControllerAnimations.OnFront_ToStand_Dazed01)
            {
                StandUpFromFrontClip = clip;
            }
            else if (clip.name == HumanControllerAnimations.OnBack_GetUp01)
            {
                StandUpFromBackClip = clip;
            }
        }

        bones = GetComponentsInChildren<Transform>();
        humanAI = GetComponentInChildren<HumanAI>();
        agent = GetComponentInChildren<NavMeshAgent>();

        physicsMaterial = new PhysicMaterial();
        physicsMaterial.staticFriction = staticFriction;
        physicsMaterial.dynamicFriction = dynamicFriction;
        physicsMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
        physicsMaterial.bounciness = bounciness;

        waterLayer = LayerMask.NameToLayer("Water");

        hips = transform.Find("mixamorig:Hips").gameObject;

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
                rigidbody.mass *= massMultiplier;
                rigidbody.isKinematic = true;
                rigidbody.drag = drag;
                rigidbody.angularDrag = angularDrag;
                rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rigidbody.useGravity = false;
                rigidbody.detectCollisions = true; // enable collision detection

                //rigidbody.maxLinearVelocity 
            }

            var joint = bone.GetComponent<Joint>();
            if (joint != null)
            {
                joint.enablePreprocessing = true;
            }
        }


        standUpBoneTransforms = new BoneTransform[bones.Length];
        ragdollBoneTransforms = new BoneTransform[bones.Length];

        for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
        {
            standUpBoneTransforms[boneIndex] = new BoneTransform();
            ragdollBoneTransforms[boneIndex] = new BoneTransform();

        }

        PopulateAnimationStartBoneTransforms();

        hipsRigidBody = hips.transform.GetComponent<Rigidbody>();
    }

    private void PopulateBones(BoneTransform[] boneTransforms)
    {
        for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
        {
            boneTransforms[boneIndex].Position = bones[boneIndex].localPosition;
            boneTransforms[boneIndex].Rotation = bones[boneIndex].localRotation;
        }
    }

    private void PopulateAnimationStartBoneTransforms()
    {
        Vector3 positionBeforeSampling = transform.position;
        Quaternion rotationBeforeSampling = transform.rotation;

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == HumanControllerAnimations.OnFront_ToStand_Dazed01 || clip.name == HumanControllerAnimations.OnBack_GetUp01)
            {
                clip.SampleAnimation(gameObject, 0);
                PopulateBones(standUpBoneTransforms);
                break;
            }
        }

        transform.position = positionBeforeSampling;
        transform.rotation = rotationBeforeSampling;
    }


    private Rigidbody hipsRigidBody;

    private void Start()
    {
        //StartCoroutine(TriggerRagdollTest());
    }

    public IEnumerator TriggerRagdoll()
    {
        yield return new WaitForSeconds(1f);

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
                collider.enabled = true;
            }

            var rigidbody = bone.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = false;
                rigidbody.useGravity = true;
                //rigidbody.AddForce(Vector3.down, ForceMode.Impulse);
            }

            isRagdollActive = true;
        }



        Debug.Log("Ragdoll!");
        StartCoroutine(KnockOutBuffer());

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

    private class BoneTransform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    } 

    private BoneTransform[] standUpBoneTransforms;
    private BoneTransform[] ragdollBoneTransforms;

    [SerializeField] private LayerMask groundLayerMask;

    public void DisableRagdoll()
    {
        AlignPositionToHips();
        PopulateBones(ragdollBoneTransforms);

        ResettingBonesBehaviour();

        Debug.Log("Getting Up!");
        bool isFacingUpward = false;

        // Perform a raycast downwards from the specified transform to check if the character is facing upward or downward
        RaycastHit hit;

        if (Physics.Raycast(hips.transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayerMask))
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

    private void AlignPositionToHips()
    {
        Vector3 originalPos = hips.transform.position;
        transform.position = hips.transform.position;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
        {
            transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
        }

        hips.transform.position = originalPos;
    }

    float _elapsedResetBonesTime = 0f;
    float _timeToResetBones = 5f;

    private void ResettingBonesBehaviour()
    {
        _elapsedResetBonesTime += Time.deltaTime;
        float elapsedPercentage = _elapsedResetBonesTime / _timeToResetBones;

        for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
        {
            bones[boneIndex].localPosition = Vector3.Lerp(
                ragdollBoneTransforms[boneIndex].Position,
                standUpBoneTransforms[boneIndex].Position,
                elapsedPercentage);

            bones[boneIndex].localRotation = Quaternion.Lerp(
                ragdollBoneTransforms[boneIndex].Rotation,
                standUpBoneTransforms[boneIndex].Rotation,
                elapsedPercentage);
        }
    }

}
