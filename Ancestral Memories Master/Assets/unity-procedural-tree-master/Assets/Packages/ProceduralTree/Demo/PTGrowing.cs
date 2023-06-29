using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.RVO;
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

        private RVOSquareObstacle obstacle;

        private bool isNavMeshCutEnabled = false;
        private Coroutine navMeshCutCoroutine;

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

        private PTGrowingManager pTGrowingManager;

        private void Awake()
        {
            proceduralTree = GetComponentInChildren<ProceduralTree>();
            material = GetComponentInChildren<Renderer>().material;
            treeData = proceduralTree.Data;
            material.SetFloat(kGrowingKey, 0);

            leafScaler = gameObject.GetComponent<LeafScaler>();
            treeAudioSFX = GetComponent<TreeAudioManager>();  // Fetch the TreeAudioSFX component
            treeFruitManager = GetComponent<TreeFruitManager>(); // Fetch the TreeFruitManager component
            obstacle = GetComponent<RVOSquareObstacle>();

            currentState = State.Buffering;
            time = 0f;

            // Get a reference to the PTGrowingManager in the scene
            pTGrowingManager = FindObjectOfType<PTGrowingManager>();

            if (pTGrowingManager != null)
            {
                pTGrowingManager.RegisterPTGrowing(this);
            }

            DisableNavMeshCut();
        }

        private void OnDestroy()
        {
            // Unregister this PTGrowing instance from the manager
            pTGrowingManager?.UnregisterPTGrowing(this);

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
            isFullyGrown = false;
            currentState = State.Growing;
            time = 0f;
            isDead = false;
            growDuration = Random.Range(minGrowDuration, maxGrowDuration);

            treeAudioSFX.StartTreeGrowthSFX(State.Growing);

            yield return null;
            EnableNavMeshCut();

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

            yield return null;
            DisableNavMeshCut();

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

        public void EnableNavMeshCut()
        {
            if (obstacle != null)
            {
                obstacle.enabled = true;
            }
        }

        public void DisableNavMeshCut()
        {
            if (obstacle != null)
            {
                obstacle.enabled = false;
            }
        }

        // Internal method to enable NavmeshCut
        public void EnableNavMeshCutInternal()
        {
            if (obstacle != null)
            {
                obstacle.enabled = true;
            }

            /*
            if (pTGrowingManager != null)
            {
                pTGrowingManager.CompleteTask(this);
            }
            */
        }

        // Internal method to disable NavmeshCut
        public void DisableNavMeshCutInternal()
        {
            if (obstacle != null)
            {
                obstacle.enabled = false;
            }

            if (pTGrowingManager != null)
            {
                pTGrowingManager.CompleteTask(this);
            }
        }
    }
}
