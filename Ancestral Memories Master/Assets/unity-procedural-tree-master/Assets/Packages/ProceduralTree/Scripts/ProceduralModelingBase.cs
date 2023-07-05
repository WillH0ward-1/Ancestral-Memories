﻿using System.Collections;
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
		MeshFilter filter;
		MeshCollider meshCollider;

		protected virtual void Start()
		{
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