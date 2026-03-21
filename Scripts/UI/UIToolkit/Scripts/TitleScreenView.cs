using System;
using UnityEngine.UIElements;

namespace GameJam_URA.UI
{
    public class TitleScreenView : UIViewBase
    {
        public override UIViewType ViewType => UIViewType.TitleScreen;
        protected override bool ClosableByEscape => false;

        public event Action onStartClicked;

        Button btnStart;
        Button btnHowto;
        Button btnCredit;
        VisualElement howtoPopup;
        VisualElement creditPopup;

        VisualElement activePopup;

        void Start()
        {
            btnStart = Root.Q<Button>("btn-start");
            btnHowto = Root.Q<Button>("btn-howto");
            btnCredit = Root.Q<Button>("btn-credit");
            howtoPopup = Root.Q("title-howto-popup");
            creditPopup = Root.Q("title-credit-popup");

            btnStart.clicked += OnStartPressed;
            btnHowto.clicked += () => TogglePopup(howtoPopup);
            btnCredit.clicked += () => TogglePopup(creditPopup);
        }

        void TogglePopup(VisualElement popup)
        {
            if (activePopup == popup)
            {
                popup.AddToClassList("hidden");
                activePopup = null;
                return;
            }

            activePopup?.AddToClassList("hidden");
            popup.RemoveFromClassList("hidden");
            activePopup = popup;
        }

        void OnStartPressed()
        {
            UnityEngine.Time.timeScale = 1f;
            Hide();
            onStartClicked?.Invoke();
        }

        protected override void OnShown()
        {
            UnityEngine.Time.timeScale = 0f;
            howtoPopup.AddToClassList("hidden");
            creditPopup.AddToClassList("hidden");
            activePopup = null;
        }
    }
}
