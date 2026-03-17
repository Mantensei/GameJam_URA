#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameJam_URA.Prototype
{
    //実行を確認したら関数の中のコードは消す。
    //ファイルを更新したらMenuItemに現在の時刻を入れる

    public class EditorSandbox
    {
        const string BasePath = "Assets/_GameJam_URA/_Claude/Sandbox/UIToolkitTest";
        const string ScenePath = BasePath + "/UIToolkitTestScene.unity";
        const string PanelSettingsPath = BasePath + "/UIToolkitPanelSettings.asset";
        const string GameHUDPath = BasePath + "/UI/GameHUD.uxml";

        [MenuItem("GameJam/Run Sandbox (03-17 15:39)")]
        static void Run()
        {
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            var gameHUD = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(GameHUDPath);

            if (panelSettings == null)
            {
                Debug.LogError("PanelSettingsが見つかりません: " + PanelSettingsPath);
                return;
            }
            if (gameHUD == null)
            {
                Debug.LogError("GameHUD.uxmlが見つかりません: " + GameHUDPath);
                return;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var go = new GameObject("GameHUD");
            var doc = go.AddComponent<UIDocument>();

            var so = new SerializedObject(doc);
            so.FindProperty("m_PanelSettings").objectReferenceValue = panelSettings;
            so.FindProperty("sourceAsset").objectReferenceValue = gameHUD;
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, ScenePath);
            Debug.Log("UIToolkitテストシーンを作成しました: " + ScenePath);
        }
    }
}
#endif
