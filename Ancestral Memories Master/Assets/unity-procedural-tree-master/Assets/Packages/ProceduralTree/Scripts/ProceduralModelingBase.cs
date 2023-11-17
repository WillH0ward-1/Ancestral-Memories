using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralModeling
{

	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	[ExecuteInEditMode]
	public abstract class ProceduralModelingBase : MonoBehaviour
	{

		public MeshFilter Filter
		{
			get
			{
				if (filter == null)
				{
					filter = GetComponent<MeshFilter>();
				}
				return filter;
			}
		}

		public MeshCollider MeshCollider
        {
            get
            {
				if(meshCollider == null)
                {
					meshCollider = GetComponent<MeshCollider>();
                }
				return meshCollider;
            }
        }

		public TerrainGenerator TerrainGenerator
        {
            get
            {
				if (terrainGenerator == null)
                {
                    terrainGenerator = FindObjectOfType<TerrainGenerator>();
                }
                return terrainGenerator;
            }
        }

		MeshFilter filter;
		MeshCollider meshCollider;
        TerrainGenerator terrainGenerator;

        protected virtual void Start()
		{
			TerrainGenerator terrainGenerator = FindObjectOfType<TerrainGenerator>();
			Rebuild();
		}

        public void Rebuild()
        {
            if (Filter.sharedMesh != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(Filter.sharedMesh);
                }
                else
                {
                    DestroyImmediate(Filter.sharedMesh);
                }
            }

            Filter.sharedMesh = Build();
        }


        protected abstract Mesh Build();
		public abstract void ClearLeaves();
	}

}