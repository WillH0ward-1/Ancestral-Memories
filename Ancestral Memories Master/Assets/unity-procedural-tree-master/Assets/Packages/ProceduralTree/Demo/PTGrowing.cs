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
        public float growDuration;
        public float deathDuration;

        public bool isDead = false;

        private TreeFruitManager treeFruitManager;

        public enum State
        {
            Buffering,
            Growing,
            Alive,
            Dying,
            Reviving
        }

        public State currentState;
        public float time = 0f;

        private TreeAudioManager treeAudioSFX;

        private void Awake()
        {
            proceduralTree = GetComponentInChildren<ProceduralTree>();
            material = GetComponentInChildren<Renderer>().material;
            treeData = proceduralTree.Data;
            material.SetFloat(kGrowingKey, 0);

            leafScaler = gameObject.GetComponent<LeafScaler>();
            treeAudioSFX = GetComponent<TreeAudioManager>();  // Fetch the TreeAudioSFX component
            treeFruitManager = GetComponent<TreeFruitManager>(); // Fetch the TreeFruitManager component

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
            //treeAudioSFX.PlayTreeSproutSFX();  // Play sprouting sound effect
            isFullyGrown = false;

            currentState = State.Growing;

            time = 0f;
            isDead = false;
            growDuration = Random.Range(minGrowDuration, maxGrowDuration);

            treeAudioSFX.StartTreeGrowthSFX(State.Growing);

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
            isDead = false;

            lifeTimeSecs = Random.Range(minLifeTimeSeconds, maxLifeTimeSeconds);

            treeFruitManager.SpawnFruits(proceduralTree.FruitPoints);

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

            deathDuration = Random.Range(minDeathDuration, maxDeathDuration);
            isDead = true;
            isFullyGrown = false;

            leafScaler.LerpScale(leafScaler.maxGrowthScale, leafScaler.minGrowthScale, leafScaler.lerpduration);

            foreach (GameObject fruit in treeFruitManager.fruits)
            {
                StartCoroutine(treeFruitManager.Fall(fruit, fruit.transform));
            }

            treeAudioSFX.StartTreeGrowthSFX(State.Dying);

            yield return new WaitForSeconds(leafScaler.lerpduration);

            while (time < deathDuration)
            {
                float t = time / deathDuration;
                float newGrowing = Mathf.Lerp(1, 0, t);
                material.SetFloat(kGrowingKey, newGrowing);
                time += Time.deltaTime;
                yield return null;
            }

            time = 0f;
            StartCoroutine(Revive());
        }

        private IEnumerator Revive()
        {
            currentState = State.Reviving;
            yield return StartCoroutine(GrowBuffer(true));
        }

        public void CutDown()
        {
            StopAllCoroutines();
            leafScaler.SetLeafScale(leafScaler.minGrowthScale);
            treeFruitManager.ClearFruits(); // Clear fruits during CutDown
            time = 0f;
            StartCoroutine(Dying());
        }
    }
}
