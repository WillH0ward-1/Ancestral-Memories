using System.Collections;
using UnityEngine;

namespace ProceduralModeling
{
    [RequireComponent(typeof(MeshRenderer))]
    public class PTGrowing : MonoBehaviour
    {
        private Material material;

        private bool isDead;
        public bool isGrowing;

        public float minLifeTimeSeconds = 10f;
        public float maxLifeTimeSeconds = 25f;

        public float lifeTimeSecs = 60f;

        private const string kGrowingKey = "_T";

        public float minGrowBuffer = 10f;
        public float maxGrowBuffer = 60f;

        private ProceduralTree proceduralTree; // Assume you have the reference to the ProceduralTree

        private TreeData treeData;

        public float minLengthAttenuation = 0f;
        public float maxLengthAttenuation = 0.95f;

        private void Awake()
        {
            proceduralTree = GetComponentInChildren<ProceduralTree>();
            treeData = proceduralTree.Data;

            treeData.lengthAttenuation = minLengthAttenuation;
        }
        private void OnEnable()
        {
            material = GetComponent<MeshRenderer>().material;
            material.SetFloat(kGrowingKey, 0f);
            isDead = true;
        }

        public void GrowTree()
        {
            float lifeTimeSeconds = Random.Range(minLifeTimeSeconds, maxLifeTimeSeconds);
            lifeTimeSecs = lifeTimeSeconds;

            treeData.randomSeed = Random.Range(0, int.MaxValue);

            StartCoroutine(GrowBuffer(lifeTimeSeconds));
        }

        public IEnumerator GrowBuffer(float lifeTimeSeconds)
        {
            float time = 0f;
            float duration = Random.Range(minGrowBuffer, maxGrowBuffer);

            while (time < 1f)
            {
                time += Time.deltaTime / duration;

                yield return null;
            }

            StartCoroutine(IGrowing(lifeTimeSeconds));

            yield break;
        }

        private IEnumerator IGrowing(float duration)
        {
            float time = 0f;
            isDead = false;

            while (time < 1f)
            {
                if (!isDead)
                {
                    treeData.lengthAttenuation = Mathf.Lerp(minLengthAttenuation, maxLengthAttenuation, time);
                   // material.SetFloat(kGrowingKey, treeData.lengthAttenuation);
                    time += Time.deltaTime / duration;
                } 

                yield return null;
            }

            treeData.lengthAttenuation = maxLengthAttenuation;

            if (!isDead)
            {
                StartCoroutine(Die());

                yield break;
            }

            yield break;
        }

        public float minDeathDuration = 2f;
        public float maxDeathDuration = 5f;
        public float deathDuration = 1;

        private IEnumerator Die()
        {
            isDead = true;

            deathDuration = Random.Range(minDeathDuration, maxDeathDuration);
            float time = 0;

            float currentLength = treeData.lengthAttenuation;

            while (time < 1f)
            {
                if (isDead)
                {
                    treeData.lengthAttenuation = Mathf.Lerp(currentLength, minLengthAttenuation, time / deathDuration);
                    //material.SetFloat(kGrowingKey, treeData.lengthAttenuation);
                    time += Time.deltaTime / deathDuration;
                }

                yield return null;
            }

            treeData.lengthAttenuation = minLengthAttenuation;

            Revive();

            yield break;
        }


        public void CutDown()
        {
            StartCoroutine(Die());
            isDead = true;
        }

        public void Revive()
        {
            GrowTree();


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
