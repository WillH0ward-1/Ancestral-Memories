using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Transform[] bones;
    [SerializeField] private HumanAI humanAI;
    private Animator animator;

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
    }

    private IEnumerator Test()
    {
        yield return new WaitForSeconds(5);
        EnableRagdoll();
        yield break;
    }

    private void Start()
    {
        StartCoroutine(Test());
    }

    public void EnableRagdoll()
    {
        humanAI.StartCoroutine(humanAI.StopAllBehaviours());
        agent.enabled = false;
        animator.enabled = false;

        foreach (var bone in bones)
        {
            var collider = bone.GetComponent<Collider>();
            if (collider != null)
            {
                collider.material = physicsMaterial;
                collider.providesContacts = true;
                collider.enabled = true;
                collider.excludeLayers = waterLayer;
                
            }

            var rigidbody = bone.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = false;
                rigidbody.drag = drag;
                rigidbody.excludeLayers = waterLayer;
                rigidbody.angularDrag = angularDrag;
                //rigidbody.interpolation = 0;
            }
        }

        isRagdollActive = true;
    }


    public void DisableRagdoll()
    {
        agent.enabled = true;
        animator.enabled = true;

        foreach (var bone in bones)
        {
            var rigidbody = bone.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = true;
            }
        }

        isRagdollActive = false;
    }
}
