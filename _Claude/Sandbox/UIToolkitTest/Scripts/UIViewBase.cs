using UnityEngine;
using UnityEngine.UIElements;
using MantenseiLib;

namespace GameJam_URA.UI
{
    public enum UIViewType
    {
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

        [GetComponent]
        protected UIDocument UIDocument { get; private set; }

        protected VisualElement Root => UIDocument.rootVisualElement;

        public void Show()
        {
            Root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            Root.style.display = DisplayStyle.None;
        }
    }

    public static class UIViewBaseExtensions
    {

    }
}
