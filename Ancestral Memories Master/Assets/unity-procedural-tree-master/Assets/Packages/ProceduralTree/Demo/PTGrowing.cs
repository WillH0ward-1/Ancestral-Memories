using System.Collections;
using UnityEngine;

namespace ProceduralModeling
{
    [RequireComponent(typeof(MeshRenderer))]
    public class PTGrowing : MonoBehaviour
    {
        private Material material;
        private ProceduralTree proceduralTree;
        private TreeData treeData;
        private LeafScaler leafScaler;

        public bool isGrowing = false;
        public bool isFullyGrown = false;

        public float minLifeTimeSeconds = 10f;
        public float maxLifeTimeSeconds = 25f;
        public float lifeTimeSecs = 60f;

        private const string kGrowingKey = "_T";

        public float minGrowBuffer = 10f;
        public float maxGrowBuffer = 60f;
        public float minGrowDuration = 30f;
        public float maxGrowDuration = 45f;
        public float minDeathDuration = 2f;
        public float maxDeathDuration = 5f;

        private float growBuffer;
        private float growDuration;
        private float deathDuration;

        private bool isDead = false;

        public enum State
        {
            Buffering,
            Growing,
            Alive,
            Dying,
            Reviving
        }

        public State currentState;
        private float time = 0f;

        private void Awake()
        {
            proceduralTree = GetComponentInChildren<ProceduralTree>();
            material = GetComponentInChildren<Renderer>().material;
            treeData = proceduralTree.Data;
            material.SetFloat(kGrowingKey, 0);

            leafScaler = gameObject.GetComponent<LeafScaler>();

            currentState = State.Buffering;
            time = 0f;
        }

        public void GrowTree()
        {
            currentState = State.Buffering;
            time = 0f;
            treeData.Setup();
            leafScaler.SetLeafScale(leafScaler.minGrowthScale);
            StartCoroutine(GrowBuffer(false));
        }

        private IEnumerator GrowBuffer(bool reseed)
        {
    

            /*
            if (reseed)
            {
                proceduralTree.Rebuild();
            }
            */

            time = 0f;
            growBuffer = Random.Range(minGrowBuffer, maxGrowBuffer);

            while (time < growBuffer)
            {
                time += Time.deltaTime;
                yield return null;
            }

            time = 0f;

            StartCoroutine(Growing());
        }

        private IEnumerator Growing()
        {
            currentState = State.Growing;

            time = 0f;
            isDead = false;
            growDuration = Random.Range(minGrowDuration, maxGrowDuration);

            while (time < growDuration)
            {
                if (!isDead)
                {
                    isGrowing = true;
                    float t = time / growDuration;
                    float newGrowing = Mathf.Lerp(0, 1, t);
                    material.SetFloat(kGrowingKey, newGrowing);
                    time += Time.deltaTime;
                }

                yield return null;
            }

            isGrowing = false;
            isFullyGrown = true;

            leafScaler.LerpScale(leafScaler.minGrowthScale, leafScaler.maxGrowthScale, leafScaler.lerpduration);

            time = 0f;

            StartCoroutine(Lifetime());
        }

        private IEnumerator Lifetime()
        {
            currentState = State.Alive;

            lifeTimeSecs = Random.Range(minLifeTimeSeconds, maxLifeTimeSeconds);

            while (time < lifeTimeSecs)
            {
                time += Time.deltaTime;

                yield return null;
            }

            time = 0f;
            StartCoroutine(Dying());
        }

        private IEnumerator Dying()
        {
            currentState = State.Dying;

            leafScaler.LerpScale(leafScaler.maxGrowthScale, leafScaler.minGrowthScale, leafScaler.lerpduration);

            yield return new WaitForSeconds(leafScaler.lerpduration);

            deathDuration = Random.Range(minDeathDuration, maxDeathDuration);
            isDead = true;
            isFullyGrown = false;

            while (time < deathDuration)
            {
                float t = time / deathDuration;
                float newGrowing = Mathf.Lerp(1, 0, t);
                material.SetFloat(kGrowingKey, newGrowing);
                time += Time.deltaTime;
                yield return null;
            }

            time = 0f;
            StartCoroutine(Reviving());

        }

        private IEnumerator Reviving()
        {
            currentState = State.Reviving;
            yield return StartCoroutine(GrowBuffer(true));
        }

        public void CutDown()
        {
            StopAllCoroutines();
            leafScaler.SetLeafScale(leafScaler.minGrowthScale);
            time = 0f;
            StartCoroutine(Dying());
        }

        public void Revive()
        {
            StartCoroutine(GrowBuffer(true));
        }
    }
}
