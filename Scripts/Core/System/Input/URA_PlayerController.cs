using GameJam_URA.UI;
using MantenseiLib;
using UnityEngine;

namespace GameJam_URA.Prototype
{
    public partial class URA_PlayerController : MonoBehaviour
    {
        [Parent]
        URA_PlayerReferenceHub _ura_hub;
        PlayerReferenceHub _hub => _ura_hub.PlayerHub;
        Interactor Interactor => _ura_hub.Interactor;

        void Start()
        {
            RestaurantInputActions.SetActive(true);
            InitUIBindings();
        }

        void OnDestroy()
        {
            RestaurantInputActions.SetActive(false);
            CleanupUIBindings();
        }

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

    //本当はこんなダサい実装したくないけど時間かけてられないので我慢
    partial class URA_PlayerController
    {
        void InitUIBindings()
        {
            UIViewHub.Instance.Comment.OnCommentSelected += OnCommentSelected;
        }

        void CleanupUIBindings()
        {
            UIViewHub.Instance.Comment.OnCommentSelected -= OnCommentSelected;
        }

        void OnCommentSelected(string comment) => _ura_hub.Commenter.Say(comment);
    }
}
