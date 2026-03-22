using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace GameJam_URA.UI
{
    public class NovelPopupView : UIViewBase
    {
        public override UIViewType ViewType => UIViewType.NovelPopup;
        protected override bool ClosableByEscape => false;

        public event Action onCompleted;

        Label textLabel;
        List<string> messages;
        int currentIndex;

        protected override void OnShown()
        {
            textLabel = Root.Q<Label>("novel-text");
            Root.Q("novel-overlay").pickingMode = PickingMode.Position;
            Root.Q("novel-overlay").RegisterCallback<PointerDownEvent>(OnClick);
        }

        public void ShowMessages(IList<string> lines)
        {
            messages = new List<string>(lines);
            currentIndex = 0;
            Show();
            DisplayCurrent();
        }

        void DisplayCurrent()
        {
            if (currentIndex < messages.Count)
                textLabel.text = messages[currentIndex];
        }

        void OnClick(PointerDownEvent evt)
        {
            currentIndex++;
            if (currentIndex < messages.Count)
            {
                DisplayCurrent();
                return;
            }

            Root.Q("novel-overlay").UnregisterCallback<PointerDownEvent>(OnClick);
            Hide();
            onCompleted?.Invoke();
        }
    }
}
