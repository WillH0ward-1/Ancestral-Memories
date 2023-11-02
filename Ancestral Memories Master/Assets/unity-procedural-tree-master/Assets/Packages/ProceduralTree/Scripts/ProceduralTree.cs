using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace ProceduralModeling
{

	public class ProceduralTree : ProceduralModelingBase
	{

		public TreeData Data { get { return data; } }

		[SerializeField] TreeData data;
		[SerializeField, Range(2, 8)] protected int generations = 5;
		[SerializeField, Range(0.5f, 5f)] protected float length = 1f;
		[SerializeField, Range(0.1f, 2f)] protected float radius = 0.15f;
		[SerializeField, Range(0f, 10f)] protected float leafSize = 0f;

		public Material leafMat;
		const float PI2 = Mathf.PI * 2f;

		private bool isLeafListInitialized;


		private LeafScaler leafScaler;
		private Transform leafRoot;

		private TreeFruitManager treeFruitManager;

		private bool generateMeshCollider = false;

		void OnEnable()
		{
			isLeafListInitialized = false;
			leafScaler = GetComponent<LeafScaler>();
			treeFruitManager = GetComponent<TreeFruitManager>();
			leafScaler.proceduralTree = this;
			
			if (leafScaler == null)
			{
				Debug.LogError("LeafScaler component not found on this GameObject.");
			}

			leafRoot = transform.Find("LeafRoot");
		}

		void OnDisable()
		{
			ClearLeaves();
		}

		public class SegmentMeshData
		{
			public List<Vector3> vertices = new List<Vector3>();
			public List<int> triangles = new List<int>();
			public List<Vector3> normals = new List<Vector3>();
			public List<Vector4> tangents = new List<Vector4>();
			public List<Vector2> uvs = new List<Vector2>();
			// Add other necessary data like materials, etc.
		}

		public static (Mesh, List<SegmentMeshData>) Build(ProceduralTree treeInstance, TreeData data, int generations, float length, float radius, float leafSize, Material leafMat)
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

			List<SegmentMeshData> segmentMeshes = new List<SegmentMeshData>();

			Traverse(root, (branch) =>
			{
				SegmentMeshData segmentMesh = new SegmentMeshData();

				var offset = vertices.Count;
				var segOffset = segmentMesh.vertices.Count;
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
						var u = 1f * j / data.radialSegments;
						float rad = u * PI2;

						float cos = Mathf.Cos(rad), sin = Mathf.Sin(rad);
						var normal = (cos * N + sin * B).normalized;
						var position = segment.Position + segment.Radius * normal;

						var pos = segment.Position + segment.Radius * normal;

						vertices.Add(pos);
						normals.Add(normal);

						var tangent = segment.Frame.Tangent;
						Vector4 tangentVals = new Vector4(tangent.x, tangent.y, tangent.z, 0f);

						tangents.Add(tangentVals);

						Vector2 uv = new Vector2(u, v);
						uvs.Add(uv);

						// Add the same data to segmentMesh
						segmentMesh.vertices.Add(pos);
						segmentMesh.normals.Add(normal);
						segmentMesh.tangents.Add(tangentVals);
						segmentMesh.uvs.Add(uv);
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

						// Indices for the segment mesh (start from 0)
						int segA = (data.radialSegments + 1) * (j - 1) + (i - 1);
						int segB = (data.radialSegments + 1) * j + (i - 1);
						int segC = (data.radialSegments + 1) * j + i;
						int segD = (data.radialSegments + 1) * (j - 1) + i;

						segA += segOffset;
						segB += segOffset;
						segC += segOffset;
						segD += segOffset;


						segmentMesh.triangles.Add(segA); segmentMesh.triangles.Add(segD); segmentMesh.triangles.Add(segB);
						segmentMesh.triangles.Add(segB); segmentMesh.triangles.Add(segD); segmentMesh.triangles.Add(segC);
					}
				}

				segmentMeshes.Add(segmentMesh);
			});

			var mesh = new Mesh();
			mesh.vertices = vertices.ToArray();
			mesh.normals = normals.ToArray();
			mesh.tangents = tangents.ToArray();
			mesh.uv = uvs.ToArray();
			mesh.triangles = triangles.ToArray();

			return (mesh, segmentMeshes);
		}

		public List<GameObject> segmentObjects = new List<GameObject>();
		public float depthFactor = 1.0f;

		public void CreateSegmentedTreeObjects(List<SegmentMeshData> segmentMeshes, Material material, float depthFactor)
		{
			ClearSegmentObjects();

			int totalSegments = segmentMeshes.Count;
			int segmentsPerGroup = Mathf.Max(1, Mathf.FloorToInt(totalSegments * depthFactor));

			for (int i = 0; i < totalSegments; i += segmentsPerGroup)
			{
				SegmentMeshData combinedSegmentMesh = new SegmentMeshData();

				// Combine the data of 'segmentsPerGroup' consecutive segments
				for (int j = i; j < Mathf.Min(i + segmentsPerGroup, totalSegments); j++)
				{
					var segmentMesh = segmentMeshes[j];
					int vertexOffset = combinedSegmentMesh.vertices.Count;

					combinedSegmentMesh.vertices.AddRange(segmentMesh.vertices);
					combinedSegmentMesh.normals.AddRange(segmentMesh.normals);
					combinedSegmentMesh.tangents.AddRange(segmentMesh.tangents);
					combinedSegmentMesh.uvs.AddRange(segmentMesh.uvs);

					// Add the triangles with the correct offset
					foreach (var tri in segmentMesh.triangles)
					{
						combinedSegmentMesh.triangles.Add(tri + vertexOffset);
					}
				}

				// Now create a GameObject for this combined segment
				string objName = (i + segmentsPerGroup >= totalSegments) ? "TreeRoot" : $"TreeSegment_{i / segmentsPerGroup + 1}";
				GameObject segmentObj = new GameObject(objName);

				// Adding components to the GameObject
				var meshFilter = segmentObj.AddComponent<MeshFilter>();
				var meshRenderer = segmentObj.AddComponent<MeshRenderer>();
				ShaderLightColor lighting = segmentObj.AddComponent<ShaderLightColor>();
				lighting.enabled = true;
				segmentObj.isStatic = true;

				// Assign material
				meshRenderer.material = material;

				// Create and assign mesh to the MeshFilter
				Mesh segmentedMesh = new Mesh();
				segmentedMesh.vertices = combinedSegmentMesh.vertices.ToArray();
				segmentedMesh.normals = combinedSegmentMesh.normals.ToArray();
				segmentedMesh.tangents = combinedSegmentMesh.tangents.ToArray();
				segmentedMesh.uv = combinedSegmentMesh.uvs.ToArray();
				segmentedMesh.triangles = combinedSegmentMesh.triangles.ToArray();
				meshFilter.mesh = segmentedMesh;

				// Parent it under a common root for organization
				segmentObj.transform.SetParent(transform); // Assuming 'this' is the main tree object
				segmentObj.transform.localPosition = Vector3.zero;
				segmentObj.transform.localRotation = Quaternion.identity;
				segmentObj.transform.localScale = Vector3.one;

				// Add the new GameObject to the list
				segmentObjects.Add(segmentObj);
			}
		}


		public void ClearSegmentObjects()
		{
			foreach (var obj in segmentObjects)
			{
				if (obj != null)
				{
					if (Application.isPlaying)
					{
						Destroy(obj);
					}
					else
					{
						DestroyImmediate(obj);
					}
				}
			}

			segmentObjects.Clear(); // Clear the list after destroying the objects
		}

		private int lastPhysicsEnabledIndex = -1; // Store the index up to which physics has been enabled
		public LayerMask physicsLayers;

		public void EnablePhysicsInBurstMode()
		{
			// Ensuring that the next range doesn't overlap with already physics-enabled segments
			int start = lastPhysicsEnabledIndex + 1;

			// Random range - you can customize the limits as per your requirements
			int range = UnityEngine.Random.Range(5, 16);  // Example: Enables physics on a random number of segments between 5 and 10

			// Apply physics in the selected range
			for (int i = start; i < start + range && i < segmentObjects.Count; i++)
			{
				// Skip the TreeRoot
				if (segmentObjects[i].name != "TreeRoot")
				{
					AssignPhysics(segmentObjects[i]);
				}

				lastPhysicsEnabledIndex = i;
			}
		}


		public void AssignPhysicsToAllSegments()
		{
			foreach (var segment in segmentObjects)
			{
				AssignPhysics(segment);
			}
		}


		public void AssignPhysics(GameObject segment)
		{
			if (segment == null)
			{
				Debug.LogError("Segment object is null. Cannot add physics components.");
				return;
			}

			if (segment.name != "TreeRoot")
			{

				// Add and configure Rigidbody
				Rigidbody rb = segment.AddComponent<Rigidbody>();
				rb.useGravity = true;
				rb.mass = 100f;

				// Add MeshCollider
				MeshCollider collider = segment.AddComponent<MeshCollider>();
				collider.convex = true; // You can adjust this based on your requirement
				collider.includeLayers = physicsLayers;
				// Using the existing mesh for the collider
				MeshFilter meshFilter = segment.GetComponent<MeshFilter>();
				if (meshFilter != null && meshFilter.sharedMesh != null)
				{
					collider.sharedMesh = meshFilter.sharedMesh;
				}
				else
				{
					Debug.LogWarning("Segment does not have a MeshFilter with a mesh assigned. MeshCollider may not function correctly.");
				}
			}
		}

		protected override Mesh Build()
		{
			var (treeMesh, segmentMeshes) = Build(this, data, generations, length, radius, leafSize, leafMat);
			// Optionally store segmentMeshes in the class for later use
			Renderer renderer = transform.GetComponent<Renderer>();
			Material material = renderer.sharedMaterial;
			CreateSegmentedTreeObjects(segmentMeshes, material, depthFactor);

			//HideMesh();
			return treeMesh;
		}

		private void HideMesh()
		{
			// To hide
			this.GetComponent<MeshRenderer>().enabled = false;
		}


		static float TraverseMaxLength(TreeBranch branch)
		{
			float max = 0f;
			branch.Children.ForEach(c =>
			{
				max = Mathf.Max(max, TraverseMaxLength(c));
			});
			return branch.Length + max;
		}


		static void Traverse(TreeBranch from, Action<TreeBranch> action)
		{
			if (from.Children.Count > 0)
			{
				from.Children.ForEach(child =>
				{
					Traverse(child, action);
				});
			}
			action(from);
		}

		void GenerateMeshCollider()
        {
			if (generateMeshCollider)
			{
				if (MeshCollider.sharedMesh != null)
				{
					if (Application.isPlaying)
					{
						Destroy(MeshCollider.sharedMesh);
					}
					else
					{
						DestroyImmediate(MeshCollider.sharedMesh);
					}
				}

				MeshCollider.sharedMesh = Build();
			}
		}

		public List<GameObject> leafList;

		public Mesh LeafMesh { get; private set; }
		public List<SegmentMeshData> segmentMeshes; // Storing segment meshes

		public Material LeafMaterial { get; private set; }
		private int maxInstances;

		[SerializeField] private LinkedList<Matrix4x4> matrixList = new LinkedList<Matrix4x4>();

		[SerializeField]
		[ArraySize(0)] // This attribute ensures the array size is displayed correctly in the inspector
		private Matrix4x4[] matrices;

		public Matrix4x4[] Matrices
		{
			get { return matrices; }
			private set { matrices = value; }
		}

		[AttributeUsage(AttributeTargets.Field)]
		public class ArraySizeAttribute : PropertyAttribute
		{
			public int Size { get; private set; }

			public ArraySizeAttribute(int size)
			{
				Size = size;
			}
		}

		public List<Quaternion> LeafRotations = new List<Quaternion>();

		void GenerateLeaves(TreeBranch root, Material leafMat)
		{
			ClearLeaves();

			Traverse(root, (branch) =>
			{
				if (branch.Children.Count == 0) // If branch has no children, it's an end branch.
				{
					foreach (var segment in branch.Segments)
					{
						var lastSegment = branch.Segments.Last();

						// Translate position to world space
						var position = transform.TransformPoint(lastSegment.Position);

						Quaternion rotation = UnityEngine.Random.rotation;

						Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, Vector3.zero);
						matrixList.AddLast(matrix);

						LeafRotations.Add(rotation);
					}
				}
			});

			maxInstances = matrixList.Count;

			if (maxInstances > 0)
			{
				Matrices = new Matrix4x4[maxInstances];
				int index = 0;
				foreach (var matrix in matrixList)
				{
					Matrices[index] = matrix;
					index++;
				}
			}

#if UNITY_EDITOR

			LeafMesh = GameObject.CreatePrimitive(PrimitiveType.Quad).GetComponent<MeshFilter>().mesh;  
#else
			LeafMesh = GameObject.CreatePrimitive(PrimitiveType.Quad).GetComponent<MeshFilter>().sharedMesh;  // Replace with your actual leaf mesh
#endif
			LeafMaterial = leafMat;
			LeafMaterial.enableInstancing = true;  // Enable GPU instancing for the material

			leafScaler.RecordOriginalMatrices();
		}

		public List<Vector3> FruitPoints = new List<Vector3>();

		private void GenerateFruitPoints(TreeBranch root)
		{
			FruitPoints.Clear();

			Traverse(root, (branch) =>
			{
				if (branch.Children.Count == 0) // If branch has no children, it's an end branch.
				{
					foreach (var segment in branch.Segments)
					{
						var lastSegment = branch.Segments.Last();

						// Translate position to world space
						var position = transform.TransformPoint(lastSegment.Position);

						FruitPoints.Add(position);
					}
				}
			});

			int fruitCount = Mathf.Min(treeFruitManager.maxFruits, FruitPoints.Count);

			treeFruitManager.InitializeFruits(fruitCount);
		}

		void OnRenderObject()
		{
			if (LeafMesh != null && LeafMaterial != null && Matrices != null)
			{
				Graphics.DrawMeshInstanced(LeafMesh, 0, LeafMaterial, Matrices, Mathf.Min(maxInstances, 1023));
			}
		}


		public override void ClearLeaves()
		{
			matrixList.Clear();

			// Reset the initialization flag
			isLeafListInitialized = false;
		}
	}


[System.Serializable]
public class TreeData
{
    public int randomSeed = 0;
    [Range(0.25f, 0.95f)] public float lengthAttenuation = 0.8f, radiusAttenuation = 0.5f;
    [Range(1, 3)] public int branchesMin = 1, branchesMax = 3;
    [Range(-45f, 0f)] public float growthAngleMin = -15f;
    [Range(0f, 45f)] public float growthAngleMax = 15f;
    [Range(1f, 10f)] public float growthAngleScale = 4f;
    [Range(0f, 45f)] public float branchingAngle = 15f;
    [Range(4, 20)] public int heightSegments = 10, radialSegments = 8;
    [Range(0.0f, 0.35f)] public float bendDegree = 0.1f;

    Rand rnd;

    public void Setup()
    {
        randomSeed = UnityEngine.Random.Range(0, int.MaxValue);
        rnd = new Rand(randomSeed);
    }

    public void Grow()
    {
        randomSeed++;
        rnd = new Rand(randomSeed);
    }

    public void Shrink()
    {
        randomSeed--;
        rnd = new Rand(randomSeed);
    }

    public int Range(int a, int b)
    {
        return rnd.Range(a, b);
    }

    public float Range(float a, float b)
    {
        return rnd.Range(a, b);
    }

    public int GetRandomBranches()
    {
        return rnd.Range(branchesMin, branchesMax + 1);
    }

    public float GetRandomGrowthAngle()
    {
        return rnd.Range(growthAngleMin, growthAngleMax);
    }

    public float GetRandomBendDegree()
    {
        return rnd.Range(-bendDegree, bendDegree);
    }
}


	public class TreeBranch
	{
		public int Generation { get { return generation; } }
		public List<TreeSegment> Segments { get { return segments; } }
		public List<TreeBranch> Children { get { return children; } }

		public Vector3 From { get { return from; } }
		public Vector3 To { get { return to; } }
		public float Length { get { return length; } }
		public float Offset { get { return offset; } }

		int generation;

		List<TreeSegment> segments;
		List<TreeBranch> children;

		Vector3 from, to;
		float fromRadius, toRadius;
		float length;
		float offset;

		// for Root branch constructor
		public TreeBranch(int generation, float length, float radius, TreeData data, Material leafMat) : this(new List<TreeBranch>(), generation, generation, Vector3.zero, Vector3.up, Vector3.right, Vector3.back, length, radius, 0f, data, leafMat)
		{
		}

		protected TreeBranch(List<TreeBranch> branches, int generation, int generations, Vector3 from, Vector3 tangent, Vector3 normal, Vector3 binormal, float length, float radius, float offset, TreeData data, Material leafMat)
		{
			this.generation = generation;

			this.fromRadius = radius;
			this.toRadius = (generation == 0) ? 0f : radius * data.radiusAttenuation;

			this.from = from;

			var scale = Mathf.Lerp(1f, data.growthAngleScale, 1f - 1f * generation / generations);
			var rotation = Quaternion.AngleAxis(scale * data.GetRandomGrowthAngle(), normal) * Quaternion.AngleAxis(scale * data.GetRandomGrowthAngle(), binormal);
			this.to = from + rotation * tangent * length;

			this.length = length;
			this.offset = offset;

			segments = BuildSegments(data, fromRadius, toRadius, normal, binormal);

			branches.Add(this);

			children = new List<TreeBranch>();
			if (generation > 0)
			{
				int count = data.GetRandomBranches();
				for (int i = 0; i < count; i++)
				{
					float ratio;
					if (count == 1)
					{
						// for zero division
						ratio = 1f;
					}
					else
					{
						ratio = Mathf.Lerp(0.5f, 1f, (1f * i) / (count - 1));
					}

					var index = Mathf.FloorToInt(ratio * (segments.Count - 1));
					var segment = segments[index];

					Vector3 nt, nn, nb;
					if (ratio >= 1f)
					{
						// sequence branch
						nt = segment.Frame.Tangent;
						nn = segment.Frame.Normal;
						nb = segment.Frame.Binormal;
					}
					else
					{
						var phi = Quaternion.AngleAxis(i * 90f, tangent);
						// var psi = Quaternion.AngleAxis(data.branchingAngle, normal) * Quaternion.AngleAxis(data.branchingAngle, binormal);
						var psi = Quaternion.AngleAxis(data.branchingAngle, normal);
						var rot = phi * psi;
						nt = rot * tangent;
						nn = rot * normal;
						nb = rot * binormal;
					}

					var child = new TreeBranch(
						branches,
						this.generation - 1,
						generations,
						segment.Position,
						nt,
						nn,
						nb,
						length * Mathf.Lerp(1f, data.lengthAttenuation, ratio),
						radius * Mathf.Lerp(1f, data.radiusAttenuation, ratio),
						offset + length,
						data,
						leafMat
					);

					children.Add(child);
				}
			}
		}

		List<TreeSegment> BuildSegments(TreeData data, float fromRadius, float toRadius, Vector3 normal, Vector3 binormal)
		{
			var segments = new List<TreeSegment>();

			var points = new List<Vector3>();

			var length = (to - from).magnitude;
			var bend = length * (normal * data.GetRandomBendDegree() + binormal * data.GetRandomBendDegree());
			points.Add(from);
			points.Add(Vector3.Lerp(from, to, 0.25f) + bend);
			points.Add(Vector3.Lerp(from, to, 0.75f) + bend);
			points.Add(to);

			var curve = new CatmullRomCurve(points);

			var frames = curve.ComputeFrenetFrames(data.heightSegments, normal, binormal, false);
			for (int i = 0, n = frames.Count; i < n; i++)
			{
				var u = 1f * i / (n - 1);
				var radius = Mathf.Lerp(fromRadius, toRadius, u);

				var position = curve.GetPointAt(u);
				var segment = new TreeSegment(frames[i], position, radius);
				segments.Add(segment);
			}
			return segments;
		}

	}

	public class TreeSegment
	{
		public FrenetFrame Frame { get { return frame; } }
		public Vector3 Position { get { return position; } }
		public float Radius { get { return radius; } }

		FrenetFrame frame;
		Vector3 position;
		float radius;

		public TreeSegment(FrenetFrame frame, Vector3 position, float radius)
		{
			this.frame = frame;
			this.position = position;
			this.radius = radius;
		}
	}

	public class Rand
	{
		System.Random rnd;

		public float value
		{
			get
			{
				return (float)rnd.NextDouble();
			}
		}

		public Rand(int seed)
		{
			rnd = new System.Random(seed);
		}

		public int Range(int a, int b)
		{
			var v = value;
			return Mathf.FloorToInt(Mathf.Lerp(a, b, v));
		}

		public float Range(float a, float b)
		{
			var v = value;
			return Mathf.Lerp(a, b, v);
		}
	}
}

