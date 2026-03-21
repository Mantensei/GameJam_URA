using System;
using UnityEngine;
using UnityEngine.UIElements;
using MantenseiLib;
using MantenseiLib.GetComponent;

namespace GameJam_URA.UI
{
    public enum UIViewType
    {
        TitleScreen,
        GameHUD,
        Menu,
        Comment,
        Register,
    }

    public interface IUIView : IMonoBehaviour
    {
        UIViewType ViewType { get; }
        void Show();
        void Hide();
    }

    [RequireComponent(typeof(UIDocument))]
    public abstract class UIViewBase : MonoBehaviour, IUIView
    {
        public abstract UIViewType ViewType { get; }
        protected virtual bool ClosableByEscape => true;

        public event Action onShown;
        public event Action onHidden;

        [GetComponent]
        public UIDocument UIDocument { get; protected set; }

        protected VisualElement Root => UIDocument.rootVisualElement;

        public void Show()
        {
            UIInputActions.SetActive(true);
            gameObject.SetActive(true);
            OnShown();
            onShown?.Invoke();
        }

        protected virtual void OnShown() { }

        public void Hide()
        {
            UIInputActions.SetActive(false);
            gameObject.SetActive(false);
            OnHidden();
            onHidden?.Invoke();
        }

        protected virtual void OnHidden() { }

        void Update()
        {
            if (ClosableByEscape && UIInputActions.Instance.Cancel.WasPressedThisFrame())
                if (gameObject.activeSelf)
                    Hide();
        }
    }

    public static class UIViewBaseExtensions
    {
        public static void SetActive(this IUIView view, bool active)
        {
            if (active)
                view.Show();
            else
                view.Hide();
        }

    }
}
