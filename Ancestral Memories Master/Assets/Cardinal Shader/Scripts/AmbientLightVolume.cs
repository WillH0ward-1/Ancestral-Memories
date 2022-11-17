using UnityEngine;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.LowLevel;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
#endif

//using MB;

namespace SWITCH
{
    [ExecuteAlways]
	public class AmbientLightVolume : MonoBehaviour
	{
        [field: SerializeField]
        public int Priority { get; private set; } = 0;

        [field: Space]

        [field: SerializeField, ColorUsage(true, true)]
        public Color Color { get; set; } = Color.white;

        [field: SerializeField, Range(0, 1)]
        public float Strength { get; set; } = 1;

        [field: Space]

        [field: SerializeField]
        public Vector3 Size { get; set; } = Vector3.one * 10;

        [field: SerializeField]
        Vector3 Padding { get; set; } = Vector3.right * 2;

        public Vector3 Center => transform.position;

        public Bounds Bounds => new Bounds(Center, Size);

        public AmbientVolumeData Data
        {
            get
            {
                var matrix = transform.worldToLocalMatrix;

                return new AmbientVolumeData(matrix, Size, Padding, Color * Strength);
            }
        }

        void OnValidate()
        {
            Padding = Clamp(Padding);
            Size = Clamp(Size);

            static Vector3 Clamp(Vector3 vector)
            {
                vector.x = Mathf.Max(vector.x, 0f);
                vector.y = Mathf.Max(vector.y, 0f);
                vector.z = Mathf.Max(vector.z, 0f);

                return vector;
            }
        }

        void OnEnable()
        {
            Collection.Add(this);
        }
        void OnDisable()
        {
            Collection.Remove(this);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, Size);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Vector3.zero, Size + Padding);
        }

        public static class Collection
        {
            public static List<AmbientLightVolume> List;

            public static GenericComparer<AmbientLightVolume> PriorityComparer;

            public static void Add(AmbientLightVolume volume)
            {
                List.Add(volume);

                Sort();
            }

            public static void Sort()
            {
                List.Sort(PriorityComparer);
            }

            public static bool Remove(AmbientLightVolume volume)
            {
                return List.Remove(volume);
            }

            static Collection()
            {
                List = new();

                PriorityComparer = new GenericComparer<AmbientLightVolume>((x, y) =>
                {
                    var px = x.Priority;
                    var py = y.Priority;

                    return px.CompareTo(py);
                });
            }
        }

        public static class Lighting
        {
            static ComputeBuffer Buffer;

            public static class Fields
            {
                public readonly static int Buffer = Shader.PropertyToID("MB_AmbientLightVolumeBuffer");
                public readonly static int Count = Shader.PropertyToID("MB_AmbientLightVolumeCount");
            }

            public const int DefaultBufferCapacity = 10;

            static ComputeBuffer CreateBuffer(int count)
            {
                return new ComputeBuffer(count, AmbientVolumeData.BinarySize, ComputeBufferType.Default, ComputeBufferMode.SubUpdates);
            }

#if UNITY_EDITOR
            [InitializeOnLoadMethod]
#else
            [RuntimeInitializeOnLoadMethod]
#endif
            static void OnLoad()
            {
                Buffer = CreateBuffer(DefaultBufferCapacity);

                PlayerLoopUtility.Register<Update>(Update, false);

#if UNITY_EDITOR
                AssemblyReloadEvents.beforeAssemblyReload += Reset;
#else
                Application.quitting += Reset;
#endif
            }

            static void Update()
            {
#if UNITY_EDITOR
                Collection.Sort();
#endif

                Upload(Collection.List);
            }

            static void Upload(IList<AmbientLightVolume> volumes)
            {
                if (volumes.Count > Buffer.count)
                {
                    Buffer.Release();
                    Buffer = CreateBuffer(volumes.Count);
                }

                var destination = Buffer.BeginWrite<AmbientVolumeData>(0, volumes.Count);

                for (int i = 0; i < volumes.Count; i++)
                    destination[i] = volumes[i].Data;

                Buffer.EndWrite<AmbientVolumeData>(volumes.Count);

                Shader.SetGlobalBuffer(Fields.Buffer, Buffer);
                Shader.SetGlobalInteger(Fields.Count, volumes.Count);
            }

            static void Reset()
            {
                Buffer.Release();
            }
        }

#if UNITY_EDITOR
        [CanEditMultipleObjects]
        [CustomEditor(typeof(AmbientLightVolume))]
        class Inspector : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawVolumeTool();

                GUILayout.Space(5);

                base.OnInspectorGUI();
            }

            void DrawVolumeTool()
            {
                var content = EditorGUIUtility.TrTempContent("Edit Volume");
                EditorGUILayout.EditorToolbarForTarget(content, this);
            }
        }

        [EditorTool("Edit Volume", typeof(AmbientLightVolume))]
        class AmbientVolumeHandleTool : EditorTool
        {
            GUIContent toolbarIcon_cache;
            public override GUIContent toolbarIcon
            {
                get
                {
                    if (toolbarIcon_cache == null)
                    {
                        var image = EditorGUIUtility.IconContent("EditCollider").image;
                        const string tooltip = "Edit bounding volume.\n\n - Hold Alt after clicking control handle to pin center in place.\n - Hold Shift after clicking control handle to scale uniformly.";

                        toolbarIcon_cache = new GUIContent(image, tooltip);
                    }

                    return toolbarIcon_cache;
                }
            }

            readonly BoxBoundsHandle handle = new BoxBoundsHandle();

            public override void OnActivated()
            {
                base.OnActivated();

                handle.midpointHandleSizeFunction = ((x) => BoxBoundsHandle.DefaultMidpointHandleSizeFunction(x) * 2);
            }

            protected void CopyVolumePropertiesToHandle(AmbientLightVolume volume)
            {
                handle.center = Vector3.zero;
                handle.size = volume.Size;
            }
            protected void CopyHandlePropertiesToVolume(AmbientLightVolume volume)
            {
                volume.Size = handle.size;
                volume.transform.position = volume.transform.TransformPoint(handle.center);
                handle.center = Vector3.zero;
            }

            public override void OnToolGUI(EditorWindow window)
            {
                foreach (AmbientLightVolume target in base.targets)
                {
                    if (target == null)
                        continue;

                    if (Mathf.Approximately(target.transform.lossyScale.sqrMagnitude, 0f))
                        continue;

                    using (new Handles.DrawingScope(target.transform.localToWorldMatrix))
                    {
                        CopyVolumePropertiesToHandle(target);

                        handle.SetColor(Color.green);

                        EditorGUI.BeginChangeCheck();
                        handle.DrawHandle();
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObjects(new Object[] { target, target.transform }, $"Modify Ambient Volume");

                            CopyHandlePropertiesToVolume(target);
                        }
                    }
                }
            }
        }
#endif

        public abstract class PlayerLoopUtility
        {
            public static void Register<TType>(PlayerLoopSystem.UpdateFunction callback, bool autoRemove = true)
            {
                var loop = PlayerLoop.GetCurrentPlayerLoop();

                var index = Locate<TType>(ref loop);

                if (index == -1)
                    throw new Exception($"No PlayerLoop Entry Found for {typeof(TType)}");

                loop.subSystemList[index].updateDelegate += callback;

                PlayerLoop.SetPlayerLoop(loop);

                if (autoRemove) Application.quitting += () => Unregister<TType>(callback);
            }

            public static int Locate<TType>(ref PlayerLoopSystem loop)
            {
                for (int i = 0; i < loop.subSystemList.Length; ++i)
                    if (loop.subSystemList[i].type == typeof(TType))
                        return i;

                return -1;
            }

            public static bool Unregister<TType>(PlayerLoopSystem.UpdateFunction callback)
            {
                var loop = PlayerLoop.GetCurrentPlayerLoop();

                var index = Locate<TType>(ref loop);

                if (index == -1) return false;

                loop.subSystemList[index].updateDelegate -= callback;

                PlayerLoop.SetPlayerLoop(loop);
                return true;
            }
        }

        public class GenericComparer<T> : Comparer<T>
        {
            public MethodDelegate Method { get; private set; }
            public delegate int MethodDelegate(T x, T y);

            public override int Compare(T x, T y) => Method(x, y);

            public GenericComparer(MethodDelegate method)
            {
                this.Method = method;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AmbientVolumeData
    {
        public Matrix4x4 Matrix;
        public Vector3 Size;
        public Vector3 Padding;

        public Color Color;

        public readonly static int BinarySize = Marshal.SizeOf<AmbientVolumeData>();

        public AmbientVolumeData(Matrix4x4 matrix, Vector3 size, Vector3 padding, Color color)
        {
            this.Matrix = matrix;
            this.Size = size;
            this.Padding = padding;
            this.Color = color;
        }
    }
}