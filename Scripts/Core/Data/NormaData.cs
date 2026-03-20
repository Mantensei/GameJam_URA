using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameJam_URA
{
    [Obsolete("SO基底。今後はTSV等からIUraTaskを生成すること")]
    [CreateAssetMenu(fileName = "NewNormaTable", menuName = "GameJam/NormaTable")]
    public class NormaTable : ScriptableObject, IUraTaskProvider
    {
        [SerializeField] ScriptableObject[] children;
        public void GetAllTasks(List<IUraTask> result)
        {
            if (children == null) return;
            var seen = new HashSet<string>();
            foreach (var provider in children.OfType<IUraTaskProvider>())
            {
                int before = result.Count;
                provider.GetAllTasks(result);
                for (int i = result.Count - 1; i >= before; i--)
                    if (!seen.Add(result[i].Name))
                        result.RemoveAt(i);
            }
        }
    }
}

#if UNITY_EDITOR
namespace GameJam_URA
{
    using UnityEditor;

    [CustomEditor(typeof(NormaTable))]
    public class NormaTableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            if (EditorGUI.EndChangeCheck())
                ValidateChildren();
            serializedObject.ApplyModifiedProperties();
        }

        void ValidateChildren()
        {
            var prop = serializedObject.FindProperty("children");
            var seen = new HashSet<UnityEngine.Object>();
            for (int i = 0; i < prop.arraySize; i++)
            {
                var obj = prop.GetArrayElementAtIndex(i).objectReferenceValue as ScriptableObject;
                if (obj == null) continue;

                if (!(obj is IUraTaskProvider))
                {
                    Debug.Log($"{obj.name} は IUraTaskProvider を実装していないため除外されました");
                    prop.GetArrayElementAtIndex(i).objectReferenceValue = null;
                }
                else if (obj is NormaTable data && WouldCauseCircle(data))
                {
                    Debug.Log($"{obj.name} は循環参照になるため除外されました");
                    prop.GetArrayElementAtIndex(i).objectReferenceValue = null;
                }
                else if (!seen.Add(obj))
                {
                    Debug.Log($"{obj.name} は重複しているため除外されました");
                    prop.GetArrayElementAtIndex(i).objectReferenceValue = null;
                }
            }
        }

        bool WouldCauseCircle(NormaTable candidate)
        {
            if (candidate == target) return true;
            var visiting = new HashSet<NormaTable> { (NormaTable)target };
            return DetectCycle(candidate, visiting);
        }

        static bool DetectCycle(NormaTable current, HashSet<NormaTable> visiting)
        {
            if (!visiting.Add(current)) return true;

            var so = new SerializedObject(current);
            var children = so.FindProperty("children");
            for (int i = 0; i < children.arraySize; i++)
            {
                var child = children.GetArrayElementAtIndex(i).objectReferenceValue as NormaTable;
                if (child != null && DetectCycle(child, visiting))
                    return true;
            }

            visiting.Remove(current);
            return false;
        }
    }
}
#endif
