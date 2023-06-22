using System.Collections;
using UnityEngine;

namespace ProceduralModeling
{
    [RequireComponent(typeof(MeshRenderer))]
    public class PTGrowing : MonoBehaviour
    {
        private Material material;

        private bool isDead = false;
        public bool isGrowing = false;
        public bool isFullyGrown = false;

        public float minLifeTimeSeconds = 10f;
        public float maxLifeTimeSeconds = 25f;

        public float lifeTimeSecs = 60f;

        private const string kGrowingKey = "_T";

        public float minGrowBuffer = 10f;
        public float maxGrowBuffer = 60f;
        public float growBuffer = 0f;

        public float minGrowDuration = 30f;
        public float maxGrowDuration = 45f;
        private float growDuration = 0f;

        public int minGrowKey = 0;
        public int maxGrowKey = 1;

        private ProceduralTree proceduralTree; // Assume you have the reference to the ProceduralTree

        private TreeData treeData;

        private LeafScaler leafScaler;

        private void Awake()
        {
            proceduralTree = GetComponentInChildren<ProceduralTree>();
            material = GetComponentInChildren<Renderer>().material;
            treeData = proceduralTree.Data;
            material.SetFloat(kGrowingKey, minGrowKey);

            leafScaler = GetComponent<LeafScaler>();
            leafScaler.SetLeafScale(0);
        }

        public void GrowTree()
        {
            float lifeTimeSeconds = Random.Range(minLifeTimeSeconds, maxLifeTimeSeconds);
            lifeTimeSecs = lifeTimeSeconds;

            treeData.randomSeed = Random.Range(0, int.MaxValue);

            StartCoroutine(GrowBuffer());
        }

        public IEnumerator GrowBuffer()
        {
            float time = 0f;
            growBuffer = Random.Range(minGrowBuffer, maxGrowBuffer);

            while (time < 1f)
            {
                time += Time.deltaTime / growBuffer;

                yield return null;
            }

            StartCoroutine(IGrowing());

            yield break;
        }

        private IEnumerator IGrowing()
        {
            float time = 0f;
            isDead = false;
            growDuration = Random.Range(minGrowDuration, maxGrowDuration);
            growDuration *= 1000;

            while (time < 1f)
            {
                if (!isDead)
                {
                    isGrowing = true;
                    float currentGrowing = material.GetFloat(kGrowingKey);
                    float newGrowing = Mathf.Lerp(currentGrowing, maxGrowKey, time);
                    material.SetFloat(kGrowingKey, newGrowing);
                    time += Time.deltaTime / growDuration;
                }

                yield return null;
            }

            isFullyGrown = true;
            StartCoroutine(leafScaler.GrowLeaves(leafScaler.minGrowthScale, leafScaler.maxGrowthScale, leafScaler.lerpSpeed));

            yield break;
        }

        public float minDeathDuration = 2f;
        public float maxDeathDuration = 5f;
        private float deathDuration = 1f;

        private IEnumerator Die()
        {
            isGrowing = false;
            isDead = true;

            deathDuration = Random.Range(minDeathDuration, maxDeathDuration);

            float time = 0;
            float currentGrowing = material.GetFloat(kGrowingKey);

            while (time < 1f)
            {
                if (isDead)
                {
                    float newGrowing = Mathf.Lerp(currentGrowing, minGrowKey, time);
                    material.SetFloat(kGrowingKey, newGrowing);
                    time += Time.deltaTime / deathDuration;
                }

                yield return null;
            }

            Revive();

            yield break;
        }

        public void CutDown()
        {
            StopCoroutine(leafScaler.GrowLeaves(0f, 0f, 0f));
            leafScaler.SetLeafScale(0);

            StartCoroutine(Die());
            isDead = true;
        }

        public void Revive()
        {
            GrowTree();
            StartCoroutine(leafScaler.GrowLeaves(leafScaler.minGrowthScale, leafScaler.maxGrowthScale, leafScaler.lerpSpeed));
        }

        private void OnDestroy()
        {
            if (material != null)
            {
                Destroy(material);
                material = null;
            }
        }
    }
}
