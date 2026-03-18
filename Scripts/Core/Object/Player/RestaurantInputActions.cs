using MantenseiLib;
using UnityEngine.InputSystem;
using static MantenseiLib.InputBindingKeys;

namespace GameJam_URA.Prototype
{
    public class RestaurantInputActions : InputActionPreset<RestaurantInputActions>
    {
        const int Priority = 0;
        const string MapName = "Restaurant";
        const string MouseLeft = "<Mouse>/leftButton";
        const string InteractKey1 = KB + "e";
        const string InteractKey2 = KB + "z";
        const string InteractKey3 = KB + "Enter";
        const string CommentKey = KB + "t";

        public InputAction Move { get; private set; }
        public InputAction Jump { get; private set; }
        public InputAction Interact { get; private set; }
        public InputAction Comment { get; private set; }

        public RestaurantInputActions() : base(MapName, Priority) { }

        protected override void InitializeActions(InputActionMap map)
        {
            Move = map.AddAction("Move", type: InputActionType.Value);
            Move.AddCompositeBinding(Composite1DAxis)
                .With(Negative, A)
                .With(Positive, D);
            Move.AddCompositeBinding(Composite1DAxis)
                .With(Negative, LeftArrow)
                .With(Positive, RightArrow);

            Jump = map.AddAction("Jump", type: InputActionType.Button);
            Jump.AddBinding(Space);

            Interact = map.AddAction("Interact", type: InputActionType.Button);
            Interact.AddBinding(InteractKey1);
            Interact.AddBinding(InteractKey2);
            //使うか未定だけど一応
            // Interact.AddBinding(InteractKey3);
            Interact.AddBinding(MouseLeft);

            Comment = map.AddAction("Comment", type: InputActionType.Button);
            Comment.AddBinding(CommentKey);
        }
    }
}
