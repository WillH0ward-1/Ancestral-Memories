﻿using System.Collections;
using System.Collections.Generic;
using Deform;
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
        public LerpTerrain lerpTerrain;

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
        private BoxCollider growthInhibitionZone;

        [SerializeField] private LayerMask entityLayers;

        private bool isNavMeshCutEnabled = false;
        private Coroutine navMeshCutCoroutine;

        public RainControl rainControl;

        public enum State
        {
            Buffering,
            Growing,
            Alive,
            Dying,
            Reviving
        }

        public MapObjGen mapObjGen;

        public State currentState;
        public float time = 0f;

        private TreeAudioManager treeAudioSFX;

        private PTGrowingManager pTGrowingManager;

        [SerializeField] private float growthInhibitionScaleFactor = 1.0f;

        public SeasonManager seasonManager;

        private void Awake()
        {
            proceduralTree = GetComponentInChildren<ProceduralTree>();
            material = GetComponentInChildren<Renderer>().material;
            treeData = proceduralTree.Data;
            material.SetFloat(kGrowingKey, 0);
            leafMaterial = proceduralTree.leafMat;

            leafScaler = gameObject.GetComponent<LeafScaler>();
            treeAudioSFX = GetComponent<TreeAudioManager>();
            treeFruitManager = GetComponent<TreeFruitManager>();
            obstacle = GetComponent<RVOSquareObstacle>();

            currentState = State.Buffering;
            time = 0f;

            pTGrowingManager = FindObjectOfType<PTGrowingManager>();

            if (pTGrowingManager != null)
            {
                pTGrowingManager.RegisterPTGrowing(this);
            }

            growthInhibitionZone = gameObject.AddComponent<BoxCollider>();
            growthInhibitionZone.isTrigger = true;
            growthInhibitionZone.size = new Vector3(obstacle.size.x * growthInhibitionScaleFactor, obstacle.height * growthInhibitionScaleFactor, obstacle.size.y * growthInhibitionScaleFactor);
            growthInhibitionZone.center = obstacle.center;

            DisableNavMeshCut();
        }

        private DeformableManager deform;

        private void OnDestroy()
        {
            pTGrowingManager?.UnregisterPTGrowing(this);
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
            Gizmos.color = Color.red;
            Vector3 center = transform.position + growthInhibitionZone.center;
            Vector3 halfSize = growthInhibitionZone.size / 2;
            Gizmos.DrawWireCube(center, halfSize);
        }

        public void GrowTree()
        {
            isFullyGrown = false;
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

            // Wait for entities to clear growth zone
            while (IsAnyEntityInGrowthZone())
            {
                yield return null;
            }

            time = 0f;
            StartCoroutine(Growing());
        }

        float growTime;

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

            mapObjGen.treeGrowingList.Add(transform.gameObject);

            while (time < growDuration)
            {
                if (!isDead)
                {
                    isGrowing = true;
                    float t = time / growDuration;
                    growTime = Mathf.Lerp(0, 1, t);
                    material.SetFloat(kGrowingKey, growTime);
                    time += Time.deltaTime;
                }

                yield return null;
            }

            isGrowing = false;
            time = 0f;

            StartCoroutine(Lifetime());
        }


        private IEnumerator Lifetime()
        {
            currentState = State.Alive;
            isFullyGrown = true;
            isDead = false;

            lifeTimeSecs = Random.Range(minLifeTimeSeconds, maxLifeTimeSeconds);

            if (!rainControl.drought && seasonManager._currentSeason != SeasonManager.Season.Winter)
            {
                leafScaler.LerpScale(leafScaler.CurrentScale, leafScaler.maxGrowthScale, leafScaler.lerpduration);
                treeFruitManager.SpawnFruits(proceduralTree.FruitPoints);
            }

            StartCoroutine(treeAudioSFX.LeafRustleSFX());

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
            float time = 0f;

            currentState = State.Dying;

            isFullyGrown = false;

            deathDuration = Random.Range(minDeathDuration, maxDeathDuration);
            isDead = true;

            mapObjGen.treeGrowingList.Remove(transform.gameObject);

            leafScaler.LerpScale(leafScaler.CurrentScale, leafScaler.minGrowthScale, leafScaler.lerpduration);

            foreach (GameObject fruit in treeFruitManager.fruits)
            {
                StartCoroutine(treeFruitManager.Fall(fruit, fruit.transform));
            }

            treeAudioSFX.StartTreeGrowthSFX(State.Dying);

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
                material.SetFloat(kGrowingKey, growTime);
                time += Time.deltaTime;
                yield return null;
            }

            isFullyGrown = false;

            yield return null;
            DisableNavMeshCut();

            StartCoroutine(Revive());
        }

        private IEnumerator Revive()
        {
            isFullyGrown = false;
            currentState = State.Reviving;
            yield return StartCoroutine(GrowBuffer(true));
        }

        public void CutDown()
        {
            leafScaler.LerpScale(leafScaler.CurrentScale, leafScaler.minGrowthScale, leafScaler.lerpduration);
            treeFruitManager.ClearFruits(); // Clear fruits during CutDown
            time = 0f;
            StartCoroutine(Dying());
        }

        public void GrowLeaves()
        {
            if (isFullyGrown)
            {
                leafScaler.LerpScale(leafScaler.CurrentScale, leafScaler.maxGrowthScale, leafScaler.lerpduration);
            }
        }

        public List<GameObject> fallingLeaves;

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

        public Color[] leafColorsPerSeason;
        private string leafColourParam = "_LeafColour";
        private Material leafMaterial;

        public IEnumerator LerpLeafColour(Color targetColor)
        {
            float lerpTime = 0;
            float lerpDuration = 5.0f; // adjust this to control the speed of the color transition
            Color currentColor = leafMaterial.GetColor(leafColourParam);

            while (lerpTime < lerpDuration)
            {
                float t = lerpTime / lerpDuration;
                leafMaterial.SetColor(leafColourParam, Color.Lerp(currentColor, targetColor, t));
                lerpTime += Time.deltaTime;
                yield return null;
            }
        }



        public void KillLeaves()
        {
            if (isFullyGrown)
            {
                leafScaler.LerpScale(leafScaler.CurrentScale, leafScaler.minGrowthScale, leafScaler.lerpduration);
                DetachLeaves();
            }
        }

        public void KillFruits()
        {
            foreach (GameObject fruit in treeFruitManager.fruits)
            {
                StartCoroutine(treeFruitManager.Fall(fruit, fruit.transform));
            }
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

/*
 * 
public static Mesh Build(ProceduralTree treeInstance, TreeData data, int generations, float length, float radius, float leafSize, Material leafMat)
{
    data.Setup();
    var root = new TreeBranch(
        generations,
        length,
        radius,
        data,
        leafMat
    );

    treeInstance.GenerateLeaves(root, leafMat);
    treeInstance.GenerateFruitPoints(root);
    treeInstance.GenerateMeshCollider();

    var vertices = new List<Vector3>();
    var normals = new List<Vector3>();
    var tangents = new List<Vector4>();
    var uvs = new List<Vector2>();
    var triangles = new List<int>();

    float maxLength = TraverseMaxLength(root);

    Traverse(root, (branch) =>
    {
        var offset = vertices.Count;
        var vOffset = branch.Offset / maxLength;
        var vLength = branch.Length / maxLength;
        for (int i = 0, n = branch.Segments.Count; i < n; i++)
        {
            var t = 1f * i / (n - 1);
            var v = vOffset + vLength * t;

            var segment = branch.Segments[i];
            var N = segment.Frame.Normal;
            var B = segment.Frame.Binormal;

            for (int j = 0; j <= data.radialSegments; j++)
            {
                // 0.0 ~ 2π
                var u = 1f * j / data.radialSegments;
                float rad = u * PI2;
                float cos = Mathf.Cos(rad), sin = Mathf.Sin(rad);
                var normal = (cos * N + sin * B).normalized;

                vertices.Add(segment.Position + segment.Radius * normal);

                normals.Add(normal);
                var tangent = segment.Frame.Tangent;
                tangents.Add(new Vector4(tangent.x, tangent.y, tangent.z, 0f));
                uvs.Add(new Vector2(u, v));
            }
        }

        for (int j = 1; j <= data.heightSegments; j++)
        {
            for (int i = 1; i <= data.radialSegments; i++)
            {
                int a = (data.radialSegments + 1) * (j - 1) + (i - 1);
                int b = (data.radialSegments + 1) * j + (i - 1);
                int c = (data.radialSegments + 1) * j + i;
                int d = (data.radialSegments + 1) * (j - 1) + i;

                a += offset;
                b += offset;
                c += offset;
                d += offset;

                triangles.Add(a); triangles.Add(d); triangles.Add(b);
                triangles.Add(b); triangles.Add(d); triangles.Add(c);
            }
        }
    });
    var mesh = new Mesh();
    mesh.vertices = vertices.ToArray();
    mesh.normals = normals.ToArray();
    mesh.tangents = tangents.ToArray();
    mesh.uv = uvs.ToArray();
    mesh.triangles = triangles.ToArray();
    return mesh;
}
*/