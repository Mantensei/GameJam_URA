using UnityEngine;
using UnityEngine.UIElements;

namespace GameJam_URA.UI
{
    public class GameHUDView : UIViewBase
    {
        public override UIViewType ViewType => UIViewType.GameHUD;
        protected override bool ClosableByEscape => false;

        public Label Money { get; private set; }
        public Label Stage { get; private set; }
        public VisualElement ProgressBar { get; private set; }
        public VisualElement ProgressFill { get; private set; }
        public VisualElement MenuBtn { get; private set; }
        public VisualElement ActionBtn { get; private set; }
        public Label ActionText { get; private set; }

        void Start()
        {
            Money = Root.Q<Label>("money");
            Stage = Root.Q<Label>("stage");
            ProgressBar = Root.Q("progress-bar");
            ProgressFill = Root.Q("progress-fill");
            MenuBtn = Root.Q("menu-btn");
            ActionBtn = Root.Q("action-btn");
            ActionText = Root.Q<Label>(className: "action-text");
        }

        public void SetActionLabel(string text)
        {
            ActionText.text = text;
        }
    }
}
