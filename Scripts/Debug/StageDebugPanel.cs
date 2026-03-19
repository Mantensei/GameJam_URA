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
        const string TimePanel = "ステージ時間";
        const string NormaPanel = "ノルマ";
        const string DobonPanel = "ドボン";
        const string MenuPanel = "メニュー";
        const string MoneyPanel = "所持金";
        const string CustomerPanel = "客";

        float elapsedTime;

        void Update()
        {
            if (!URADebugger.IsVisible) return;

            var stage = GameManager.Instance.CurrentStage;
            if (stage == null) return;

            elapsedTime += Time.deltaTime;
            float remaining = Mathf.Max(0f, stage.TimeLimit - elapsedTime);
            RuntimeDebugDisplay.LogPanel(TimePanel, "制限", FormatTime(stage.TimeLimit));
            RuntimeDebugDisplay.LogPanel(TimePanel, "経過", FormatTime(elapsedTime));
            RuntimeDebugDisplay.LogPanel(TimePanel, "残り", FormatTime(remaining));

            foreach (var n in stage.Normas)
                RuntimeDebugDisplay.LogPanel(NormaPanel, n.name, n.IsCompleted ? "○" : "✗");

            foreach (var d in stage.Dobons)
                RuntimeDebugDisplay.LogPanel(DobonPanel, d.name, d.IsCompleted ? "○" : "✗");

            foreach (var m in stage.MenuList)
                RuntimeDebugDisplay.LogPanel(MenuPanel, m.MenuItem.Name, "¥" + m.MenuItem.Price);

            RuntimeDebugDisplay.LogPanel(MoneyPanel, "残金", "¥" + GameManager.Instance.CurrentMoney);

            // var timelines = stage.CustomerTimelineData.Timelines;
            // for (int i = 0; i < timelines.Length; i++)
            // {
            //     var t = timelines[i];
            //     string type = t.IsRegular ? "常連" : "一般";
            //     RuntimeDebugDisplay.LogPanel(CustomerPanel, t.CustomerName, type);
            // }
        }

        static string FormatTime(float seconds)
        {
            int m = (int)(seconds / 60f);
            int s = (int)(seconds % 60f);
            return $"{m:D2}:{s:D2}";
        }

        public void ClearAll()
        {
            RuntimeDebugDisplay.ClearPanel(TimePanel);
            RuntimeDebugDisplay.ClearPanel(NormaPanel);
            RuntimeDebugDisplay.ClearPanel(DobonPanel);
            RuntimeDebugDisplay.ClearPanel(MenuPanel);
            RuntimeDebugDisplay.ClearPanel(MoneyPanel);
            RuntimeDebugDisplay.ClearPanel(CustomerPanel);
            elapsedTime = 0f;
        }

        void OnDestroy()
        {
            ClearAll();
        }
    }
}
