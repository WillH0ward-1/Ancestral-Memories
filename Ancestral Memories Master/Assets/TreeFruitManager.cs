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
    public float minDecayTime = 3f;
    public float maxDecayTime = 6f;

    public Vector3 maxFruitScale = Vector3.one;

    private LayerMask hitGroundLayer;
    private Queue<GameObject> fruitPool; // Pool of inactive fruit game objects

    public MapObjGen mapObjGen;

    public bool isBearingFruit = false;

    private Dictionary<GameObject, Coroutine> growCoroutines = new Dictionary<GameObject, Coroutine>();

    private PTGrowing ptGrow;

    public ResourcesManager resources;

    private void Awake()
    {
        ptGrow = GetComponent<PTGrowing>();

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

    private void AddToResources(GameObject fruit)
    {
        resources.AddResourceObject("Food", fruit);
    }

    private void RemoveFromResources(GameObject fruit)
    {
        resources.RemoveResourceObject("Food", fruit);
    }

    public void SpawnFruits(List<Vector3> fruitPoints)
    {
        Debug.Log($"Player Faith: {player.faith}");

        isBearingFruit = Random.value > 0.5f;
        if (!isBearingFruit)
        {
            Debug.Log("Tree is not bearing fruit this time.");
            return;
        }

        int fruitCount = Mathf.Min(maxFruits, fruitPoints.Count);
        ReturnActiveFruitsToPool();

        // Ensure a minimum percentage of fruits fall regardless of player's faith
        float minFruitFallPercentage = 0.2f; // At least 20% of fruits will fall
        int fruitsToFall = Mathf.Max(
            Mathf.RoundToInt(fruitCount * (player.faith / 100f)),
            Mathf.RoundToInt(fruitCount * minFruitFallPercentage)
        );
        Debug.Log($"Fruits to fall: {fruitsToFall}, Fruit Pool Count: {fruitPool.Count}");

        for (int i = 0; i < fruitsToFall; i++)
        {
            if (fruitPool.Count == 0)
            {
                Debug.LogWarning("Fruit pool is empty, breaking loop.");
                break;
            }
            GameObject fruit = fruitPool.Dequeue();
            fruitAttributesDict[fruit].isDead = false;
            ResetFruit(fruit, fruitPoints[i]);
            fruit.SetActive(true);
            growCoroutines[fruit] = StartCoroutine(GrowFruit(fruit, i, fruitsToFall));
        }
    }




    private void ReturnActiveFruitsToPool()
    {
        foreach (GameObject fruit in fruits)
        {
            if (fruit.activeSelf)
            {
                fruit.SetActive(false);
                if (growCoroutines.TryGetValue(fruit, out Coroutine coroutine))
                {
                    StopCoroutine(coroutine);
                    growCoroutines.Remove(fruit);
                }
                fruit.transform.localScale = Vector3.zero; // Reset scale to zero when returning to pool
                fruitPool.Enqueue(fruit);
            }
        }
    }


    private IEnumerator GrowFruit(GameObject fruit, int index, int fruitCount)
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

        yield return StartCoroutine(Lifetime(fruit, index, fruitCount));
    }

    private IEnumerator Lifetime(GameObject fruit, int index, int totalFruits)
    {
        // Get the remaining lifetime of the tree
        float remainingLifetime = ptGrow.GetRemainingLifetime();

        // Calculate the interval at which each fruit should fall
        float fallInterval = remainingLifetime / totalFruits;

        // Calculate the specific fall time for this fruit based on its index
        float fallTime = fallInterval * index;

        // Wait for the calculated fall time
        yield return new WaitForSeconds(fallTime);

        // Start the fall coroutine
        StartCoroutine(Fall(fruit));
    }

    public IEnumerator Fall(GameObject fruit)
    {
        fruit.transform.SetParent(null);

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
                AddToResources(fruit);
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

    public float minDecayMultiplier = 0.5f; // Faster decay (half the base decay time)
    public float maxDecayMultiplier = 2.0f; // Slower decay (double the base decay time)
    public float baseDecayTime = 5f; // Base decay time in seconds when faith is 50

    public float DetermineFruitDecay()
    {
        float decayMultiplier = Mathf.Lerp(minDecayMultiplier, maxDecayMultiplier, (100f - player.faith) / 100f);

        float decayTime = baseDecayTime * decayMultiplier;

        return decayTime;
    }


    public IEnumerator Decay(GameObject fruit)
    {
        // Wait for a delay before starting the decay process
        yield return new WaitForSeconds(DetermineFruitDecay());

        // Reset the timeElapsed for the decay process
        float timeElapsed = 0;

        // Randomly determine the time it will take for the fruit to decay
        float decayBuffer = Random.Range(minDecayTime, maxDecayTime);

        // Get the current scale of the fruit
        Vector3 initialScale = fruit.transform.localScale;

        // Set the target scale to zero
        Vector3 targetScale = Vector3.zero;

        // Gradually scale down the fruit to simulate decay
        while (timeElapsed < decayBuffer)
        {
            fruit.transform.localScale = Vector3.Lerp(initialScale, targetScale, timeElapsed / decayBuffer);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        RemoveFromResources(fruit);
        fruit.SetActive(false);
        fruitPool.Enqueue(fruit);
    }



    public void ClearFruits()
    {
        ReturnActiveFruitsToPool();
    }

    private void OnDestroy()
    {
        // Make sure to clean up coroutines on destroy
        foreach (var entry in growCoroutines)
        {
            if (entry.Value != null)
            {
                StopCoroutine(entry.Value);
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
