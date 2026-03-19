using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameJam_URA
{
    [CreateAssetMenu(fileName = "NewNormaData", menuName = "GameJam/NormaData")]
    public class NormaData : ScriptableObject, INormaProvider
    {
        //入れ子にしたかったが難しいので、広めの参照をもってカスタムエディターで整える
        [SerializeField] ScriptableObject[] children;
        public void GetAllNormas(List<Norma> result)
        {
            if (children == null) return;
            var seen = new HashSet<string>();
            foreach (var norma in children.OfType<INormaProvider>())
            {
                int before = result.Count;
                norma.GetAllNormas(result);
                for (int i = result.Count - 1; i >= before; i--)
                    if (!seen.Add(result[i].name))
                        result.RemoveAt(i);
            }
        }
    }
}

#if UNITY_EDITOR
namespace GameJam_URA
{
    using UnityEditor;

    [CustomEditor(typeof(NormaData))]
    public class NormaDataEditor : Editor
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
            var seen = new HashSet<Object>();
            for (int i = 0; i < prop.arraySize; i++)
            {
                var obj = prop.GetArrayElementAtIndex(i).objectReferenceValue as ScriptableObject;
                if (obj == null) continue;

                if (!(obj is INormaProvider))
                {
                    Debug.Log($"{obj.name} は INormaProvider を実装していないため除外されました");
                    prop.GetArrayElementAtIndex(i).objectReferenceValue = null;
                }
                else if (obj is NormaData data && WouldCauseCircle(data))
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

        bool WouldCauseCircle(NormaData candidate)
        {
            if (candidate == target) return true;
            var visiting = new HashSet<NormaData> { (NormaData)target };
            return DetectCycle(candidate, visiting);
        }

        static bool DetectCycle(NormaData current, HashSet<NormaData> visiting)
        {
            if (!visiting.Add(current)) return true;

            var so = new SerializedObject(current);
            var children = so.FindProperty("children");
            for (int i = 0; i < children.arraySize; i++)
            {
                var child = children.GetArrayElementAtIndex(i).objectReferenceValue as NormaData;
                if (child != null && DetectCycle(child, visiting))
                    return true;
            }

            visiting.Remove(current);
            return false;
        }
    }
}
#endif
