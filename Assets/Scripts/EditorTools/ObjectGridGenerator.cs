using System;
using UnityEngine;

namespace Zombies.EditorTools
{
    /// <summary>Creates copies of one prefab in a regular 3D grid while editing the scene.</summary>
    public sealed class ObjectGridGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Vector3Int counts = Vector3Int.one;
        [SerializeField] private Vector3 spacing = Vector3.one;
        [SerializeField] private TransformOffset firstOffset = TransformOffset.Identity;
        [SerializeField] private TransformOffset secondOffset = TransformOffset.Identity;

        private void OnValidate()
        {
            counts.x = Mathf.Max(1, counts.x);
            counts.y = Mathf.Max(1, counts.y);
            counts.z = Mathf.Max(1, counts.z);

        }

#if UNITY_EDITOR
        public void Generate()
        {
            if (prefab == null)
            {
                Debug.LogWarning("Object Grid Generator: assign a prefab first.", this);
                return;
            }

            Clear();

            for (var y = 0; y < counts.y; y++)
            for (var z = 0; z < counts.z; z++)
            for (var x = 0; x < counts.x; x++)
            {
                var copy = UnityEditor.PrefabUtility.IsPartOfPrefabAsset(prefab)
                    ? UnityEditor.PrefabUtility.InstantiatePrefab(prefab, transform) as GameObject
                    : Instantiate(prefab, transform);

                if (copy == null)
                    continue;

                var offset = TransformOffset.RandomBetween(firstOffset, secondOffset);

                UnityEditor.Undo.RegisterCreatedObjectUndo(copy, "Generate prefab copies");
                copy.transform.localPosition = Vector3.Scale(new Vector3(x, y, z), spacing) + offset.position;
                copy.transform.localRotation = Quaternion.Euler(offset.rotation);
                copy.transform.localScale = Vector3.Scale(copy.transform.localScale, offset.scale);
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }

        public void Clear()
        {
            for (var index = transform.childCount - 1; index >= 0; index--)
                UnityEditor.Undo.DestroyObjectImmediate(transform.GetChild(index).gameObject);

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
    }

    [Serializable]
    public struct TransformOffset
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        public static TransformOffset Identity => new()
        {
            position = Vector3.zero,
            rotation = Vector3.zero,
            scale = Vector3.one
        };

        public static TransformOffset RandomBetween(TransformOffset first, TransformOffset second) => new()
        {
            position = RandomVector(first.position, second.position),
            rotation = RandomVector(first.rotation, second.rotation),
            scale = RandomVector(first.scale, second.scale)
        };

        private static Vector3 RandomVector(Vector3 first, Vector3 second) => new(
            UnityEngine.Random.Range(Mathf.Min(first.x, second.x), Mathf.Max(first.x, second.x)),
            UnityEngine.Random.Range(Mathf.Min(first.y, second.y), Mathf.Max(first.y, second.y)),
            UnityEngine.Random.Range(Mathf.Min(first.z, second.z), Mathf.Max(first.z, second.z)));
    }
}
