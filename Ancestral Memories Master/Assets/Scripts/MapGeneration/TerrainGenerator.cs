﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Pathfinding;

public class TerrainGenerator : MonoBehaviour {

	const float viewerMoveThresholdForChunkUpdate = 25f;
	const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;


	public int colliderLODIndex;
	public LODInfo[] detailLevels;

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureData textureSettings;

	public GameObject navMeshContainer;
	public NavMeshSurface[] surfaces;

	[SerializeField] private MapObjGen mapObjectGen;

	//public NavMeshPrefabInstance navMeshSurface;

	public Transform viewer;
	public Material mapMaterial;

	Vector2 viewerPosition;
	Vector2 viewerPositionOld;

	public GameObject mapObject;

	float meshWorldSize;

	[SerializeField] private Interactable interactable;

	int chunksVisibleInViewDst;

	private string terrainChunkName = "Terrain Chunk";

	[SerializeField] private Player player;
	[SerializeField] private CharacterBehaviours behaviours;
	[SerializeField] private CorruptionControl corruptionControl;
	[SerializeField] private LerpTerrain lerpTerrain;

	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

	public int customSeed = 0;
	public int defaultSeed = 9;

	public enum SeedSettingState
	{
		UseDefaultSeed,
		UseRandomSeed,
		UseCustomSeed,
		None
	}

	[SerializeField]
	private SeedSettingState seedSettingState = SeedSettingState.UseDefaultSeed;

	private void SetSeed(SeedSettingState seedSettingState)
	{
		switch (seedSettingState)
		{
			case SeedSettingState.UseDefaultSeed:
				heightMapSettings.noiseSettings.seed = defaultSeed;
				break;
			case SeedSettingState.UseRandomSeed:
				heightMapSettings.noiseSettings.seed = new System.Random().Next();
				break;
			case SeedSettingState.UseCustomSeed:
				heightMapSettings.noiseSettings.seed = customSeed;
				break;
			case SeedSettingState.None:
				break;
			default:
				Debug.LogError("Invalid seed setting state.");
				break;
		}
	}

	private void Awake()
	{

		SetSeed(seedSettingState);

		surfaces = navMeshContainer.GetComponentsInChildren<NavMeshSurface>();

		foreach (NavMeshSurface surface in surfaces)
		{
			surface.RemoveData();
		}

		foreach (NavMeshSurface surface in surfaces) {
			surface.BuildNavMesh();
		}
		//mapObjGen.meshWorldSize = meshSettings.MeshWorldSize;
	}


	private GameObject FindChildGameObject(GameObject parent, string terrainChunkName)
    {
		for (int i = 0; i < parent.transform.childCount; i++)
        {
			if (parent.transform.GetChild(i).name == terrainChunkName)
			{
				return parent.transform.GetChild(i).gameObject;
			}

			GameObject tmp = FindChildGameObject(parent.transform.GetChild(i).gameObject, terrainChunkName);

			if (tmp != null)
            {
				return tmp;
            }
        }
		return null;
    }

	[SerializeField] private RainControl rainControl;

	[SerializeField] private float staticFriction = 1000f;
	[SerializeField] private float dynamicFriction = 1000f;
	[SerializeField] private float bounciness = 0f;

	[SerializeField] private GridGraph gridGraph;
	[SerializeField] private NavMeshGraph navGraph;

	private Mesh mesh;

	void Start() {

		navGraph = AstarPath.active.data.navmesh;

		/*
		gridGraph = AstarPath.active.data.gridGraph;

		gridGraph.depth = (int)(meshSettings.meshWorldSize / 9);
		gridGraph.width = (int)(meshSettings.meshWorldSize / 9);
		*/

		textureSettings.ApplyToMaterial(mapMaterial);
		textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

		float maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
		meshWorldSize = meshSettings.meshWorldSize;

		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);

		UpdateVisibleChunks();

		GameObject tmp = FindChildGameObject(mapObject, terrainChunkName);

		if (tmp != null)
		{
			tmp.isStatic = true;
			tmp.tag = "Walkable";
			tmp.layer = 8; // 'Ground' Layer

			corruptionControl = tmp.AddComponent<CorruptionControl>();

			corruptionControl.player = player;
			corruptionControl.behaviours = behaviours;

			corruptionControl.CorruptionModifierActive = true;

			lerpTerrain = tmp.AddComponent<LerpTerrain>();
			rainControl.lerpTerrain = lerpTerrain;

			StartCoroutine(EnableContacts(tmp));

			MeshFilter meshFilter = tmp.GetComponentInChildren<MeshFilter>();

			Debug.Log("MESHFILTER:" + meshFilter);

			StartCoroutine(GetMesh(tmp));

			//Interactable interactable = tmp.AddComponent<Interactable>();
			//GameObject terrainObject = tmp;
			//mapObjGen.terrain = terrainObject;




			//interactable.name = "Plant";
			//interactable.options[0].title = "Plant";
			//interactable.options[0].color = Color.white;
			//Sprite spriteName = Resources.Load<Sprite>("Menu/Icons/PlantIcon");
			//interactable.options[0].sprite = spriteName;

			//tmp.AddComponent<ParticleCollision>();

		}

	}

	private IEnumerator GetMesh(GameObject tmp)
	{
		// Get the MeshFilter component from the desired GameObject
		MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();

		// Wait until the Mesh is not null
		yield return new WaitUntil(() => (meshFilter = tmp.GetComponentInChildren<MeshFilter>()) != null);

		// Set the sourceMesh of the NavmeshGraph to the Mesh
		navGraph.sourceMesh = meshFilter.mesh;

		// Rescan the NavmeshGraph to apply the changes
		AstarPath.active.Scan();
		mapObjectGen.GenerateMapObjects();
		yield break;
	}

public IEnumerator EnableContacts(GameObject tmp)
	{
		Collider collider = null;

		yield return new WaitUntil(() => (collider = tmp.GetComponentInChildren<Collider>()) != null);

		collider.providesContacts = false;

		// Add a physics material with high friction and bounciness
		var physicsMaterial = new PhysicMaterial();
		physicsMaterial.staticFriction = staticFriction;
		physicsMaterial.dynamicFriction = dynamicFriction;
		physicsMaterial.bounciness = bounciness;
		physicsMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
		collider.material = physicsMaterial;
		yield break;
	}

	void Update() {
		/*
		viewerPosition = new Vector2 (viewer.position.x, viewer.position.z);

		if (viewerPosition != viewerPositionOld) {
			foreach (TerrainChunk chunk in visibleTerrainChunks) {
				chunk.UpdateCollisionMesh ();
			}
		}

		if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate) {
			viewerPositionOld = viewerPosition;
			UpdateVisibleChunks ();
		}
		*/
	}
		
	void UpdateVisibleChunks() {

		HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2> ();
		for (int i = visibleTerrainChunks.Count-1; i >= 0; i--) {
			alreadyUpdatedChunkCoords.Add (visibleTerrainChunks [i].coord);
			visibleTerrainChunks [i].UpdateTerrainChunk ();
		}
			
		int currentChunkCoordX = Mathf.RoundToInt (viewerPosition.x / meshWorldSize);
		int currentChunkCoordY = Mathf.RoundToInt (viewerPosition.y / meshWorldSize);

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
				Vector2 viewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
				if (!alreadyUpdatedChunkCoords.Contains (viewedChunkCoord)) {
					if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) {
						terrainChunkDictionary [viewedChunkCoord].UpdateTerrainChunk ();
					} else {
						TerrainChunk newChunk = new TerrainChunk (viewedChunkCoord,heightMapSettings,meshSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial);
						terrainChunkDictionary.Add (viewedChunkCoord, newChunk);

						newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
						newChunk.Load ();
					}
				}

			}
		}
	}

	void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible) {
		if (isVisible) {
			visibleTerrainChunks.Add (chunk);
		} else {
			visibleTerrainChunks.Remove (chunk);
		}
	}

}

[System.Serializable]
public struct LODInfo {
	[Range(0,MeshSettings.numSupportedLODs-1)]
	public int lod;
	public float visibleDstThreshold;


	public float sqrVisibleDstThreshold {
		get {
			return visibleDstThreshold * visibleDstThreshold;
		}
	}
}
