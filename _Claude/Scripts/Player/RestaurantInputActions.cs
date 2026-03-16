using UnityEngine.InputSystem;
using static MantenseiLib.InputBindingKeys;

namespace GameJam_URA.Prototype
{
    public class RestaurantInputActions
    {
        static readonly RestaurantInputActions _instance = new();
        public static RestaurantInputActions Instance => _instance;

        readonly InputActionMap _map;
        public InputAction Interact { get; private set; }
        public InputAction Comment { get; private set; }

        RestaurantInputActions()
        {
            _map = new InputActionMap("Restaurant");

            Interact = _map.AddAction("Interact", type: InputActionType.Button);
            Interact.AddBinding(KB + "e");
            Interact.AddBinding(KB + "z");
            Interact.AddBinding("<Mouse>/leftButton");

            Comment = _map.AddAction("Comment", type: InputActionType.Button);
            Comment.AddBinding(KB + "t");
        }

        public static void Enable() => _instance._map.Enable();
        public static void Disable() => _instance._map.Disable();
    }
}
