using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitControl : MonoBehaviour
{
    [SerializeField] private GameObject fruitPrefab;
    [SerializeField] private int fruitPoolSize = 10;
    [SerializeField] private float growTime = 1f;
    [SerializeField] private float lifetime = 5f;

    [SerializeField] private List<Transform> fruitRoots = new List<Transform>();
    [SerializeField] private List<GameObject> fruitList = new List<GameObject>();
    [SerializeField] private List<GameObject> fruitPool = new List<GameObject>();

    private ScaleControl scaleControl;

    private bool poolSetup = false;

    private void SetupFruitPool()
    {
        poolSetup = true;

        scaleControl = transform.GetComponent<ScaleControl>();

        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag("FruitRoot"))
            {
                fruitRoots.Add(child);
                GameObject fruit = Instantiate(fruitPrefab, transform.position, Quaternion.identity, child);
                fruit.SetActive(false);
                fruitPool.Add(fruit);
            }
        }

        foreach (Transform root in fruitRoots)
        {
            GameObject fruit = GetFruitFromPool();

            fruit.transform.SetParent(root);
            fruit.transform.position = root.transform.position;

            Rigidbody rigidBody = fruit.transform.GetComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;

            fruit.SetActive(true);

            fruitList.Add(fruit);
        }
    }

    float appleGrowThreshold = 0.8f;


    public IEnumerator FruitGrowthBuffer()
    {
        if (!poolSetup)
        {
            SetupFruitPool();
        }

        // Reset the fruits before starting the growth process
        foreach (GameObject apple in fruitList)
        {
            Rigidbody rigidBody = apple.transform.GetComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;

            apple.SetActive(false);
            apple.transform.localScale = Vector3.zero;
        }

        for (int i = 0; i < fruitList.Count; i++)
        {
            GameObject apple = fruitList[i];

            while (scaleControl.growthPercent <= appleGrowThreshold)
            {
                yield return null;
            }

            if (i < fruitRoots.Count)
            {
                Transform root = fruitRoots[i];

                apple.transform.SetParent(root);
                apple.transform.position = root.transform.position;

                Rigidbody rigidBody = apple.transform.GetComponent<Rigidbody>();
                rigidBody.useGravity = false;
                rigidBody.isKinematic = true;

                apple.SetActive(true);

                StartCoroutine(GrowFruit(apple, apple.transform.parent));
            }
        }
    }


    public IEnumerator StopFruitGrowth()
    {
        StopCoroutine(GrowFruit(null,null));
        StopCoroutine(Lifetime(null,null));

        yield break;
    }

    private GameObject GetFruitFromPool()
    {
        foreach (GameObject fruit in fruitPool)
        {
            if (!fruit.activeInHierarchy)
            {
                return fruit;
            }
        }

        return null;
    }

    private IEnumerator GrowFruit(GameObject fruit, Transform fruitRoot)
    {
        float timeElapsed = 0;
        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;

        fruit.transform.localScale = Vector3.zero;

        while (timeElapsed < growTime)
        {
            fruit.transform.localScale = Vector3.Lerp(initialScale, targetScale, timeElapsed / growTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        fruit.transform.localScale = targetScale;

        StartCoroutine(Lifetime(fruit, fruitRoot));

        yield break;
    }

    private IEnumerator Lifetime(GameObject fruit, Transform fruitRoot)
    {
        lifetime = Random.Range(10f, 20f);
        yield return new WaitForSeconds(lifetime);

        StartCoroutine(Fall(fruit, fruitRoot));

        yield break;
    }

    float minDecayTime = 3f;
    float maxDecayTime = 6f;

    private IEnumerator Fall(GameObject fruit, Transform fruitRoot)
    {
        fruit.transform.SetParent(null);

        float timeElapsed = 0;
        Vector3 initialScale = transform.localScale; 
        Vector3 targetScale = Vector3.zero;

        Collider collider = fruit.transform.GetComponent<Collider>();
        HitGround hitGround = fruit.GetComponent<HitGround>();
        
        Rigidbody rigidBody = fruit.transform.GetComponent<Rigidbody>();

        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;

        while (!hitGround.hit)
        {
            yield return null;
        }

        float decayBuffer = Random.Range(minDecayTime, maxDecayTime);

        if (hitGround.hit)
        {
            float time = 0;

            while (time <= decayBuffer)
            {
                time += Time.deltaTime;
                yield return null;
            }

            while (timeElapsed < growTime)
            {
                fruit.transform.localScale = Vector3.Lerp(initialScale, targetScale, timeElapsed / growTime);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            fruit.transform.localScale = targetScale;
            fruit.SetActive(false);

            fruitList.Remove(fruit);
            fruitPool.Add(fruit);

            if (!transform.GetComponent<TreeDeathManager>().treeDead)
            {
                float delayTime = Random.Range(5f, 10f);
                yield return new WaitForSeconds(delayTime);

                yield break;
            }

            yield break;
        }
    }
}
