using System.Collections;
using System.Collections.Generic;
using ProceduralModeling;
using UnityEngine;

public class TreeFruitManager : MonoBehaviour
{
    public GameObject fruitPrefab; // Assign this in the inspector
    public Player player;
    public List<GameObject> fruits; // Stores the pooled fruit game objects
    public int maxFruits; // Maximum number of fruits we want to instantiate
    public float growTime = 3f;
    public float lifetime;
    public float minDecayTime = 3f;
    public float maxDecayTime = 6f;
    private ProceduralTree proceduralTree;

    // Scale parameters
    public Vector3 minFruitScale = Vector3.zero;
    public Vector3 maxFruitScale = Vector3.one;

    private LayerMask hitGroundLayer;
    private Queue<GameObject> fruitPool; // Pool of inactive fruit game objects

    public MapObjGen mapObjGen;

    public bool isBearingFruit = false;

    private void Awake()
    {
        hitGroundLayer = LayerMask.GetMask("Ground", "Water", "Rocks", "Cave");
        fruitPool = new Queue<GameObject>();
    }

    // Declare a Dictionary to hold the references to the FoodAttributes of each fruit
    public Dictionary<GameObject, FoodAttributes> fruitAttributesDict;

    public void InitializeFruits(int maxFruits)
    {
        fruits = new List<GameObject>();
        fruitPool = new Queue<GameObject>();
        fruitAttributesDict = new Dictionary<GameObject, FoodAttributes>(); // Initialize the dictionary

        for (int i = 0; i < maxFruits; i++)
        {
            GameObject fruit = Instantiate(fruitPrefab);
            fruit.SetActive(false);
            FoodAttributes foodAttributes = fruit.GetComponentInChildren<FoodAttributes>();
            foodAttributes.treeFruitManager = this;
            DisableFruitGravity(fruit);
            fruits.Add(fruit);
            fruitPool.Enqueue(fruit);

            fruitAttributesDict[fruit] = foodAttributes; // Store the reference to the FoodAttributes component in the dictionary
        }
    }



    public void SpawnFruits(List<Vector3> fruitPoints)
    {
        if (Random.value > 0.5f)
        {
            isBearingFruit = false;
            return;
        }

        int fruitCount = Mathf.Min(maxFruits, fruitPoints.Count);

        // Deactivate any existing active fruits and add them back to the pool
        foreach (GameObject fruit in fruits)
        {
            if (fruit.activeSelf)
            {
                fruit.SetActive(false);
                StopCoroutine(GrowFruit(fruit, fruit.transform));
                DisableFruitGravity(fruit);
                fruitPool.Enqueue(fruit);
            }
        }

        // Spawn fruits at the fruit points
        for (int i = 0; i < fruitCount; i++)
        {
            if (fruitPool.Count == 0)
            {
                // If the fruit pool is empty, break the loop
                break;
            }

            GameObject fruit = fruitPool.Dequeue();

            if (fruitAttributesDict.TryGetValue(fruit, out FoodAttributes foodAttributes))
            {
                foodAttributes.isDead = false;
            }

            ResetFruit(fruit, fruitPoints[i]); // Reset the fruit's state and set its position
            fruit.SetActive(true);
            StartCoroutine(GrowFruit(fruit, fruit.transform));
        }
    }

    private IEnumerator GrowFruit(GameObject fruit, Transform fruitRoot)
    {
        float timeElapsed = 0;
        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = maxFruitScale;

        fruit.transform.localScale = Vector3.zero;

        while (timeElapsed < growTime)
        {
            fruit.transform.localScale = Vector3.Lerp(initialScale, targetScale, timeElapsed / growTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        fruit.transform.localScale = targetScale;

        yield break;
    }

    private IEnumerator Lifetime(GameObject fruit, Transform fruitRoot)
    {
        lifetime = Random.Range(10f, 20f);
        yield return new WaitForSeconds(lifetime);

        StartCoroutine(Fall(fruit, fruitRoot));

        yield break;
    }

    public IEnumerator Fall(GameObject fruit, Transform fruitRoot)
    {
        fruit.transform.SetParent(null);

        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;

        Collider collider = fruit.transform.GetComponent<Collider>();
        Rigidbody rigidBody = fruit.transform.GetComponent<Rigidbody>();

        collider.providesContacts = true;

        if (!rigidBody.useGravity)
        {
            EnableFruitGravity(fruit);

            // Start checking collision with ground or water
            CheckCollision(fruit, collider);
        }

        yield break;
    }

    private void CheckCollision(GameObject fruit, Collider fruitCollider)
    {
        fruitCollider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); // Temporarily change the layer to ignore raycasts

        // Add the CollisionNotifier component if it doesn't exist
        CollisionNotifier collisionNotifier = fruitCollider.gameObject.GetComponent<CollisionNotifier>();
        if (collisionNotifier == null)
        {
            collisionNotifier = fruitCollider.gameObject.AddComponent<CollisionNotifier>();
        }

        // Subscribe to the collision event
        collisionNotifier.OnCollisionEnterEvent.AddListener(OnCollision);

        // Unsubscribe from the collision event
        void OnCollision(Collision collision)
        {
            if (IsCollisionWithGround(collision))
            {
                DisableFruitGravity(fruit);
                mapObjGen.foodSourcesList.Add(fruit);
                StartCoroutine(Decay(fruit));

                if (collision.collider.CompareTag("Water"))
                {
                    CheckWaterDepth(fruit);
                }

                // Unsubscribe from the collision event
                collisionNotifier.OnCollisionEnterEvent.RemoveListener(OnCollision);
                fruitCollider.gameObject.layer = LayerMask.NameToLayer("Food"); // Restore the original layer
            }
        }

        // Unsubscribe from the collision event in the OnDestroy method
        void OnDestroy()
        {
            collisionNotifier.OnCollisionEnterEvent.RemoveListener(OnCollision);
        }
    }

    private bool IsCollisionWithGround(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (hitGroundLayer == (hitGroundLayer | (1 << contact.otherCollider.gameObject.layer)))
            {
                return true;
            }
        }
        return false;
    }

    private void CheckWaterDepth(GameObject fruit)
    {
        float floatDepthThreshold = 1f;

        if (Physics.Raycast(fruit.transform.position, -fruit.transform.up, out RaycastHit hit, Mathf.Infinity, hitGroundLayer))
        {
            float distanceToGround = hit.distance;

            if (distanceToGround >= floatDepthThreshold)
            {
                WaterFloat floating = fruit.GetComponent<WaterFloat>();
                if (floating != null)
                {
                    floating.enabled = true;
                    StartCoroutine(floating.Float(fruit));
                }
            }
            else
            {
                WaterFloat floating = fruit.GetComponent<WaterFloat>();
                if (floating != null)
                {
                    floating.enabled = false;
                }
            }
        }
    }

    public float waitToDecay = 5f;

    public float DetermineFruitDecay()
    {
        float fruitDecayBuffer = player.faith / 10;

        return fruitDecayBuffer;
    }

    public IEnumerator Decay(GameObject fruit)
    {
        float decayBuffer = Random.Range(minDecayTime, maxDecayTime);
        float timeElapsed = 0;

        waitToDecay = DetermineFruitDecay();

        while (timeElapsed < waitToDecay)
        {
            yield return null;
        }


        Vector3 initialScale = fruit.transform.localScale;  // Get the current scale of the fruit
        Vector3 targetScale = Vector3.zero;  // Set the target scale to zero

        while (timeElapsed < decayBuffer)
        {
            fruit.transform.localScale = Vector3.Lerp(initialScale, targetScale, timeElapsed / decayBuffer);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        mapObjGen.foodSourcesList.Remove(fruit);

        fruit.transform.localScale = targetScale;
        fruit.SetActive(false);

        if (fruitAttributesDict.TryGetValue(fruit, out FoodAttributes foodAttributes))
        {
            foodAttributes.isDead = true;
        }

        fruitPool.Enqueue(fruit); // Add the fruit back to the pool for reuse

        yield break;
    }

    public void ClearFruits()
    {
        // Deactivate and enqueue all active fruits back into the pool
        foreach (GameObject fruit in fruits)
        {
            if (fruit.activeSelf)
            {
                fruit.SetActive(false);
                StopCoroutine(GrowFruit(fruit, fruit.transform));
                DisableFruitGravity(fruit);
                fruitPool.Enqueue(fruit);
            }
        }
    }

    private void ResetFruit(GameObject fruit, Vector3 position)
    {
        fruit.transform.position = position;
        fruit.transform.localScale = Vector3.zero;
    }

    private void EnableFruitGravity(GameObject fruit)
    {
        Rigidbody rigidBody = fruit.GetComponent<Rigidbody>();
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
    }

    private void DisableFruitGravity(GameObject fruit)
    {
        Rigidbody rigidBody = fruit.GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.isKinematic = true;
    }
}
