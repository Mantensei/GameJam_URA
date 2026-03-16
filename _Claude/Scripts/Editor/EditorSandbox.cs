#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GameJam_URA.Prototype
{
    public class EditorSandbox
    {
        [MenuItem("GameJam/Run Sandbox")]
        static void Run()
        {
            ReplaceTextWithTMP();
        }

        static void ReplaceTextWithTMP()
        {
            var texts = Object.FindObjectsByType<Text>(FindObjectsSortMode.None);
            Debug.Log("Found " + texts.Length + " Text components to replace");

            foreach (var oldText in texts)
            {
                var go = oldText.gameObject;
                string content = oldText.text;
                int fontSize = oldText.fontSize;
                Color color = oldText.color;
                TextAnchor anchor = oldText.alignment;

                Undo.DestroyObjectImmediate(oldText);

                var tmp = Undo.AddComponent<TextMeshProUGUI>(go);
                tmp.text = content;
                tmp.fontSize = fontSize;
                tmp.color = color;
                tmp.alignment = ConvertAlignment(anchor);

                EditorUtility.SetDirty(go);
            }

            Debug.Log("Replaced " + texts.Length + " Text → TextMeshProUGUI");
        }

        static TextAlignmentOptions ConvertAlignment(TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.UpperLeft: return TextAlignmentOptions.TopLeft;
                case TextAnchor.UpperCenter: return TextAlignmentOptions.Top;
                case TextAnchor.UpperRight: return TextAlignmentOptions.TopRight;
                case TextAnchor.MiddleLeft: return TextAlignmentOptions.MidlineLeft;
                case TextAnchor.MiddleCenter: return TextAlignmentOptions.Center;
                case TextAnchor.MiddleRight: return TextAlignmentOptions.MidlineRight;
                case TextAnchor.LowerLeft: return TextAlignmentOptions.BottomLeft;
                case TextAnchor.LowerCenter: return TextAlignmentOptions.Bottom;
                case TextAnchor.LowerRight: return TextAlignmentOptions.BottomRight;
                default: return TextAlignmentOptions.Center;
            }
        }
    }
}
#endif
