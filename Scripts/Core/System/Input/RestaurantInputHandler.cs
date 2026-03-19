using MantenseiLib;
using UnityEngine;

namespace GameJam_URA.Prototype
{
    public class RestaurantInputHandler : MonoBehaviour
    {
        [Parent]
        URA_PlayerReferenceHub _ura_hub;
        PlayerReferenceHub _hub => _ura_hub.PlayerHub;
        Interactor Interactor => _ura_hub.Interactor;

        void OnEnable() => RestaurantInputActions.SetActive(true);
        void OnDisable() => RestaurantInputActions.SetActive(false);

        void Update()
        {
            var input = RestaurantInputActions.Instance;

            var h = input.Move.ReadValue<float>();
            _hub.MoverReference?.Move(new MoveCommand(h, 0));

            if (input.Jump.IsPressed())
                _hub.JumperReference?.Jump(JumpCommand.one);

            if (input.Interact.WasPressedThisFrame())
                TryInteract();

            if (input.Comment.WasPressedThisFrame())
                TryComment();
        }

        void TryInteract()
        {
            Interactor?.Interact();
        }

        void TryComment()
        {
            _ura_hub.Commenter?.OpenCommentView();
        }
    }
}
