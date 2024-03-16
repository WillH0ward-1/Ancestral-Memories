using System.Collections;
using System.Collections.Generic;
using Deform;
using Pathfinding.RVO;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ProceduralModeling
{
    [RequireComponent(typeof(MeshRenderer))]
    public class PTGrowing : MonoBehaviour
    {
        private Material material;
        private ProceduralTree proceduralTree;
        private TreeData treeData;
        private LeafScaler leafScaler;
        private NavMeshCutting navMeshCutting;
        private TreeFruitManager treeFruitManager;

        private BoxCollider growthInhibitionZone;
        private BoxCollider interactionCollider;
        private Interactable interactable;

        [Header("=== External Components ===")]

        public MapObjGen mapObjGen;
        public SeasonManager seasonManager;
        public RainControl rainControl;
        public LerpTerrain lerpTerrain;

        [Header("=== Tree State ===")]
        public State currentState;
        public bool isGrowing = false;
        public bool isFullyGrown = false;
        public bool isDead = false;
        public float time = 0f;

        [Header("=== Tree Lifespan ===")]
        public float minLifeTimeSeconds = 10f;
        public float maxLifeTimeSeconds = 25f;
        public float lifeTimeSecs = 60f;
        [SerializeField] private float remainingLifeTime = 60f;

        private const string kGrowingKey = "_T";

        [Header("=== Tree Duration Parameters ===")]
        public float minGrowBuffer = 10f;
        public float maxGrowBuffer = 60f;
        public float minGrowDuration = 30f;
        public float maxGrowDuration = 45f;
        public float minDeathDuration = 2f;
        public float maxDeathDuration = 5f;
        public float minReviveBuffer = 20f;
        public float maxReviveBuffer = 30f;

        [Header("=== Calculated Durations ===")]
        [SerializeField] private float growBuffer;
        public float growDuration;
        public float deathDuration;
        [SerializeField] private float reviveBuffer;

        [Header("=== Navmesh Obstacle ===")]
        [SerializeField] private LayerMask entityLayers;
        [SerializeField] private float growthInhibitionScaleFactor = 1.0f;

        [Header("=== Tree Ground Decal ===")]
        public GameObject dirtDecal;

        [Header("=== Tree Leaf Control ===")]

        public List<GameObject> fallingLeaves;
        public Color[] leafColorsPerSeason;
        private string leafColourParam = "_LeafColour";
        private Material leafMaterial;

        public enum State
        {
            Buffering,
            Growing,
            Alive,
            Dying,
            Dead,
            Reviving
        }

        private TreeAudioManager treeAudioSFX;
        private PTGrowingManager pTGrowingManager;

        private DecalProjector dirtDecalProjector;
        private DecalProjectorManager decalProjectorManager;

        private Vector3 obstacleSize;

        private AudioTreeManager audioTreeGrow;
        private AudioTreeLeavesManager audioTreeLeaves;

        private void Awake()
        {
            proceduralTree = GetComponentInChildren<ProceduralTree>();
            navMeshCutting = GetComponentInChildren<NavMeshCutting>();
            material = GetComponentInChildren<Renderer>().material;
            treeData = proceduralTree.Data;
            material.SetFloat(kGrowingKey, 0);
            leafMaterial = proceduralTree.leafMat;

            leafScaler = gameObject.GetComponent<LeafScaler>();
            treeAudioSFX = GetComponent<TreeAudioManager>();
            treeFruitManager = GetComponent<TreeFruitManager>();
            interactable = GetComponent<Interactable>();

            audioTreeGrow = GetComponentInChildren<AudioTreeManager>();
            audioTreeGrow.pTGrowing = this;

            currentState = State.Buffering;
            time = 0f;

            pTGrowingManager = FindObjectOfType<PTGrowingManager>();

            if (pTGrowingManager != null)
            {
                pTGrowingManager.RegisterPTGrowing(this);
            }
        }

        private float minDecalSizeX = 0f;
        private float minDecalSizeY = 0f;

        private float maxDecalSizeX;
        private float maxDecalSizeY;
        private float projectionDepth;

        private Vector3 maxDecalSize;
        private Vector3 minDecalSize;

        private GameObject decalInstance;

        private void CreateDirtDecal()
        {
            decalInstance = Instantiate(dirtDecal, transform, false);
            decalInstance.transform.localPosition = Vector3.zero; // Set localPosition instead of position
            
            dirtDecalProjector = decalInstance.GetComponent<DecalProjector>();
            decalProjectorManager = decalInstance.GetComponent<DecalProjectorManager>();

            maxDecalSizeX = obstacleSize.x * 80f;
            maxDecalSizeY = obstacleSize.z * 80f;
            projectionDepth = 100f;

            maxDecalSize = new Vector3(maxDecalSizeX, maxDecalSizeY, projectionDepth);
            minDecalSize = new Vector3(minDecalSizeX, minDecalSizeY, projectionDepth);

            Vector3 decalSize = new Vector3(minDecalSizeX, minDecalSizeY, projectionDepth);
            decalProjectorManager.SetDecalSize(decalSize);
        }

        private Coroutine lerpSizeCoroutine;

        public void LerpDecalSize(Vector3 targetSize, float duration)
        {
            // If there is an ongoing coroutine, stop it
            if (lerpSizeCoroutine != null)
            {
                StopCoroutine(lerpSizeCoroutine);
            }

            // Start a new lerp coroutine
            lerpSizeCoroutine = StartCoroutine(LerpDecalScale(targetSize, duration));
        }

        private IEnumerator LerpDecalScale(Vector3 targetSize, float duration)
        {
            float timeElapsed = 0f;
            Vector3 initialSize = new Vector3(dirtDecalProjector.size.x, dirtDecalProjector.size.y, dirtDecalProjector.size.z);
            Vector3 newSize = initialSize;

            // Lerp the size over the given duration
            while (timeElapsed < duration)
            {
                newSize.x = Mathf.Lerp(initialSize.x, targetSize.x, timeElapsed / duration);
                newSize.y = Mathf.Lerp(initialSize.y, targetSize.y, timeElapsed / duration);
  
                decalProjectorManager.SetDecalSize(newSize);

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure the final size is set
            newSize.x = targetSize.x;
            newSize.y = targetSize.y;
            decalProjectorManager.SetDecalSize(newSize);

            lerpSizeCoroutine = null; // Coroutine finished
        }

        public void SetupBounds()
        {

            growthInhibitionZone = gameObject.AddComponent<BoxCollider>();
            growthInhibitionZone.isTrigger = true;
            growthInhibitionZone.size = new Vector3(navMeshCutting.obstacle.size.x * growthInhibitionScaleFactor, navMeshCutting.obstacle.height * growthInhibitionScaleFactor, navMeshCutting.obstacle.size.y * growthInhibitionScaleFactor);
            growthInhibitionZone.center = navMeshCutting.obstacle.center;
            obstacleSize = growthInhibitionZone.size;

            interactionCollider = gameObject.AddComponent<BoxCollider>();
            interactionCollider.isTrigger = false;
            interactionCollider.size = new Vector3(navMeshCutting.obstacle.size.x, 5f, navMeshCutting.obstacle.size.y);
            interactionCollider.center = new Vector3(0, 1.5f, 0);

            CreateDirtDecal();

            navMeshCutting.DisableNavMeshCut();
        }

        private DeformableManager deform;

        private void OnDestroy()
        {
            if (pTGrowingManager != null)
            {
                pTGrowingManager.UnregisterPTGrowing(this);
            }
        }


        public float overlapBoxScaleFactor = 1.2f;  // exposed factor for scaling overlap box size

        private bool IsAnyEntityInGrowthZone()
        {
            Vector3 overlapBoxSize = growthInhibitionZone.size;
            overlapBoxSize.Scale(transform.lossyScale);  // Adjust the size of the OverlapBox based on the GameObject's world scale

            Collider[] hitColliders = Physics.OverlapBox(transform.position + growthInhibitionZone.center, overlapBoxSize, Quaternion.identity, entityLayers);
            return hitColliders.Length > 0;
        }

        private void OnDrawGizmos()
        {
            /*
            Gizmos.color = Color.red;
            Vector3 center = transform.position + growthInhibitionZone.center;
            Vector3 halfSize = growthInhibitionZone.size / 2;
            Gizmos.DrawWireCube(center, halfSize);
            */
        }

        public void SetInteractable(bool isInteractable)
        {
            if (interactable.enabled != isInteractable)
            {
                interactable.enabled = isInteractable;
                interactionCollider.enabled = isInteractable;
            }
        }


        public void GrowTree()
        {
            SetInteractable(false);

            isFullyGrown = false;
            currentState = State.Buffering;
            time = 0f;
            treeData.Setup();
            leafScaler.SetLeafScale(leafScaler.minGrowthScale);
            StartCoroutine(GrowBuffer(false));
        }

        public IEnumerator GrowTreeInstant()
        {
            SetInteractable(false);

            isFullyGrown = false;
            currentState = State.Buffering;
            time = 0f;
            treeData.Setup();
            leafScaler.SetLeafScale(leafScaler.minGrowthScale);

            while (IsAnyEntityInGrowthZone())
            {
                yield return null;
            }

            isDead = false;
            StartCoroutine(Growing());
        }

        public bool ValidateTree()
        {
            if (isGrowing || isDead || !isFullyGrown)
            {
                return false;
            } else
            {
                return true;
            }
        }

        public bool ValidateIsFullyGrown()
        {
            if (isGrowing || !isFullyGrown)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private IEnumerator GrowBuffer(bool reseed)
        {
            SetInteractable(false);

            time = 0f;
            growBuffer = Random.Range(minGrowBuffer, maxGrowBuffer);

            LerpDecalSize(maxDecalSize, growBuffer);

            while (time < growBuffer)
            {
                time += Time.deltaTime;
                yield return null;
            }

            // Wait for entities to clear growth zone
            while (IsAnyEntityInGrowthZone())
            {
                yield return null;
            }

            time = 0f;

            isDead = false;
            StartCoroutine(Growing());
        }

        float growTime;
        // droughtGrowthBuffer: This factor determines how much slower the tree will grow during a drought.
        // For example, a value of 3 means the tree will grow three times slower during a drought.
        public float droughtGrowthBuffer = 3f;

        // lerpTimeAdjustment: This value determines the speed at which the growDuration adapts to changing drought conditions.
        // A smaller value makes the transition faster, and a larger value makes it slower.
        private float lerpTimeAdjustment = 5f; // Adjust this value as needed

        int maxGrow = 1;
        int minGrow = 0;
        float halfGrow = 0.5f;
        float growAudioTime = 0;

        private IEnumerator Growing()
        {
            if (!isDead)
            {
                SetInteractable(false);

                isFullyGrown = false;
                currentState = State.Growing;
                time = 0f;
                isDead = false;
                growDuration = Random.Range(minGrowDuration, maxGrowDuration);

                // Start the drought adjustment coroutine
                StartCoroutine(AdjustForDrought());

                
                //treeAudioSFX.StartTreeGrowthSFX(State.Growing);

                yield return null;
                navMeshCutting.EnableNavMeshCut();

                mapObjGen.treeGrowingList.Add(transform.gameObject);

                audioTreeGrow.SetPlayState(true);

                while (time < growDuration)
                {
                    if (!isDead)
                    {
                        isGrowing = true;
                        float t = time / growDuration;

                        UpdateGrowAudioTime(t);

                        growTime = Mathf.Lerp(minGrow, maxGrow, t);
                        audioTreeGrow.SetGrowthFX(growAudioTime);
                        material.SetFloat(kGrowingKey, growTime);
                        time += Time.deltaTime;
                    }

                    yield return null;
                }

                isGrowing = false;
                audioTreeGrow.SetPlayState(false);

                time = 0f;

                StartCoroutine(Lifetime());
            }
        }

        private void UpdateGrowAudioTime(float t)
        {
            if (t <= halfGrow)
            {
                float remappedT = t / halfGrow;
                growAudioTime = Mathf.Lerp(0, 1, remappedT);
            }
            else
            {
                float remappedT = (t - halfGrow) / halfGrow;
                growAudioTime = Mathf.Lerp(maxGrow, minGrow, remappedT);
            }
        }

        private IEnumerator AdjustForDrought()
        {
            float originalDuration = growDuration;
            while (currentState == State.Growing)
            {
                if (rainControl.drought)
                {
                    growDuration = Mathf.Lerp(growDuration, originalDuration * droughtGrowthBuffer, Time.deltaTime / lerpTimeAdjustment);
                }
                else
                {
                    growDuration = Mathf.Lerp(growDuration, originalDuration, Time.deltaTime / lerpTimeAdjustment);
                }
                yield return null;
            }
        }



        public void KillTree()
        {

            if (!isDead)
            {
                isDead = true;
            }

            if (isGrowing)
            {
                isGrowing = false;
            }

            if (currentState != State.Dead)
            {
                currentState = State.Dead;
            }

            SetInteractable(false);

            leafScaler.LerpScale(leafScaler.CurrentScale, leafScaler.minGrowthScale, leafScaler.lerpduration);
            treeFruitManager.ClearFruits(); // Clear fruits during CutDown
        }

        private IEnumerator Lifetime()
        {
            if (!isDead)
            {
                SetInteractable(true);

                currentState = State.Alive;
                isFullyGrown = true;
                isDead = false;

                lifeTimeSecs = Random.Range(minLifeTimeSeconds, maxLifeTimeSeconds);

                if (!rainControl.drought && seasonManager._currentSeason != SeasonManager.Season.Winter)
                {
                    leafScaler.LerpScale(leafScaler.CurrentScale, leafScaler.maxGrowthScale, leafScaler.lerpduration);
                    audioTreeLeaves.SetPlayState(true);
                    audioTreeLeaves.SetLeafGrowthFX(leafScaler.lerpduration);
                    
                    treeFruitManager.SpawnFruits(proceduralTree.FruitPoints);
                }

                StartCoroutine(RustleTreeLeaves());

                while (time < lifeTimeSecs)
                {
                    time += Time.deltaTime;
                    remainingLifeTime = GetRemainingLifetime();

                    yield return null;
                }

                time = 0f;
                StartCoroutine(Dying());
            }
        }

        public WeatherControl weatherControl;

        private IEnumerator RustleTreeLeaves()
        {
            while (!isDead)
            {
                float windStrength = weatherControl.averageWindStrength;
                audioTreeLeaves.SetLeafRustlingFX(windStrength);

                yield return null;
            }

            yield break;
        }

        public float GetRemainingLifetime()
        {
            // Ensure that we don't return negative values if the tree is already dead.
            return Mathf.Max(lifeTimeSecs - time, 0);
        }

        private IEnumerator Dying()
        {
            if (!isDead)
            {
                SetInteractable(false);

                float time = 0f;

                currentState = State.Dying;

                isFullyGrown = false;

                deathDuration = Random.Range(minDeathDuration, maxDeathDuration);
                isDead = true;

                mapObjGen.treeGrowingList.Remove(transform.gameObject);

                leafScaler.LerpScale(leafScaler.CurrentScale, leafScaler.minGrowthScale, leafScaler.lerpduration);
                audioTreeLeaves.SetLeafGrowthFX(leafScaler.lerpduration);
                audioTreeLeaves.SetLeafRustlingFX(leafScaler.lerpduration);

                KillAllFruits();

                audioTreeGrow.SetPlayState(true);
                //treeAudioSFX.StartTreeGrowthSFX(State.Dying);

                while (time < leafScaler.lerpduration)
                {
                    time += Time.deltaTime;
                    yield return null;
                }

                time = 0f;

                while (time < deathDuration)
                {
                    float t = time / deathDuration;
                    growTime = Mathf.Lerp(1, 0, t);

                    UpdateGrowAudioTime(t);
                    audioTreeGrow.SetGrowthFX(growAudioTime);

                    material.SetFloat(kGrowingKey, growTime);
                    time += Time.deltaTime;
                    yield return null;
                }

                isFullyGrown = false;

                audioTreeGrow.SetPlayState(false);

                navMeshCutting.DisableNavMeshCut();

                StartCoroutine(Revive());
            }
        }

        private IEnumerator Revive()
        {
            SetInteractable(false);

            isFullyGrown = false;

            time = 0f;
            reviveBuffer = Random.Range(minReviveBuffer, maxReviveBuffer);

            LerpDecalSize(minDecalSize, reviveBuffer);

            while (time < reviveBuffer)
            {
                time += Time.deltaTime;
                yield return null;
            }

            currentState = State.Reviving;
            yield return StartCoroutine(GrowBuffer(true));
        }

        public void CutDown()
        {
            interactable.enabled = false;
            leafScaler.LerpScale(leafScaler.CurrentScale, leafScaler.minGrowthScale, leafScaler.lerpduration);
            treeFruitManager.ClearFruits(); // Clear fruits during CutDown
            time = 0f;
            StartCoroutine(Dying());
        }

        private void DetachLeaves()
        {
            foreach (GameObject leaf in proceduralTree.leafList)
            {
                // Store original position and rotation
                Vector3 origPos = leaf.transform.position;
                Quaternion origRot = leaf.transform.rotation;

                // Create new leaf under container
                GameObject newLeaf = Instantiate(leaf, origPos, origRot);

                Rigidbody rb = newLeaf.AddComponent<Rigidbody>();

                // Random force with downward bias
                Vector3 force = new Vector3(Random.Range(-2, 2), Random.Range(-1, -3), Random.Range(-2, 2));

                rb.AddForce(force, ForceMode.Impulse);

                // Add to leaves list
                fallingLeaves.Add(newLeaf);
            }
        }

        public IEnumerator LerpLeafColour(Color targetColor)
        {
            float baseLerpDuration = 5.0f; // Base duration for the color transition
            float randomFactor = Random.Range(0.8f, 1.2f); // Random factor to vary the duration
            float lerpDuration = baseLerpDuration * randomFactor; // Adjusted duration with randomness

            Color currentColor = leafMaterial.GetColor(leafColourParam);
            float lerpTime = 0;

            while (lerpTime < lerpDuration)
            {
                float t = lerpTime / lerpDuration;
                leafMaterial.SetColor(leafColourParam, Color.Lerp(currentColor, targetColor, t));
                lerpTime += Time.deltaTime;
                yield return null;
            }
        }

        public void GrowLeaves()
        {
            if (isFullyGrown)
            {
                leafScaler.LerpScale(leafScaler.CurrentScale, leafScaler.maxGrowthScale, leafScaler.lerpduration);
            }
        }

        public void KillLeaves()
        {
            if (isFullyGrown)
            {
                leafScaler.LerpScale(leafScaler.CurrentScale, leafScaler.minGrowthScale, leafScaler.lerpduration);
                //DetachLeaves();
            }
        }

        public void KillAllFruits()
        {
            foreach (GameObject fruit in treeFruitManager.fruits)
            {
                if (fruit == null || !fruit.activeSelf)
                {
                    continue; // Skip null or inactive fruits
                }

                FoodAttributes foodAttributes = fruit.GetComponent<FoodAttributes>();
                if (foodAttributes == null)
                {
                    continue; // Skip if FoodAttributes is missing
                }

                if (treeFruitManager.growCoroutines.TryGetValue(fruit, out Coroutine growCoroutine))
                {
                    treeFruitManager.StopCoroutine(growCoroutine);
                    treeFruitManager.growCoroutines.Remove(fruit);
                }

                treeFruitManager.StartCoroutine(treeFruitManager.Fall(fruit, foodAttributes));
            }
        }


        public void EnableNavMeshCutInternal()
        {
            if (navMeshCutting.obstacle != null)
            {
                navMeshCutting.obstacle.enabled = true;
            }
        }

        public void DisableNavMeshCutInternal()
        {
            if (navMeshCutting.obstacle != null)
            {
                navMeshCutting.obstacle.enabled = false;
            }

            if (pTGrowingManager != null)
            {
                pTGrowingManager.CompleteTask(this);
            }
        }
    }
}
