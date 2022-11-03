﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class TerrainGenerator : MonoBehaviour {

	const float viewerMoveThresholdForChunkUpdate = 25f;
	const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;


	public int colliderLODIndex;
	public LODInfo[] detailLevels;

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureData textureSettings;

	public NavMeshSurface surface;

	//public NavMeshPrefabInstance navMeshSurface;

	public Transform viewer;
	public Material mapMaterial;

	Vector2 viewerPosition;
	Vector2 viewerPositionOld;

	public GameObject mapObject;

	float meshWorldSize;

	int chunksVisibleInViewDst;

	public Player player;
	public Camera interactCam;
	private string terrainChunkName = "Terrain Chunk";
	public Interactable interactable;
	public RadialMenu radialMenu;

	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

	private void Awake()
	{
		surface.BuildNavMesh();
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

	void Start() {

		textureSettings.ApplyToMaterial(mapMaterial);
		textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

		float maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
		meshWorldSize = meshSettings.meshWorldSize;

		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);

		UpdateVisibleChunks();

		GameObject tmp = FindChildGameObject(mapObject, terrainChunkName);
		if (tmp != null)
		{
			tmp.tag = "Walkable";
			tmp.layer = 8; // 'Ground' Layer
			tmp.AddComponent<CorruptionControl>();

		    interactable = tmp.AddComponent<Interactable>();

		}
	}

	void Update() {

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
