using System;
using UnityEngine.UIElements;

namespace GameJam_URA.UI
{
    public class CommentView : UIViewBase
    {
        public override UIViewType ViewType => UIViewType.Comment;

        public event Action<string> OnCommentSelected;

        protected override void OnShown()
        {
            var list = Root.Q<VisualElement>(className: "popup-list");
            list.Clear();

            var comments = GameManager.Instance.CurrentStage.CommentList;
            foreach (var comment in comments)
            {
                var label = new Label("・" + comment.Name);
                label.AddToClassList("popup-list-item-label");
                label.RegisterCallback<ClickEvent>(_ =>
                {
                    OnCommentSelected?.Invoke(comment.Name);
                    Hide();
                });
                list.Add(label);
            }
        }
    }
}
