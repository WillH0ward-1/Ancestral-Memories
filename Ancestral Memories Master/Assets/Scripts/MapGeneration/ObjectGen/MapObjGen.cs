using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Deform;
using FIMSpace.FLook;
//using FMODUnity;
using Pathfinding;
using ProceduralModeling;

public class MapObjGen : MonoBehaviour
{
    //public int density;

    [Header("Map Object Generator")]
    [Header("========================================================================================================================")]

    public bool mapObjectsGenerated = false;

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
    [SerializeField] private GameObject[] cave;
    [SerializeField] private GameObject[] humans;
    [SerializeField] private GameObject[] proceduralTrees;
    [SerializeField] private GameObject[] limeStones;
    [SerializeField] private GameObject[] temples;
    [SerializeField] private GameObject[] shamans;

    [SerializeField] private GameObject spawnPointPrefab;

    [Header("========================================================================================================================")]
    [Header("Layer Masks")]
    [Space(10)]

    [SerializeField] private LayerMask caveLayerMask;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask groundAndWaterLayerMask;
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

    [SerializeField] Vector3 minCaveScale;
    [SerializeField] Vector3 maxCaveScale;

    [SerializeField] Vector3 minLimeStoneScale;
    [SerializeField] Vector3 maxLimeStoneScale;

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
    [SerializeField] float minimumCaveRadius = 70;
    [SerializeField] float minimumHumanRadius = 70;
    [SerializeField] float minimumLimeStoneRadius = 70;
    [SerializeField] float minimumSpawnPointRadius = 70;
    [SerializeField] float minimumTempleRadius = 70;
    [SerializeField] float minimumShamanRadius = 70;

    [Header("========================================================================================================================")]

    [Header("Max Objects")]
    [Space(10)]

    public int maxHumans = 0;

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
    private readonly string humanTag = "Human";
    private readonly string caveTag = "Cave";
    private readonly string limeStoneTag = "Limestone";
    private readonly string structureTag = "Structure";
    private readonly string shamanTag = "Shaman";

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

    public RadialMenu radialMenu;
    public Camera cam;

    [SerializeField] int vertSampleFactor;
    [SerializeField] private bool invertSpreadOrigin = false;
    private Vector3 mapCenter;


    [Header("========================================================================================================================")]
    [Header("Terrain")]
    [Space(10)]

    [SerializeField] private LerpTerrain lerpTerrain;

    [Header("========================================================================================================================")]
    [Header("Seasons")]
    [Space(10)]

    [SerializeField] private SeasonManager seasonManager;

    [Header("========================================================================================================================")]
    [Header("Weather")]
    [Space(10)]

    public WeatherControl weather;
    [SerializeField] private RainControl rainControl;
    [SerializeField] private TreeDeathManager treeFallManager;
    public TimeCycleManager timeCycleManager;

    [Header("========================================================================================================================")]

    [Header("Rock Settings")]

    [SerializeField] private float rockYOffset = 0;
    private Material rockMaterial;

    [Header("========================================================================================================================")]
    [Header("Sound Emitters")]
    [Space(10)]

    [SerializeField] private AudioSFXManager audioManager;

    [Header("========================================================================================================================")]
    [Header("Sound Emitters")]
    [Space(10)]

    private GameObject emitterInstance;

    private float emitterY;
    [SerializeField] private float checkOffset = 10f;

    [SerializeField] private Transform EmittersContainer;
    [SerializeField] private GameObject waterSoundEmitter;
    [SerializeField] private Transform waterEmitterTransform;
    [SerializeField] private Transform waterEmitterRoot;
    [SerializeField] private List<GameObject> waterEmitters;

    private Vector3[] waterEmitterVerts;

    [Header("========================================================================================================================")]
    [Header("Behaviours Reference")]
    [Space(10)]

    [SerializeField] private CharacterBehaviours playerBehaviours;
    public Player player;
    [SerializeField] private InteractCamera interactCam;
    [SerializeField] private ResourcesManager resources;
    [SerializeField] private Camera camera;

    [Header("========================================================================================================================")]
    [Header("Generated Objects")]
    [Space(10)]

    private Vector3 zeroScale = new Vector3(0.0000001f, 0.0000001f, 0.0000001f);

    private List<List<GameObject>> listOfLists = new List<List<GameObject>>();
    List<List<AICharacterStats>> listOfStatsLists = new List<List<AICharacterStats>>();
    public List<GameObject> mapObjectList;
    public List<GameObject> treeList;
    public List<GameObject> treeGrowingList;
    public List<GameObject> npcList;
    public List<GameObject> grassList;
    public List<GameObject> templeList;

    [Header("========================================================================================================================")]
    [Header("Resources")]
    [Space(10)]

    public List<GameObject> foodSourcesList;
    public List<GameObject> limeStoneList;
    public List<GameObject> stoneList;

    [Header("========================================================================================================================")]
    [Header("NPC's")]
    [Space(10)]

    public List<GameObject> humanPopulationList;
    public List<GameObject> huntableAnimalsList;
    public List<GameObject> shamanList;

    [Header("========================================================================================================================")]
    [Header("Flammable Objects")]
    [Space(10)]

    public List<GameObject> flammableObjectList;


    private List<Vector3> XspawnPositions;
    private List<Vector3> ZspawnPositions;

    public List<AICharacterStats> allHumanStats;
    private VocabularyManager vocabularyManager;
    private DialogueLines dialogueLines;

    private float minDistanceFromCenter;

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

    private MushroomGrowth mushroomControl;

    [SerializeField] private float minDecayDuration = 5f;
    [SerializeField] public float maxDecayDuration = 10f;

    [SerializeField] public float minDecayDelayTime = 5f;
    [SerializeField] public float maxDecayDelayTime = 10f;

    [SerializeField] public float minFruitFallBuffer = 1f;
    [SerializeField] public float maxFruitFallBuffer = 60f;

    public List<GameObject> spawnPointsList;

    private FluteControl fluteControl;

    private DisasterManager disasterManager;

    private List<PTGrowing> ptGrowComponents = new List<PTGrowing>();

    private float randomTreeColourSeed = 0;
    public bool readyToGrow = false;

    private FireManager fireManager;

    private CamControl camControl;
    private FollowersManager followersManager;

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
            StartCoroutine(CheckOverWater(waterEmitters));
        }

        yield break;
    }

    public static bool IsOverWater(GameObject obj, float emitterYOffset, out float hitPointY)
    {
        hitPointY = 0f; // Default value
        float emitterY = obj.transform.position.y + emitterYOffset;
        Vector3 emitterPosition = new Vector3(obj.transform.position.x, emitterY, obj.transform.position.z);

        if (Physics.Raycast(emitterPosition, Vector3.down, out RaycastHit downHit, Mathf.Infinity))
        {
            if (downHit.collider.CompareTag("Water"))
            {
                hitPointY = downHit.point.y;
                return true;
            }
        }

        return false;
    }


    private void InitializeFlute()
    {
        fluteControl = player.GetComponentInChildren<FluteControl>();

        fluteControl.player = player;
        fluteControl.behaviours = playerBehaviours;
        fluteControl.mapObjGen = this;
    }

    private void LogNullWarning(string variableName)
    {
        Debug.LogWarning("MAPOBJGEN: " + variableName + " has not been set in MapObjGen inspector!");
    }

    private void SetupPlayer()
    {
        mapCenter = Vector3.zero;
        camera = Camera.main;
        player = FindObjectOfType<Player>();
        interactCam = camera.GetComponentInChildren<InteractCamera>();
        interactCam.InitInteractions();
        camControl = camera.GetComponentInChildren<CamControl>();
        camControl.timeCycleManager = timeCycleManager;
        playerBehaviours = player.GetComponentInChildren<CharacterBehaviours>();
        playerBehaviours.mapObjGen = this;
        rainControl = player.GetComponentInChildren<RainControl>();
        seasonManager = FindObjectOfType<SeasonManager>();
        weather = FindObjectOfType<WeatherControl>();
        weather.player = player;
        rainControl.seasonManager = seasonManager;
        dialogueLines = GetComponentInChildren<DialogueLines>();
        vocabularyManager = GetComponentInChildren<VocabularyManager>();
        fireManager = GetComponentInChildren<FireManager>();
        fireManager.mapObjGen = this;
        resources = FindObjectOfType<ResourcesManager>();
        followersManager = player.GetComponentInChildren<FollowersManager>();
        followersManager.resourcesManager = resources;
        AudioFootStepManager playerAudioFootStepManager = player.GetComponentInChildren<AudioFootStepManager>();
        playerAudioFootStepManager.lerpTerrain = lerpTerrain;

        FormationController formationController = GetComponentInChildren<FormationController>();
        formationController.mapObjGen = this;
        formationController.InitFormationAgents();
        AICharacterStats stats = player.GetComponentInChildren<AICharacterStats>();
        stats.time = timeCycleManager;
        stats.SubscribeToBirthday();
        
    }

    void InitWind()
    {
        if (weather != null)
        {
            weather.InitializeWindZonePool();
        } else
        {
            Debug.LogError("WeatherControl component reference in MapObjGen 'InitWind' is null!");
        }
    }

    public IEnumerator InitAllRelationships()
    {
        // Directly iterate over NPCs to initialize their relationships
        foreach (GameObject npc in humanPopulationList)
        {
            if (npc != null)
            {
                AICharacterStats stats = npc.GetComponentInChildren<AICharacterStats>();
                if (stats != null)
                {
                    stats.mapObjGen = this;
                    stats.player = player;
                    stats.InitAllRelationships(humanPopulationList);
                }
            }
        }

        // Initialize the player's relationships if needed
        AICharacterStats playerStats = player.GetComponentInChildren<AICharacterStats>();
        if (playerStats != null)
        {
            playerStats.mapObjGen = this;
            playerStats.player = player;
            playerStats.InitAllRelationships(humanPopulationList);
        }

        yield break;
    }



    private void InitAudioManager()
    {
        foreach (GameObject g in npcList)
        {
            if (g != null)
            {
                AudioSFXManager audio = g.GetComponentInChildren<AudioSFXManager>();

                if (audio != null)
                {
                    audio.player = player;
                    audio.behaviours = playerBehaviours;
                }
                else
                {
                    continue;
                }
            }
        }

        AudioSFXManager playerAudio = player.GetComponentInChildren<AudioSFXManager>();
        playerAudio.player = player;
        playerAudio.behaviours = playerBehaviours;
    }

    private void SetupDeformers()
    {
        foreach(GameObject g in npcList)
        {
            LerpDeformation npcDeform = g.GetComponentInChildren<LerpDeformation>();
            npcDeform.stats = player;
            npcDeform.SubscribeToHunger();
            //DeformableManager npcDeformManager = npcDeform.transform.gameObject.AddComponent<DeformableManager>();
            //npcDeformManager.update = true;
        }

        LerpDeformation playerDeform = player.GetComponentInChildren<LerpDeformation>();
        playerDeform.stats = player;
        playerDeform.SubscribeToHunger();

        //DeformableManager playerDeformManager = playerDeform.transform.gameObject.AddComponent<DeformableManager>();
        // playerDeformManager.update = true;
    }

    void InitializeLists()
    {
        listOfLists.Add(mapObjectList);
        listOfLists.Add(npcList);
        listOfLists.Add(humanPopulationList);
        listOfLists.Add(huntableAnimalsList);
        listOfLists.Add(flammableObjectList);
        listOfLists.Add(limeStoneList);
        listOfLists.Add(treeList);
        listOfLists.Add(grassList);
        listOfLists.Add(stoneList);
        listOfLists.Add(spawnPointsList);
        listOfStatsLists.Add(allHumanStats);

    }

    void RemoveGameObjectFromAllLists(GameObject objToRemove)
    {
        foreach (var list in listOfLists)
        {
            if (list.Contains(objToRemove))
            {
                list.Remove(objToRemove);
            }
        }
    }

    void RemoveAIStatsFromAllLists(AICharacterStats stats)
    {
        foreach (var list in listOfStatsLists)
        {
            if (list.Contains(stats))
            {
                list.Remove(stats);
            }
        }
    }



    public void GenerateMapObjects()
    {
        SetupPlayer();
        
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
        PoissonDiscSampler caveSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumCaveRadius);
        PoissonDiscSampler humanSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumHumanRadius);
        PoissonDiscSampler limeStoneSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumLimeStoneRadius);
        PoissonDiscSampler templeSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumTempleRadius);
        PoissonDiscSampler shamanSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumShamanRadius);

        //TreePoissonDisc(treeSampler);
        //AppleTreePoissonDisc(appleTreeSampler);
        GrassPoissonDisc(grassSampler);
        //FoliagePoissonDisc(foliageSampler);
        RocksPoissonDisc(rockSampler);
        //FliesPoissonDisc(fliesSampler);
        AnimalPoissonDisc(animalSampler);
        MushroomPoissonDisc(mushroomSampler);
        //FireWoodPoissonDisc(fireWoodSampler);
        //SeaShellPoissonDisc(seaShellSampler);
        //CavePoissonDisc(caveSampler);
        SpawnPointsPoissonDisc(spawnPointsSampler);
        HumanPoissonDisc(humanSampler);
        ProceduralTreePoissonDisc(treeSampler);
        LimeStonePoissonDisc(limeStoneSampler);

        InitializeLists();

        //TemplePoissonDisc(templeSampler);

        SetOffset();

        GroundCheck(mapObjectList);
        GroundCheck(spawnPointsList);

        StartCoroutine(CheckOverWater(limeStoneList));
        StartCoroutine(WaterSFXEmitterGen());
        DestroyDeadZones();

        SetMaxObj(humanPopulationList, maxHumans);

        //ListCleanup(templeList);

        //GenerateTemples();

        mapObjectsGenerated = true;

        SetupCorruptionControl();

        InitAudioManager();

        RandomizeHumanRace();
        fireManager.GenerateFirePoints();
        EnableStudioEmitters(grassList);

        GetPTGrowComponents();

        InitWind();

        //StartCoroutine(StartProceduralTreeGrowth(treeList));

        SpawnPlayer();
        SpawnShaman();

        SetupShaderLightColours();
        SetupDeformers();
        SetupDisasters();
        StartCoroutine(InitAllRelationships());
        //StartCoroutine(StartTreeGrowth(treeList));

        //EnableNavMeshAgents(npcList);
        //RandomizeTreecolours();

    }



    void SetMaxObj(List<GameObject> listOfObjects, int maxValue)
    {
        // Ensure maxValue is not negative.
        maxValue = Mathf.Max(maxValue, 0);

        // Check if the current count exceeds the maximum allowed value.
        while (listOfObjects.Count > maxValue)
        {
            // Calculate the index of the last object in the list.
            int lastIndex = listOfObjects.Count - 1;

            // Retrieve the last object.
            GameObject objToRemove = listOfObjects[lastIndex];

            if (objToRemove != null)
            {
                // Check if the object is a human and requires special handling.
                if (objToRemove.CompareTag("Human"))
                {
                    // Attempt to remove AI stats from all lists if they exist.
                    var stats = objToRemove.GetComponentInChildren<AICharacterStats>();
                    if (stats != null)
                    {
                        RemoveAIStatsFromAllLists(stats);
                    }

                    // Use the centralized method to remove the GameObject from all lists.
                    RemoveGameObjectFromAllLists(objToRemove);
                }
                else
                {
                    // For non-human objects, just remove from the current list.
                    listOfObjects.RemoveAt(lastIndex);
                }

                // Destroy the object after handling list removals.
                DestroyObject(objToRemove);
            }
            else
            {
                // Just in case the object is null, remove it from the list.
                listOfObjects.RemoveAt(lastIndex);
            }
        }
    }



    void GroundCheck(List<GameObject> objectsList)
    {
        for (int i = objectsList.Count - 1; i >= 0; i--)
        {
            GameObject obj = objectsList[i];
            if (obj != null && !obj.CompareTag(limeStoneTag))
            {
                if (Physics.Raycast(obj.transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundAndWaterLayerMask))
                {
                    if (hit.collider.CompareTag(waterTag))
                    {
                        if (obj.CompareTag("Human"))
                        {
                            var stats = obj.GetComponentInChildren<AICharacterStats>();
                            if (stats != null) RemoveAIStatsFromAllLists(stats);
                        }
                        RemoveGameObjectFromAllLists(obj);
                        DestroyObject(obj);
                    }
                    else
                    {
                        AnchorToGround(obj);
                    }
                }
                else
                {
                    if (obj.CompareTag("Human"))
                    {
                        var stats = obj.GetComponentInChildren<AICharacterStats>();
                        if (stats != null) RemoveAIStatsFromAllLists(stats);
                    }
                    RemoveGameObjectFromAllLists(obj);
                    DestroyObject(obj);
                }
            }
        }
    }

    public IEnumerator CheckOverWater(List<GameObject> list)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            GameObject obj = list[i];
            if (obj != null && IsOverWater(obj, checkOffset, out float hitPointY))
            {
                obj.transform.position = new Vector3(obj.transform.position.x, hitPointY, obj.transform.position.z);
            }
            else
            {
                if (obj.CompareTag("Human"))
                {
                    var stats = obj.GetComponentInChildren<AICharacterStats>();
                    if (stats != null) RemoveAIStatsFromAllLists(stats);
                }
                RemoveGameObjectFromAllLists(obj);
                DestroyObject(obj);
            }
        }
        yield break;
    }

    void DestroyDeadZones()
    {
        for (int i = mapObjectList.Count - 1; i >= 0; i--)
        {
            GameObject gameObject = mapObjectList[i];
            if (gameObject != null && Physics.Raycast(gameObject.transform.position, Vector3.down, out RaycastHit hitFloor, Mathf.Infinity, deadZoneLayerMask))
            {
                if (hitFloor.collider.CompareTag("DeadZone") && !gameObject.CompareTag("Structure"))
                {
                    if (gameObject.CompareTag("Human"))
                    {
                        var stats = gameObject.GetComponentInChildren<AICharacterStats>();
                        if (stats != null) RemoveAIStatsFromAllLists(stats);
                    }
                    RemoveGameObjectFromAllLists(gameObject);
                    DestroyObject(gameObject);
                }
            }
        }
    }


    void DestroyObject(GameObject obj)
    {
        if (Application.isEditor)
        {
            DestroyImmediate(obj);
        }
        else
        {
            Destroy(obj);
        }
    }


    void SetupShaderLightColours()
    {
        foreach (GameObject obj in mapObjectList)
        {
            if (obj != null)
            {
                // Get all ShaderLightColor components in the children of the object, including inactive ones
                ShaderLightColor[] shaderComponents = obj.GetComponentsInChildren<ShaderLightColor>(true);

                // Iterate through each ShaderLightColor component and assign timeCycleManager
                foreach (ShaderLightColor shader in shaderComponents)
                {
                    shader.timeCycleManager = timeCycleManager;
                }
            }
        }
    }

    public void SetAllHumanInteractables(InteractableManager.InteractableAction newAction)
    {
        foreach(GameObject human in humanPopulationList)
        {
            if (human != null)
            {
                InteractableManager interactableManager = human.GetComponentInChildren<InteractableManager>();
                if (interactableManager != null)
                {
                    interactableManager.AddOption(newAction);
                }
            }
        }
    }

    private void SetupDisasters()
    {
        disasterManager = FindObjectOfType<DisasterManager>();
        disasterManager.player = player;
        disasterManager.humanTargets = humanPopulationList;
        disasterManager.animalTargets = huntableAnimalsList;
        disasterManager.treeTargets = treeList;
        disasterManager.InitDisasters();
    }

    public IEnumerator StartProceduralTreeGrowth(List<GameObject> treeList)
    {
        foreach (GameObject tree in treeList)
        {
            //ProceduralModelingBase ptBase = tree.GetComponentInChildren<ProceduralModelingBase>();
            //ptBase.player = player;
            if (tree != null)
            {
                PTGrowing ptGrowing = tree.GetComponentInChildren<PTGrowing>();
                // TreeFruitManager fruitManager = tree.GetComponentInChildren<TreeFruitManager>();
                ptGrowing.SetupBounds();
                StartCoroutine(ptGrowing.GrowTreeInstant());
            }
            // fruitManager.InitializeFruits(fruitManager.maxFruits);
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

            if (human != null)
            {
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
    }

    void ListCleanup(List<GameObject> list)
    {
        for (var i = list.Count - 1; i > -1; i--)
        {
            if (list[i] == null)
                list.RemoveAt(i);
        }
    }

    void ListCleanupAIStats(List<AICharacterStats> list)
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
                /* 
                StudioEventEmitter eventEmitter = emitter.transform.GetComponent<StudioEventEmitter>();
                eventEmitter.enabled = true;

                eventEmitter.enabled = true;

                 eventEmitter.Play();
                */
            }
        }
    }

    private GameObject playerInitialSpawnPoint;

    public GameObject GetRandomMapObject(GameObject[] mapElements)
    {
        return mapElements[Random.Range(0, mapElements.Length)];
    }

    void SpawnPlayer()
    {
        InitializeFlute();
        playerInitialSpawnPoint = SetInitialPlayerSpawnPosition(spawnPointsList);
        player.transform.position = playerInitialSpawnPoint.transform.position;
    }

    public GameObject SetInitialPlayerSpawnPosition(List<GameObject> spawnPointsList)
    {
        int randomIndex = Random.Range(0, spawnPointsList.Count);
        GameObject initialSpawnPoint = spawnPointsList[randomIndex];
        // Optionally remove the spawn point from the list if it should not be used again for initialization
        // spawnPointsList.RemoveAt(randomIndex);
        return initialSpawnPoint;
    }
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

    public GameObject GetRandomSpawnPointExcludingPlayer()
    {
        List<GameObject> availableSpawnPoints = new List<GameObject>(spawnPointsList);
        availableSpawnPoints.Remove(playerInitialSpawnPoint); // Exclude player's initial spawn point

        if (availableSpawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points available!");
            return null;
        }

        int randomIndex = Random.Range(0, availableSpawnPoints.Count);
        return availableSpawnPoints[randomIndex];
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

            deform.enabled = false;

            //NavMeshAgent agent = animalInstance.GetComponentInChildren<NavMeshAgent>();
 
            FLookAnimator lookAnimator = animalInstance.GetComponentInChildren<FLookAnimator>();
            lookAnimator.enabled = true;
            //lookAnimator.ObjectToFollow = player.transform;

            AnimalAI animalAI = animalInstance.GetComponentInChildren<AnimalAI>();
            animalAI.player = player;
            animalAI.playerBehaviours = playerBehaviours;
            animalAI.lookAnimator = lookAnimator;

            AICharacterStats animalStats = animalInstance.GetComponentInChildren<AICharacterStats>();
            animalStats.time = timeCycleManager;
            animalStats.SubscribeToBirthday();

            Dialogue dialogue = animalInstance.GetComponentInChildren<Dialogue>();
            dialogue.player = player;
            dialogue.vocabularyManager = vocabularyManager;
            dialogue.dialogueLines = dialogueLines;

            deform.enabled = true;

            mapObjectList.Add(animalInstance);
            npcList.Add(animalInstance);
            huntableAnimalsList.Add(animalInstance);
            flammableObjectList.Add(animalInstance);
            //GroundCheck(instantiatedPrefab);
            //WaterCheck();
        }
    }

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
            deform.enabled = false;

            HumanAI humanAI = humanInstance.GetComponentInChildren<HumanAI>();
            humanAI.mapObjGen = this;
            humanAI.player = player;
            humanAI.playerBehaviours = playerBehaviours;
            humanAI.resources = resources;
            humanAI.seasonManager = seasonManager;

            Dialogue dialogue = humanInstance.GetComponentInChildren<Dialogue>();
            dialogue.player = player;
            dialogue.vocabularyManager = vocabularyManager;
            dialogue.dialogueLines = dialogueLines;

            AICharacterStats humanStats = humanInstance.GetComponentInChildren<AICharacterStats>();
            humanStats.time = timeCycleManager;
            humanStats.SubscribeToBirthday();
            allHumanStats.Add(humanStats);

            FLookAnimator lookAnimator = humanInstance.GetComponentInChildren<FLookAnimator>();
            lookAnimator.enabled = true;
            lookAnimator.ObjectToFollow = player.transform;

            humanAI.lookAnimator = lookAnimator;

            deform.enabled = true;

            AudioFootStepManager humanAudioFootStepManager = humanInstance.GetComponentInChildren<AudioFootStepManager>();
            humanAudioFootStepManager.lerpTerrain = lerpTerrain;

            mapObjectList.Add(humanInstance);
            npcList.Add(humanInstance);
            humanPopulationList.Add(humanInstance);
            flammableObjectList.Add(humanInstance);

            humanAI.InitHuman();
        }
    }


    void SpawnShaman()
    {

        GameObject shaman = GetRandomMapObject(shamans);

        GameObject shamanInstance = Instantiate(shaman, GetRandomSpawnPointExcludingPlayer().transform.position, Quaternion.identity);

        shamanInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

        shamanInstance.tag = shamanTag;

        int humanLayer = LayerMask.NameToLayer("Human");
        shamanInstance.layer = humanLayer;

        float randomXScale = humanAverageScale.x;
        float randomYScale = humanAverageScale.y;
        float randomZScale = humanAverageScale.z;

        shamanInstance.transform.localScale = new Vector3(randomXScale, randomYScale, randomZScale);
        shamanInstance.transform.SetParent(hierarchyRoot.transform);

        LerpDeformation deform = shamanInstance.transform.GetComponentInChildren<LerpDeformation>();
        deform.enabled = false;

        HumanAI humanAI = shamanInstance.GetComponentInChildren<HumanAI>();
        humanAI.mapObjGen = this;
        humanAI.player = player;
        humanAI.playerBehaviours = playerBehaviours;
        humanAI.resources = resources;
        humanAI.seasonManager = seasonManager;

        Dialogue dialogue = shamanInstance.GetComponentInChildren<Dialogue>();
        dialogue.player = player;
        dialogue.vocabularyManager = vocabularyManager;
        dialogue.dialogueLines = dialogueLines;

        AICharacterStats humanStats = shamanInstance.GetComponentInChildren<AICharacterStats>();
        humanStats.time = timeCycleManager;
        humanStats.SubscribeToBirthday();
        allHumanStats.Add(humanStats);


        FLookAnimator lookAnimator = shamanInstance.GetComponentInChildren<FLookAnimator>();
        lookAnimator.enabled = true;
        lookAnimator.ObjectToFollow = player.transform;

        humanAI.lookAnimator = lookAnimator;

        deform.enabled = true;

        mapObjectList.Add(shamanInstance);
        npcList.Add(shamanInstance);
        humanPopulationList.Add(shamanInstance);

        humanAI.InitHuman();

        //GroundCheck(instantiatedPrefab);
        //WaterCheck();
        
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

    private float GetDistanceFromCenter(GameObject mapObject)
    {
        float distanceFromCenter = Vector3.Distance(mapObject.transform.position, mapCenter);

        return distanceFromCenter;
    }

    void SetupCorruptionControl()
    {
        foreach (GameObject mapObj in mapObjectList)
        {
            if (mapObj != null)
            {
                CorruptionControl corruptionControl = mapObj.transform.gameObject.GetComponentInChildren<CorruptionControl>();
                if (corruptionControl == null)
                {
                    corruptionControl = mapObj.transform.gameObject.AddComponent<CorruptionControl>();
                }

                corruptionControl.rain = rainControl;
                corruptionControl.player = player;
                corruptionControl.behaviours = playerBehaviours;

                if (mapObj.transform.CompareTag("Animal"))
                {
                    corruptionControl.newMin = 1;
                    corruptionControl.newMax = 0;
                }

                corruptionControl.CorruptionModifierActive = true;

                corruptionControl.InitCorruption();
            }
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
            ptGrow.mapObjGen = this;
            ptGrow.rainControl = rainControl;
            ptGrow.lerpTerrain = terrain.GetComponentInChildren<LerpTerrain>();
            ptGrow.seasonManager = seasonManager;

            TreeAudioManager treeAudioManager = treeInstance.transform.GetComponentInChildren<TreeAudioManager>();
            treeAudioManager.timeManager = timeCycleManager;
            treeAudioManager.weatherManager = weather;

            TreeFruitManager treeFruitManager = treeInstance.transform.GetComponentInChildren<TreeFruitManager>();
            treeFruitManager.player = player;
            treeFruitManager.mapObjGen = this;
            treeFruitManager.resources = resources;
            //ptGrow.GrowTree();

            ProceduralTree pt = treeInstance.GetComponentInChildren<ProceduralTree>();
            pt.resources = resources;
            pt.fireManager = fireManager;

            int treeLayer = LayerMask.NameToLayer("Trees");
            treeInstance.layer = treeLayer;

            mapObjectList.Add(treeInstance);

            treeInstance.transform.SetParent(hierarchyRoot.transform);

            //TreeDeathManager treeDeathManager = treeInstance.GetComponent<TreeDeathManager>();
            //treeDeathManager.mapObjGen = this;

            weather.windAffectedRendererList.Add(treeInstance.transform);

            treeList.Add(treeInstance);
            flammableObjectList.Add(treeInstance);

            RandomiseTreeTextures();

        }
    }

    private void GetPTGrowComponents()
    {
        ptGrowComponents.Clear();

        foreach (GameObject tree in treeList)
        {
            PTGrowing ptGrow = tree.GetComponentInChildren<PTGrowing>();
            ptGrowComponents.Add(ptGrow);
        }
    }

    public void KillAllTreeProduce()
    {
        foreach (PTGrowing ptGrow in ptGrowComponents)
        {
            ptGrow.KillLeaves();
            ptGrow.KillAllFruits();
        }
    }

    public void LerpLeafColour(Color leafColour)
    {
        foreach (PTGrowing ptGrow in ptGrowComponents)
        {
            StartCoroutine(ptGrow.LerpLeafColour(leafColour));
        }
    }

    public void ReviveTreeProduce()
    {
        foreach (PTGrowing ptGrow in ptGrowComponents)
        {
            ptGrow.GrowLeaves();
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
//        treeGrowControl.rainControl = rainControl;

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

            caveInstance.tag = caveTag;

            caveInstance.transform.SetParent(hierarchyRoot.transform);
         
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
            GameObject rockInstance = Instantiate(randomRocks, new Vector3(sample.x, initY, sample.y), Quaternion.identity);
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
            stoneList.Add(rockInstance);
        }
    }

    void LimeStonePoissonDisc(PoissonDiscSampler limeStoneSamples)
    {
        foreach (Vector2 sample in limeStoneSamples.Samples())
        {
            GameObject randomLimeStone = GetRandomMapObject(limeStones);
            GameObject limeStoneInstance = Instantiate(randomLimeStone, new Vector3(sample.x, initY, sample.y), Quaternion.identity);
            limeStoneInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);
            limeStoneInstance.transform.localScale = new Vector3(
            Random.Range(minLimeStoneScale.x, minLimeStoneScale.x),
            Random.Range(minLimeStoneScale.y, minLimeStoneScale.y),
            Random.Range(minLimeStoneScale.z, minLimeStoneScale.z));
            //limeStoneInstance.tag = limeStoneTag;
            //int limeStoneLayer = LayerMask.NameToLayer("LimeStone");
            //limeStoneInstance.layer = limeStoneLayer;
            limeStoneInstance.transform.SetParent(hierarchyRoot.transform);
            mapObjectList.Add(limeStoneInstance);
            limeStoneList.Add(limeStoneInstance);
        }
    }

    void TemplePoissonDisc(PoissonDiscSampler templeSamples)
    {
        foreach (Vector2 sample in templeSamples.Samples())
        {
            GameObject randomTemple = GetRandomMapObject(temples);
            GameObject templeInstance = Instantiate(randomTemple, new Vector3(sample.x, initY, sample.y), Quaternion.identity);
            templeInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            //limeStoneInstance.tag = limeStoneTag;
            //int limeStoneLayer = LayerMask.NameToLayer("LimeStone");
            //limeStoneInstance.layer = limeStoneLayer;
            templeInstance.transform.SetParent(hierarchyRoot.transform);
            mapObjectList.Add(templeInstance);
            templeList.Add(templeInstance);
        }
    }

    void GenerateTemples()
    {
        foreach(GameObject temple in templeList)
        {
            TempleGenerator templeGen = temple.GetComponentInChildren<TempleGenerator>();
            templeGen.player = player;
            templeGen.playerStats = player.GetComponentInChildren<AICharacterStats>();
            templeGen.GenerateTemple();
        }
    }


    void MushroomPoissonDisc(PoissonDiscSampler mushroomSampler)
    {
        foreach (Vector2 sample in mushroomSampler.Samples())
        {
            GameObject randomMushroom = GetRandomMapObject(mushrooms);

            GameObject mushroomInstance = Instantiate(randomMushroom, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            MushroomGrowth mushroomControl = mushroomInstance.transform.GetComponent<MushroomGrowth>();
            mushroomControl.player = player;
            mushroomControl.mapObjGen = this;
            mushroomControl.seasonManager = seasonManager;
            
            mushroomInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            mushroomInstance.transform.localScale = new Vector3(
            Random.Range(minMushroomScale.x, maxMushroomScale.x),
            Random.Range(minMushroomScale.y, maxMushroomScale.y),
            Random.Range(minMushroomScale.z, maxMushroomScale.z));

            ScaleControl mushroomGrowControl = mushroomInstance.transform.GetComponent<ScaleControl>();

//            mushroomGrowControl.rainControl = rainControl;

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


        if (!Application.isPlaying)
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


