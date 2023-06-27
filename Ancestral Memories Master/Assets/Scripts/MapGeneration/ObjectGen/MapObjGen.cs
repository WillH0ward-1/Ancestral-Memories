using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Deform;
using FIMSpace.FLook;
using FMODUnity;
using Pathfinding;
using ProceduralModeling;

public class MapObjGen : MonoBehaviour
{
    //public int density;

    [Header("Map Object Generator")]
    [Header("========================================================================================================================")]

    public GameObject terrain;

    [Header("Terrain Mesh Data")]
    [Space(10)]

    [SerializeField] private MeshData meshData;
    [SerializeField] private MeshSettings meshSettings;
    [SerializeField] private GameObject mapObject;
    [SerializeField] private GameObject hierarchyRoot;

    [SerializeField] private float sampleWidth = 0;
    [SerializeField] private float sampleHeight = 0;

    //private MeshFilter meshFilter;

    [Header("========================================================================================================================")]
    [Header("Spawnable Objects")]
    [Space(10)]

    [SerializeField] private GameObject[] trees;
    [SerializeField] private GameObject[] appleTrees;
    [SerializeField] private GameObject[] grass;
    [SerializeField] private GameObject[] foliage;
    [SerializeField] private GameObject[] rocks;
    [SerializeField] private GameObject[] mushrooms;
    [SerializeField] private GameObject[] flies;
    [SerializeField] private GameObject[] fish;
    [SerializeField] private GameObject[] animals;
    [SerializeField] private GameObject[] fireWood;
    [SerializeField] private GameObject[] seaShells;
    [SerializeField] private GameObject[] pedestals;
    [SerializeField] private GameObject[] cave;
    [SerializeField] private GameObject[] humans;
    [SerializeField] private GameObject[] proceduralTrees;

    [SerializeField] private GameObject spawnPointPrefab;

    [Header("========================================================================================================================")]
    [Header("Layer Masks")]
    [Space(10)]

    [SerializeField] private LayerMask caveLayerMask;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask deadZoneLayerMask;

    [Header("========================================================================================================================")]
    [Header("Object Scaling")]
    [Space(10)]

    [SerializeField] Vector3 minTreeScale;
    [SerializeField] Vector3 maxTreeScale;

    [SerializeField] Vector3 minAppleScale;
    [SerializeField] Vector3 maxAppleScale;

    [SerializeField] Vector3 minGrassScale;
    [SerializeField] Vector3 maxGrassScale;

    [SerializeField] Vector3 minFoliageScale;
    [SerializeField] Vector3 maxFoliageScale;

    [SerializeField] Vector3 minRockScale;
    [SerializeField] Vector3 maxRockScale;

    [SerializeField] Vector3 minMushroomScale;
    [SerializeField] Vector3 maxMushroomScale;

    [SerializeField] Vector3 minFliesScale;
    [SerializeField] Vector3 maxFliesScale;

    [SerializeField] Vector3 minFishScale;
    [SerializeField] Vector3 maxFishScale;

    [SerializeField] Vector3 minFireWoodScale;
    [SerializeField] Vector3 maxFireWoodScale;

    [SerializeField] Vector3 minSeaShellScale;
    [SerializeField] Vector3 maxSeaShellScale;

    [SerializeField] Vector3 minPedestalScale;
    [SerializeField] Vector3 maxPedestalScale;

    [SerializeField] Vector3 minCaveScale;
    [SerializeField] Vector3 maxCaveScale;

    [SerializeField] private Vector3 humanAverageScale = new Vector3(1.03f, 0.59f, 0.83f);

    [Header("========================================================================================================================")]

    [Header("Object Rotation")]
    [Space(10)]

    [SerializeField, Range(0, 1)] float rotateTowardsNormal;
    [SerializeField] Vector2 rotationRange;

    [Header("========================================================================================================================")]

    [Header("Density")]
    [Space(10)]

    [SerializeField] float minimumHumanRadiusminimumSpawnPointRadius = 70;
    [SerializeField] float minimumTreeRadius = 70;
    [SerializeField] float minimumAppleTreeRadius = 70;
    [SerializeField] float minimumGrassRadius = 70;
    [SerializeField] float minimumFoliageRadius = 70;
    [SerializeField] float minimumRockRadius = 70;
    [SerializeField] float minimumMushroomRadius = 70;
    [SerializeField] float minimumFliesRadius = 70;
    [SerializeField] float minimumFishRadius = 70;
    [SerializeField] float minimumAnimalRadius = 70;
    [SerializeField] float minimumFireWoodRadius = 70;
    [SerializeField] float minimumSeaShellRadius = 70;
    [SerializeField] float minimumPedestalRadius = 70;
    [SerializeField] float minimumCaveRadius = 70;
    [SerializeField] float minimumHumanRadius = 70;
    [SerializeField] float minimumSpawnPointRadius = 70;
    [Header("========================================================================================================================")]

    [Header("Navmesh")]
    [Space(10)]

    [SerializeField] private NavMeshObstacle navMeshObstacle;
    [SerializeField] private NavMeshModifier navModifier;

    [SerializeField] float obstacleSizeX = 1;
    [SerializeField] float obstacleSizeY = 14;
    [SerializeField] float obstacleSizeZ = 1;

    [Header("========================================================================================================================")]

    private readonly string treeTag = "Trees";
    private readonly string appleTreeTag = "AppleTree";
    private readonly string waterTag = "Water";
    private readonly string groundTag = "Ground";
    private readonly string grassTag = "Grass";
    private readonly string foliageTag = "Foliage";
    private readonly string rockTag = "Rocks";
    private readonly string fliesTag = "Flies";
    private readonly string fishTag = "Fish";
    private readonly string animalTag = "Animal";
    private readonly string mushroomTag = "Mushrooms";
    private readonly string fireWoodTag = "FireWood";
    private readonly string seaShellTag = "SeaShell";
    private readonly string pedestalTag = "Pedestal";
    private readonly string humanTag = "Human";

    [Header("========================================================================================================================")]

    [Header("========================================================================================================================")]

    [Header("Positioning")]
    [Space(10)]

    [SerializeField] float yOffset;
    [SerializeField] private float mapSizeX = 0;
    [SerializeField] private float mapSizeY = 0;
    [SerializeField] private float mapSizeZ = 0;

    [SerializeField] private float xOffset = 0;
    [SerializeField] private float zOffset = 0;
    [SerializeField] private float initY = 0;

    [SerializeField] private Player player;
    public RadialMenu radialMenu;
    public Camera cam;

    [SerializeField] int vertSampleFactor;
    [SerializeField] private bool invertSpreadOrigin = false;
    private Vector3 mapCenter;

    [Header("========================================================================================================================")]
    [Header("Sound Emitters")]
    [Space(10)]

    [SerializeField] private Transform EmittersContainer;
    [SerializeField] private GameObject waterSoundEmitter;
    [SerializeField] private Transform waterEmitterTransform;
    [SerializeField] private Transform waterEmitterRoot;
    [SerializeField] private List<GameObject> waterEmitters;

    [Header("========================================================================================================================")]
    [Header("Behaviours Reference")]
    [Space(10)]

    [SerializeField] private CharacterBehaviours behaviours;

    [Header("========================================================================================================================")]
    [Header("Weather")]
    [Space(10)]

    [SerializeField] private RainControl rainControl;
    [SerializeField] private TreeDeathManager treeFallManager;

    public TimeCycleManager timeCycleManager;

    private GameObject emitterInstance;

    public IEnumerator WaterSFXEmitterGen()
    {
        Vector3[] verts = waterEmitterTransform.GetComponent<MeshFilter>().mesh.vertices;
        int sampleDensity = verts.Length / vertSampleFactor;

        int i = 0;

        for (i = 0; i <= verts.Length; i++)
        {
            emitterInstance = Instantiate(waterSoundEmitter, waterEmitterTransform.localToWorldMatrix.MultiplyPoint3x4(verts[i]), Quaternion.identity, waterEmitterRoot);

            waterEmitters.Add(emitterInstance);

            i += sampleDensity;
        }

        if (i >= verts.Length)
        {
            StartCoroutine(GenerateEmitterCheckers());
        }

        yield break;
    }

    private float emitterY;
    [SerializeField] private float emitterYOffset = 10f;
    [SerializeField] private LayerMask emitterLayerMask;

    public IEnumerator GenerateEmitterCheckers()
    {
        foreach (GameObject emitter in waterEmitters)
        {
            emitterY = emitter.transform.position.y;

            emitter.transform.position = new Vector3(emitter.transform.position.x, emitterY += emitterYOffset, emitter.transform.position.z);

            if (Physics.Raycast(emitter.transform.position, Vector3.down, out RaycastHit downHit, Mathf.Infinity, emitterLayerMask))
            {
                if (downHit.collider.CompareTag("Water"))
                {
                    float newY = emitterY -= emitterYOffset;

                    emitter.transform.position = new Vector3(emitter.transform.position.x, newY, emitter.transform.position.z);
                    continue;
                }
                else
                {
                    DestroyObject(emitter);
                }
            }

            static void DestroyObject(GameObject emitter)
            {
                Debug.Log("Object destroyed in game.");
                Destroy(emitter);

            }
        }

        ListCleanup(waterEmitters);
        EnableStudioEmitters(waterEmitters);

        yield break;

    }

    private void Awake()
    {
        mapCenter = Vector3.zero;
    }

    private Vector3[] waterEmitterVerts;

    public bool mapObjectsGenerated = false;

    public void GenerateMapObjects()
    {
        mapObjectsGenerated = false;

       // ResetPosOffset(mapObject.transform);
        ResetPosOffset();

        sampleWidth = meshSettings.meshWorldSize; 
        sampleHeight = meshSettings.meshWorldSize; 

        xOffset = -sampleWidth / 2;
        zOffset = -sampleHeight / 2;

        //mapObjectList.Clear();

        //GenerateOceanEmitters(sampleWidth, sampleHeight);

        PoissonDiscSampler spawnPointsSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumSpawnPointRadius);
        PoissonDiscSampler treeSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumTreeRadius);
        PoissonDiscSampler appleTreeSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumAppleTreeRadius);
        PoissonDiscSampler grassSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumGrassRadius);
        PoissonDiscSampler foliageSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumFoliageRadius);
        PoissonDiscSampler rockSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumRockRadius);
        PoissonDiscSampler fliesSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumFliesRadius);
        PoissonDiscSampler animalSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumAnimalRadius);
        PoissonDiscSampler mushroomSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumMushroomRadius);
        PoissonDiscSampler fireWoodSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumFireWoodRadius);
        PoissonDiscSampler seaShellSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumSeaShellRadius);
        PoissonDiscSampler pedestalSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumPedestalRadius);
        PoissonDiscSampler caveSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumCaveRadius);
        PoissonDiscSampler humanSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumHumanRadius);

        //TreePoissonDisc(treeSampler);
        //AppleTreePoissonDisc(appleTreeSampler);
        GrassPoissonDisc(grassSampler);
        //FoliagePoissonDisc(foliageSampler);
        //RocksPoissonDisc(rockSampler);
        //FliesPoissonDisc(fliesSampler);
        AnimalPoissonDisc(animalSampler);
        MushroomPoissonDisc(mushroomSampler);
        //FireWoodPoissonDisc(fireWoodSampler);
        //SeaShellPoissonDisc(seaShellSampler);
        //PedestalPoissonDisc(pedestalSampler);
        //CavePoissonDisc(caveSampler);
        HumanPoissonDisc(humanSampler);
        SpawnPointsPoissonDisc(spawnPointsSampler);

        ProceduralTreePoissonDisc(treeSampler);


        SetOffset();

        foreach (GameObject obj in mapObjectList)
        {
            GroundCheck(obj);
        }

        foreach (GameObject obj in spawnPointsList)
        {
            GroundCheck(obj);
        }

        DestroyDeadZones();

        StartCoroutine(WaterSFXEmitterGen());

        ListCleanup(spawnPointsList);
        ListCleanup(mapObjectList);
        ListCleanup(treeList);
        ListCleanup(npcList);
        ListCleanup(grassList);
        ListCleanup(humanPopulationList);

        mapObjectsGenerated = true;

        SetupCorruptionControl(mapObjectList);

        RandomizeHumanRace();

        EnableStudioEmitters(grassList);

        StartCoroutine(StartProceduralTreeGrowth(treeList));
        //StartCoroutine(StartTreeGrowth(treeList));

        //EnableNavMeshAgents(npcList);

        player.transform.GetComponentInChildren<SpawnPoints>().SetSpawnPosition(spawnPointsList);
        //RandomizeTreecolours();

    }

    public IEnumerator StartProceduralTreeGrowth(List<GameObject> treeList)
    {
        foreach (GameObject tree in treeList)
        {
            //ProceduralModelingBase ptBase = tree.GetComponentInChildren<ProceduralModelingBase>();
            //ptBase.player = player;
            PTGrowing ptGrowing = tree.GetComponentInChildren<PTGrowing>();
            ptGrowing.GrowTree();
        }

        yield break;
    }

    private void RandomizeHumanRace()
    {
        int populationSize = humanPopulationList.Count;
        float stepSize = 1f / populationSize;

        for (int i = 0; i < populationSize; i++)
        {
            GameObject human = humanPopulationList[i];
            float skinToneValue = i * stepSize;
            Renderer[] renderers = human.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.materials;
                foreach (Material material in materials)
                {
                    material.SetFloat("_SkinTone", skinToneValue);
                }
            }
        }
    }
        void ListCleanup(List<GameObject> list)
    {
        for (var i = list.Count - 1; i > -1; i--)
        {
            if (list[i] == null)
                list.RemoveAt(i);
        }
    }

    private void EnableStudioEmitters(List<GameObject> list)
    {
        foreach (GameObject emitter in list)
        {
            if (emitter != null)
            {
                StudioEventEmitter eventEmitter = emitter.transform.GetComponent<StudioEventEmitter>();
                // eventEmitter.enabled = true;

                eventEmitter.enabled = true;

                eventEmitter.Play();
            }
        }
    }


    private List<Vector3> XspawnPositions;
    private List<Vector3> ZspawnPositions;

    public GameObject GetRandomMapObject(GameObject[] mapElements)
    {
        return mapElements[Random.Range(0, mapElements.Length)];
    }

    public List<GameObject> spawnPointsList;

    void SpawnPointsPoissonDisc(PoissonDiscSampler spawnPointSampler)
    {
        foreach (Vector2 sample in spawnPointSampler.Samples())
        {
            GameObject spawnPoint = spawnPointPrefab;
            GameObject spawnPointInstance = Instantiate(spawnPoint, new Vector3(sample.x, initY, sample.y), Quaternion.identity);
            spawnPointInstance.transform.SetParent(hierarchyRoot.transform);

            spawnPointsList.Add(spawnPointInstance);
        }
    }


    void AnimalPoissonDisc(PoissonDiscSampler animalSampler)
    {
        foreach (Vector2 sample in animalSampler.Samples())
        {
            GameObject randomAnimal = GetRandomMapObject(animals);

            GameObject animalInstance = Instantiate(randomAnimal, new Vector3(sample.x, initY, sample.y), Quaternion.identity);
        
            animalInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            animalInstance.tag = animalTag;

            int animalsLayer = LayerMask.NameToLayer("Animals");
            animalInstance.layer = animalsLayer;

            animalInstance.transform.SetParent(hierarchyRoot.transform);

            LerpDeformation deform = animalInstance.transform.GetComponentInChildren<LerpDeformation>();

            deform.player = player;

            deform.enabled = false;

            //NavMeshAgent agent = animalInstance.GetComponentInChildren<NavMeshAgent>();
            AnimalAI animalAI = animalInstance.GetComponentInChildren<AnimalAI>();
            animalAI.player = player;
            animalAI.playerBehaviours = behaviours;

            FLookAnimator lookAnimator = animalInstance.GetComponentInChildren<FLookAnimator>();
            lookAnimator.enabled = true;
            lookAnimator.ObjectToFollow = player.transform;

            animalAI.lookAnimator = lookAnimator;

            deform.enabled = true;

            mapObjectList.Add(animalInstance);
            npcList.Add(animalInstance);

            //GroundCheck(instantiatedPrefab);
            //WaterCheck();
        }
    }

    public List<GameObject> humanPopulationList;

    void HumanPoissonDisc(PoissonDiscSampler humanSampler)
    {
        foreach (Vector2 sample in humanSampler.Samples())
        {
            GameObject randomHuman = GetRandomMapObject(humans);

            GameObject humanInstance = Instantiate(randomHuman, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            humanInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            humanInstance.tag = humanTag;

            int humanLayer = LayerMask.NameToLayer("Human");
            humanInstance.layer = humanLayer;

            float randomXScale = humanAverageScale.x;
            float randomYScale = humanAverageScale.y;
            float randomZScale = humanAverageScale.z;

            humanInstance.transform.localScale = new Vector3(randomXScale, randomYScale, randomZScale);


            humanInstance.transform.SetParent(hierarchyRoot.transform);

            LerpDeformation deform = humanInstance.transform.GetComponentInChildren<LerpDeformation>();
            deform.player = player;
            deform.enabled = false;

            //NavMeshAgent agent = animalInstance.GetComponentInChildren<NavMeshAgent>();
            HumanAI humanAI = humanInstance.GetComponentInChildren<HumanAI>();
            humanAI.player = player;
            humanAI.playerBehaviours = behaviours;

            FLookAnimator lookAnimator = humanInstance.GetComponentInChildren<FLookAnimator>();
            lookAnimator.enabled = true;
            lookAnimator.ObjectToFollow = player.transform;

            humanAI.lookAnimator = lookAnimator;

            deform.enabled = true;

            mapObjectList.Add(humanInstance);
            npcList.Add(humanInstance);
            humanPopulationList.Add(humanInstance);

            //GroundCheck(instantiatedPrefab);
            //WaterCheck();
        }
    }

    void EnableNavMeshAgents(List<GameObject> list)
    {
        foreach (GameObject instance in list)
        {
            RichAI aiPath = GetComponentInChildren<RichAI>();
            NavMeshAgent navMeshAgent = instance.GetComponentInChildren<NavMeshAgent>();
            navMeshAgent.enabled = true;
        }
        
    }

    void PedestalPoissonDisc(PoissonDiscSampler pedestalSampler)
    {
        foreach (Vector2 sample in pedestalSampler.Samples())
        {
            GameObject randomPedestal = GetRandomMapObject(pedestals);

            GameObject pedestalInstance = Instantiate(randomPedestal, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            pedestalInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            /*
            new Vector3(
            Random.Range(minTreeScale.x, maxTreeScale.x),
            Random.Range(minTreeScale.y, maxTreeScale.y),
            Random.Range(minTreeScale.z, maxTreeScale.z));
            */

            pedestalInstance.tag = pedestalTag;

            //int pedestalLayer = LayerMask.NameToLayer("Pedestal");
            //pedestalInstance.layer = pedestalLayer;

            mapObjectList.Add(pedestalInstance);

            pedestalInstance.transform.SetParent(hierarchyRoot.transform);


            //GroundCheck(instantiatedPrefab);
            //WaterCheck();

           // GrowTrees(treeInstance);

        }
    }


    private int distanceFromCenter;

    private float GetDistanceFromCenter(GameObject mapObject)
    {
        float distanceFromCenter = Vector3.Distance(mapObject.transform.position, mapCenter);

        return distanceFromCenter;
    }

    private float minDistanceFromCenter;

    void SeaShellPoissonDisc(PoissonDiscSampler seaShellSampler)
    {
        minDistanceFromCenter = sampleHeight / 2;

        foreach (Vector2 sample in seaShellSampler.Samples())
        {
            GameObject randomSeaShell = GetRandomMapObject(seaShells);

            GameObject seaShellInstance = Instantiate(randomSeaShell, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            seaShellInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            float distanceFromCenter = GetDistanceFromCenter(seaShellInstance);

            if (distanceFromCenter <= minDistanceFromCenter)
            {
                if (Application.isEditor)
                {
                    Debug.Log("Object destroyed in Editor.");
                    DestroyImmediate(seaShellInstance);
                }
                else if (!Application.isEditor)
                {
                    Debug.Log("Object destroyed in game.");
                    Destroy(seaShellInstance);
                }

                continue;
            }

            if (distanceFromCenter >= minDistanceFromCenter)
            {

                seaShellInstance.transform.SetParent(hierarchyRoot.transform);

                mapObjectList.Add(seaShellInstance);
            }
            //GroundCheck(instantiatedPrefab);
            //WaterCheck();
        }
    }

    [Header("Generated Objects")]
    [Space(10)]

    public List<GameObject> mapObjectList; 
    public List<GameObject> treeList;
    public List<GameObject> npcList;
    public List<GameObject> grassList;

    private Vector3 zeroScale = new Vector3(0.0000001f, 0.0000001f, 0.0000001f);
    //private CorruptionControl corruptionControl;

    public WeatherControl weather;

    void SetupCorruptionControl(List<GameObject> list)
    {
        foreach (GameObject mapObj in list)
        {
            CorruptionControl corruptionControl = mapObj.transform.gameObject.GetComponent<CorruptionControl>();
            if (corruptionControl == null)
            {
                corruptionControl = mapObj.transform.gameObject.AddComponent<CorruptionControl>();
            }

            corruptionControl.rain = rainControl;
            corruptionControl.player = player;
            corruptionControl.behaviours = behaviours;

            if (mapObj.transform.CompareTag("Animal"))
            {
                corruptionControl.newMin = 1;
                corruptionControl.newMax = 0;
            }

            corruptionControl.CorruptionModifierActive = true;
        }
    }

    void ProceduralTreePoissonDisc(PoissonDiscSampler treeSampler)
    {

        foreach (Vector2 sample in treeSampler.Samples())
        {
            GameObject randomTree = GetRandomMapObject(proceduralTrees);

            GameObject treeInstance = Instantiate(randomTree, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            Vector3 treeScale = new Vector3(maxTreeScale.x, maxTreeScale.y, maxTreeScale.z);

            treeInstance.transform.localScale = treeScale;

            treeInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            //TreeAudioSFX treeAudio = treeInstance.transform.GetComponent<TreeAudioSFX>();

            //treeAudio.timeManager = timeCycleManager;
            //treeAudio.weatherManager = weather;

            PTGrowing ptGrow = treeInstance.GetComponentInChildren<PTGrowing>();

            TreeAudioManager treeAudioManager = treeInstance.transform.GetComponentInChildren<TreeAudioManager>();
            treeAudioManager.timeManager = timeCycleManager;
            treeAudioManager.weatherManager = weather;
            //ptGrow.GrowTree();

            int treeLayer = LayerMask.NameToLayer("Trees");
            treeInstance.layer = treeLayer;

            mapObjectList.Add(treeInstance);

            treeInstance.transform.SetParent(hierarchyRoot.transform);

            //TreeDeathManager treeDeathManager = treeInstance.GetComponent<TreeDeathManager>();
            //treeDeathManager.mapObjGen = this;

            weather.windAffectedRendererList.Add(treeInstance.transform);

            treeList.Add(treeInstance);

            RandomiseTreeTextures();

        }
    }

    private void RandomiseTreeTextures()
    {
        foreach (var tree in treeList)
        {
            tree.GetComponent<RandomizeTreeTexture>().ApplyRandomTexture();
        }
    }

    void TreePoissonDisc(PoissonDiscSampler treeSampler)
    {
        foreach (Vector2 sample in treeSampler.Samples())
        {
            GameObject randomTree = GetRandomMapObject(trees);

            GameObject treeInstance = Instantiate(randomTree, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            treeInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            TreeAudioSFX treeAudio = treeInstance.transform.GetComponent<TreeAudioSFX>();

            treeAudio.timeManager = timeCycleManager;
            treeAudio.weatherManager = weather;

            int treeLayer = LayerMask.NameToLayer("Trees");
            treeInstance.layer = treeLayer;

            mapObjectList.Add(treeInstance);

            treeInstance.transform.SetParent(hierarchyRoot.transform);

            TreeDeathManager treeDeathManager = treeInstance.GetComponent<TreeDeathManager>();
            treeDeathManager.mapObjGen = this;

            weather.windAffectedRendererList.Add(treeInstance.transform);

            treeList.Add(treeInstance);
        }
    }

    private float randomTreeColourSeed = 0;

    void RandomizeTreecolours() {

        foreach (GameObject tree in treeList)
        {
            randomTreeColourSeed = Random.Range(0f, 1f);

            foreach (Material m in tree.transform.GetComponentInChildren<Renderer>().materials)
            {
                m.SetFloat("_RandomColourSeed", randomTreeColourSeed);
            }
        }

    }

    public bool readyToGrow = false;
    
    private IEnumerator StartTreeGrowth(List<GameObject> objectList)
    {
        ListCleanup(objectList);

        foreach (GameObject growObject in objectList)
        {
            GrowTrees(growObject);
        }

        yield return null;
    }


    void AppleTreePoissonDisc(PoissonDiscSampler appleTreeSampler) {

        foreach (Vector2 sample in appleTreeSampler.Samples())
        {

            GameObject randomAppleTree = GetRandomMapObject(appleTrees);

            GameObject appleTreeInstance = Instantiate(randomAppleTree, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            appleTreeInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            /*
            new Vector3(
            Random.Range(minTreeScale.x, maxTreeScale.x),
            Random.Range(minTreeScale.y, maxTreeScale.y),
            Random.Range(minTreeScale.z, maxTreeScale.z));
            */

            //treeInstance.tag = treeTag;

            int appleTreeLayer = LayerMask.NameToLayer("AppleTree");
//            appleTreeInstance.layer = appleTreeLayer;

            mapObjectList.Add(appleTreeInstance);

            appleTreeInstance.transform.SetParent(hierarchyRoot.transform);


            //GroundCheck(instantiatedPrefab);
            //WaterCheck();

            GrowTrees(appleTreeInstance);
        }

    }


    [Header("Tree Growth + Fruit Growth")]
    [Space(10)]

    [SerializeField] private int minTreeGrowDuration = 15;
    [SerializeField] private int maxTreeGrowDuration = 30;
    [SerializeField] private int minTreeGrowDelay = 15;
    [SerializeField] private int maxTreeGrowDelay = 30;

    [SerializeField] private int minAppleGrowDuration = 15;
    [SerializeField] private int maxAppleGrowDuration = 30;
    [SerializeField] private int minAppleGrowDelay = 15;
    [SerializeField] private int maxAppleGrowDelay = 30;

    private int treeGrowDuration;
    private int appleGrowDuration;

    float appleGrowthDelay;
    float treeGrowthDelay;

    private LeafControl leafControl;

    //private TreeAudioSFX treeAudio;

    public void GrowTrees(GameObject tree)
    {
        Vector3 treeScaleDestination = new Vector3(maxTreeScale.x, maxTreeScale.y, maxTreeScale.z);

        Transform treeTransform = tree.transform;
        ScaleControl treeGrowControl = treeTransform.GetComponent<ScaleControl>();
        TreeDeathManager treeDeathManager = treeTransform.GetComponent<TreeDeathManager>();
       
        DirtExplode dirt = treeTransform.GetComponentInChildren<DirtExplode>();
        GameObject dirtExplodeObj = dirt.transform.gameObject;

        CorruptionControl dirtCorruption = dirtExplodeObj.transform.GetComponent<CorruptionControl>();
        dirtCorruption.CorruptionModifierActive = true;

        var interactable = tree.GetComponent<Interactable>();
        interactable.enabled = false;

        var dirtExplodeParticles = dirt.transform.GetComponent<ParticleSystem>();
        dirtExplodeParticles.transform.localScale = Vector3.one;
        dirtExplodeParticles.transform.localPosition = Vector3.zero;
        var emission = dirtExplodeParticles.emission;
        emission.enabled = false;

        treeDeathManager.scaleControl = treeGrowControl;
        treeGrowControl.rainControl = rainControl;

        treeGrowDuration = Random.Range(minTreeGrowDuration, maxTreeGrowDuration);
        appleGrowDuration = Random.Range(minAppleGrowDuration, maxAppleGrowDuration);

        appleGrowthDelay = Random.Range(minAppleGrowDelay, maxAppleGrowDelay);
        treeGrowthDelay = Random.Range(minTreeGrowDelay, maxTreeGrowDelay);

        StartCoroutine(WaitForGrowDelay(tree, dirtExplodeParticles, emission, treeGrowthDelay));

        StartCoroutine(treeGrowControl.LerpScale(tree, zeroScale, treeScaleDestination, treeGrowDuration, treeGrowthDelay));
        
    }

    private IEnumerator WaitForGrowDelay(GameObject tree, ParticleSystem dirtExplodeParticles, ParticleSystem.EmissionModule emission, float treeGrowthDelay)
    {
        yield return new WaitForSeconds(treeGrowthDelay);

        emission.enabled = true;
        dirtExplodeParticles.Play();

        tree.GetComponentInChildren<CorruptionControl>().CorruptionModifierActive = true;
        TreeAudioSFX treeAudio = tree.GetComponent<TreeAudioSFX>();

        treeAudio.PlayTreeSproutSFX();
        treeAudio.StartCoroutine(treeAudio.StartTreeGrowthSFX());

        FruitControl fruitControl = tree.GetComponent<FruitControl>();

        StartCoroutine(fruitControl.FruitGrowthBuffer());

        TreeDeathManager treeDeathManager = tree.GetComponent<TreeDeathManager>();
        treeDeathManager.treeAudioSFX = treeAudio;

        StartCoroutine(ParticleTimeOut(dirtExplodeParticles));

        yield break;
    }

    private IEnumerator ParticleTimeOut(ParticleSystem dirtExplodeParticles)
    {
        yield return new WaitForSeconds(dirtExplodeParticles.main.duration);
        dirtExplodeParticles.Stop();
        yield break;
    }

    [SerializeField] private float minDecayDuration = 5f;
    [SerializeField] public float maxDecayDuration = 10f;

    [SerializeField] public float minDecayDelayTime = 5f;
    [SerializeField] public float maxDecayDelayTime = 10f;

    [SerializeField] public float minFruitFallBuffer = 1f;
    [SerializeField] public float maxFruitFallBuffer = 60f;

    public IEnumerator WaitUntilFruitGrown(GameObject growObject, ScaleControl scaleControl)
    {
        yield return new WaitUntil(() => scaleControl.isFullyGrown);

        float fallBuffer = Random.Range(minFruitFallBuffer, maxFruitFallBuffer);

        yield return new WaitForSeconds(fallBuffer);

        if (growObject.transform.CompareTag("Apple") || growObject.transform.CompareTag("Stick"))
        {
            Rigidbody rigidBody = growObject.transform.GetComponent<Rigidbody>();
            Collider collider = growObject.transform.GetComponent<Collider>();
            HitGround hitGround = growObject.GetComponent<HitGround>();

            rigidBody.useGravity = true; // Object falling to ground.
            rigidBody.isKinematic = false;

            yield return new WaitUntil(() => hitGround.hit);

            float decayDuration = Random.Range(minDecayDuration, maxDecayDuration);
            float decayDelay = Random.Range(minDecayDelayTime, maxDecayDelayTime);

            StartCoroutine(scaleControl.LerpScale(growObject, growObject.transform.localScale, Vector3.zero, decayDuration, decayDelay));

            Destroy(growObject, decayDelay + decayDuration);

            yield break;

        } else if (growObject.transform.CompareTag("AppleTree"))
        {
            yield break;
        }
    }

    void GrassPoissonDisc(PoissonDiscSampler grassSampler)
    {
        foreach (Vector2 sample in grassSampler.Samples())
        {
            GameObject randomGrass = GetRandomMapObject(grass);

            GameObject grassInstance = Instantiate(randomGrass, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            grassInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            grassInstance.transform.localScale = new Vector3(
            Random.Range(minGrassScale.x, maxGrassScale.x),
            Random.Range(minGrassScale.y, maxGrassScale.y),
            Random.Range(minGrassScale.z, maxGrassScale.z));

            grassInstance.tag = grassTag;

            int grassLayer = LayerMask.NameToLayer("Grass");
            grassInstance.layer = grassLayer;

            grassInstance.transform.SetParent(hierarchyRoot.transform);

            grassInstance.AddComponent<NavMeshModifier>();

            grassInstance.transform.gameObject.AddComponent<CorruptionControl>();

            mapObjectList.Add(grassInstance);
            weather.windAffectedRendererList.Add(grassInstance.transform);
            grassList.Add(grassInstance);


        }
    }

    void CavePoissonDisc(PoissonDiscSampler caveSampler)
    {
        foreach (Vector2 sample in caveSampler.Samples())
        {
            GameObject randomCave = GetRandomMapObject(cave);

            GameObject caveInstance = Instantiate(randomCave, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            caveInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            caveInstance.transform.localScale = new Vector3(
            Random.Range(minGrassScale.x, maxGrassScale.x),
            Random.Range(minGrassScale.y, maxGrassScale.y),
            Random.Range(minGrassScale.z, maxGrassScale.z));

            caveInstance.tag = grassTag;

            caveInstance.transform.SetParent(hierarchyRoot.transform);

            caveInstance.AddComponent<NavMeshModifier>();

            mapObjectList.Add(caveInstance);
        }
    }

    void FoliagePoissonDisc(PoissonDiscSampler foliageSamples)
    {
        foreach (Vector2 sample in foliageSamples.Samples())
        {
            GameObject randomFoliage = GetRandomMapObject(foliage);

            GameObject foliageInstance = Instantiate(randomFoliage, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            foliageInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            foliageInstance.transform.localScale = new Vector3(
            Random.Range(minFoliageScale.x, maxFoliageScale.x),
            Random.Range(minFoliageScale.y, maxFoliageScale.y),
            Random.Range(minFoliageScale.z, maxFoliageScale.z));

            foliageInstance.tag = foliageTag;

            int foliageLayer = LayerMask.NameToLayer("Foliage");
            foliageInstance.layer = foliageLayer;

            foliageInstance.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(foliageInstance);
        }
    }

    void RocksPoissonDisc(PoissonDiscSampler rockSamples)
    {
        foreach (Vector2 sample in rockSamples.Samples())
        {
            GameObject randomRocks = GetRandomMapObject(rocks);

            float rockYboost = 35;

            GameObject rockInstance = Instantiate(randomRocks, new Vector3(sample.x, initY + rockYboost, sample.y), Quaternion.identity);

            rockInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            rockInstance.transform.localScale = new Vector3(
            Random.Range(minRockScale.x, maxRockScale.x),
            Random.Range(minRockScale.y, maxRockScale.y),
            Random.Range(minRockScale.z, maxRockScale.z));

            rockInstance.tag = rockTag;

            int rockLayer = LayerMask.NameToLayer("Rocks");
            rockInstance.layer = rockLayer;

            rockInstance.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(rockInstance);
        }
    }

    void MushroomPoissonDisc(PoissonDiscSampler mushroomSampler)
    {
        foreach (Vector2 sample in mushroomSampler.Samples())
        {
            GameObject randomMushroom = GetRandomMapObject(mushrooms);

            GameObject mushroomInstance = Instantiate(randomMushroom, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            MushroomGrowth mushroomGrowth = mushroomInstance.transform.GetComponent<MushroomGrowth>();
            mushroomGrowth.player = player;

            mushroomInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            mushroomInstance.transform.localScale = new Vector3(
            Random.Range(minMushroomScale.x, maxMushroomScale.x),
            Random.Range(minMushroomScale.y, maxMushroomScale.y),
            Random.Range(minMushroomScale.z, maxMushroomScale.z));

            ScaleControl mushroomGrowControl = mushroomInstance.transform.GetComponent<ScaleControl>();

            mushroomGrowControl.rainControl = rainControl;

            mushroomInstance.tag = mushroomTag;

            mushroomInstance.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(mushroomInstance);
        }
    }

    void FliesPoissonDisc(PoissonDiscSampler fliesSampler)
    {
        foreach (Vector2 sample in fliesSampler.Samples())
        {
            GameObject randomFlies = GetRandomMapObject(flies);

            GameObject fliesInstance = Instantiate(randomFlies, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            fliesInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            fliesInstance.transform.localScale = new Vector3(
            Random.Range(minFliesScale.x, maxFliesScale.x),
            Random.Range(minFliesScale.y, maxFliesScale.y),
            Random.Range(minFliesScale.z, maxFliesScale.z));


            fliesInstance.tag = fliesTag;

            int fliesLayer = LayerMask.NameToLayer("Flies");
            fliesInstance.layer = fliesLayer;

            fliesInstance.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(fliesInstance);
        }
    }

    void FireWoodPoissonDisc(PoissonDiscSampler fireWoodSampler)
    {
        foreach (Vector2 sample in fireWoodSampler.Samples())
        {
            GameObject randomFireWood = GetRandomMapObject(fireWood);

            GameObject fireWoodInstance = Instantiate(randomFireWood, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            fireWoodInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            fireWoodInstance.transform.localScale = new Vector3(
            Random.Range(minFireWoodScale.x, maxFireWoodScale.x),
            Random.Range(minFireWoodScale.y, maxFireWoodScale.y),
            Random.Range(minFireWoodScale.z, maxFireWoodScale.z));


            fireWoodInstance.tag = fireWoodTag;

            int fireWoodLayer = LayerMask.NameToLayer("FireWood");
            fireWoodInstance.layer = fireWoodLayer;

            fireWoodInstance.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(fireWoodInstance);
        }
    }
    
    void GroundCheck(GameObject obj)
    {

        if (Physics.Raycast(obj.transform.position, Vector3.down, out RaycastHit downHit, Mathf.Infinity))
        {
            //Debug.DrawRay(mapObject.transform.position, Vector3.down, Color.red);

            if (downHit.collider.CompareTag(waterTag))
            {
                DestroyObject();
                //Debug.Log("Water Ahoy!");
            }

            if (downHit.collider == null)
            {
                DestroyObject();
                //Debug.Log("Nothing detected.");
            }
        }

        void DestroyObject()
        {
            if (Application.isEditor)
            {
                DestroyImmediate(obj);
                // Debug.Log("Object destroyed in Editor.");
            }
            else if (!Application.isEditor)
            {
                Destroy(obj);
                // Debug.Log("Object destroyed in game.");
            }
        }

        AnchorToGround(obj);
    }

    void AnchorToGround(GameObject obj)
    {
        if (obj != null)
        {
            if (Physics.Raycast(obj.transform.position, Vector3.down, out RaycastHit hitFloor, Mathf.Infinity, groundLayerMask))
            {
                float distance = hitFloor.distance;

                float x = obj.transform.position.x;
                float y = obj.transform.position.y - distance;
                float z = obj.transform.position.z;

                Vector3 newPosition = new Vector3(x, y, z);

                obj.transform.position = newPosition;

                //Debug.Log("Clamped to Ground!");
                //Debug.Log("Distance: " + distance);
            }
        }
        

    }

    void DestroyDeadZones()
    {
        if (Physics.Raycast(mapObject.transform.position, Vector3.down, out RaycastHit hitFloor, Mathf.Infinity, deadZoneLayerMask))
        {
            if (hitFloor.collider.CompareTag("DeadZone"))
            {
                //Debug.Log("DeadZone hit.");
                DestroyObject();
            }
        }


        void DestroyObject()
        {
            if (Application.isEditor)
            {
                Debug.Log("Object destroyed in Editor.");
                DestroyImmediate(mapObject);
            }
            else
            {
                Debug.Log("Object destroyed in game.");
                Destroy(mapObject);
            }
        }
        
    }

    void SetOffset()
    {
        mapObject.transform.position = new Vector3(xOffset, initY, zOffset);
    }

    void ResetPosOffset()
    {
        mapObject.transform.position = new Vector3(0, initY, 0);
    }

    public void Clear()
    {
        mapObjectList.Clear();

        if (Application.isEditor)
        {

            ResetPosOffset();

            while (hierarchyRoot.transform.childCount != 0)
            {
                foreach (Transform child in hierarchyRoot.transform)
                {
                    DestroyImmediate(child.gameObject);
                    if (hierarchyRoot.transform.childCount != 0)
                    {
                        continue;
                    }

                }
            }

            while (EmittersContainer.childCount != 0)
            {
                foreach (Transform child in EmittersContainer)
                {
                    DestroyImmediate(child.gameObject);
                    if (EmittersContainer.childCount != 0)
                    {
                        continue;
                    }

                }
            }

        }

    }
}

    // Update is called once per frame


