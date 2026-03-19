using System.Diagnostics;
using MantenseiDebug;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJam_URA
{
    public static class URADebugger
    {
        static StageDebugPanel instance;
        static bool visible;
        static InputAction toggleAction;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        [Conditional("UNITY_EDITOR")]
        static void Init()
        {
            var go = new GameObject("URADebugger");
            Object.DontDestroyOnLoad(go);
            instance = go.AddComponent<StageDebugPanel>();

            toggleAction = new InputAction(binding: "<Keyboard>/f1");
            toggleAction.performed += _ => Toggle();
            toggleAction.Enable();
        }

        static void Toggle()
        {
            visible = !visible;
            if (!visible)
                instance.ClearAll();
        }

        public static bool IsVisible => visible;
    }

    public class StageDebugPanel : MonoBehaviour
    {
        const string NormaPanel = "ノルマ";
        const string DobonPanel = "ドボン";
        const string MenuPanel = "メニュー";
        const string MoneyPanel = "所持金";

        void Update()
        {
            if (!URADebugger.IsVisible) return;

            var stage = GameManager.Instance.CurrentStage;
            if (stage == null) return;

            foreach (var n in stage.Normas)
                RuntimeDebugDisplay.LogPanel(NormaPanel, n.name, n.IsCompleted ? "○" : "✗");

            foreach (var d in stage.Dobons)
                RuntimeDebugDisplay.LogPanel(DobonPanel, d.name, d.IsCompleted ? "○" : "✗");

            foreach (var m in stage.MenuList)
                RuntimeDebugDisplay.LogPanel(MenuPanel, m.MenuItem.Name, "¥" + m.MenuItem.Price);

            RuntimeDebugDisplay.LogPanel(MoneyPanel, "残金", "¥" + GameManager.Instance.CurrentMoney);
        }

        public void ClearAll()
        {
            RuntimeDebugDisplay.ClearPanel(NormaPanel);
            RuntimeDebugDisplay.ClearPanel(DobonPanel);
            RuntimeDebugDisplay.ClearPanel(MenuPanel);
            RuntimeDebugDisplay.ClearPanel(MoneyPanel);
        }

        void OnDestroy()
        {
            ClearAll();
        }
    }
}
