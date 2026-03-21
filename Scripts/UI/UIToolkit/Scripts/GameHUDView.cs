using UnityEngine;
using UnityEngine.UIElements;

namespace GameJam_URA.UI
{
    public class GameHUDView : UIViewBase
    {
        public override UIViewType ViewType => UIViewType.GameHUD;
        protected override bool ClosableByEscape => false;

        public Label TimeLabel { get; private set; }
        public Label Stage { get; private set; }
        public VisualElement ProgressBar { get; private set; }
        public VisualElement ProgressFill { get; private set; }
        public VisualElement MenuBtn { get; private set; }
        public VisualElement ActionBtn { get; private set; }
        public Label ActionText { get; private set; }
        public VisualElement KeyLeft { get; private set; }
        public VisualElement KeyRight { get; private set; }
        public VisualElement MenuOpenBtn { get; private set; }

        void Awake()
        {
            TimeLabel = Root.Q<Label>("time-label");
            Stage = Root.Q<Label>("stage");
            ProgressBar = Root.Q("progress-bar");
            ProgressFill = Root.Q("progress-fill");
            MenuBtn = Root.Q("menu-btn");
            ActionBtn = Root.Q("action-btn");
            ActionText = Root.Q<Label>(className: "action-text");
            KeyLeft = Root.Q("key-left");
            KeyRight = Root.Q("key-right");
            MenuOpenBtn = Root.Q("menu-open-btn");
        }

        public void SetActionLabel(string text)
        {
            // ActionText.text = text;
        }

        public void UpdateTime(float remaining, float total)
        {
            int sec = Mathf.CeilToInt(Mathf.Max(0f, remaining));
            TimeLabel.text = $"残り {sec}秒";
            float ratio = Mathf.Clamp01(remaining / total);
            ProgressFill.style.width = new StyleLength(new Length(ratio * 100f, LengthUnit.Percent));
        }
    }
}
