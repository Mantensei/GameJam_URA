using GameJam_URA.UI;
using MantenseiLib;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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

        float _uiMoveDirection;
        float _elapsed;

        void Update()
        {
            var input = RestaurantInputActions.Instance;

            _elapsed += Time.deltaTime;
            var stage = GameManager.Instance.CurrentStage;
            UIViewHub.Instance.GameHUD.UpdateTime(stage.TimeLimit - _elapsed, stage.TimeLimit);

            var h = input.Move.ReadValue<float>();
            if (h == 0f) h = _uiMoveDirection;
            _hub.MoverReference?.Move(new MoveCommand(h, 0));

            if (input.Jump.IsPressed())
                _hub.JumperReference?.Jump(JumpCommand.one);

            if (input.Comment.WasPressedThisFrame())
                TryOrder();

            // if (input.Interact.WasPressedThisFrame())
            //     TryInteract();

            var pointer = Pointer.current;
            if (pointer != null && pointer.press.wasPressedThisFrame)
                TryInspectCustomer();

            // if (input.Comment.WasPressedThisFrame())
            //     TryComment();
        }

        void TryOrder()
        {
            var menu = UIViewHub.Instance.Menu;
            if (menu.gameObject.activeSelf)
                menu.Hide();
            else
                menu.Show();
        }

        // void TryInteract()
        // {
        //     Interactor?.Interact();
        // }

        void TryInspectCustomer()
        {
            var cam = Camera.main;
            var worldPos = cam.ScreenToWorldPoint(Pointer.current.position.ReadValue());
            var hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (!hit.collider) return;

            var ai = hit.collider.GetComponentInChildren<CustomerMineAI>();
            if (!ai || ai.Dish == null) return;

            if (Vector2.Distance(transform.position, ai.transform.position) > 3f) return;

            var dish = ai.Dish;
            var symbol = dish.CategorySymbol();
            var offset = new Vector3(0f, 1f, 0f);
            var bubble = UIViewHub.Instance.SpeechBubble.Show(new SpeechBubbleCommand
            {
                Parent = transform,
                Text = $"(『{dish.Name}』のニオイがする...)",
                Offset = offset,
                TextColor = dish.CategoryColor(),
            });
            // bubble.transform.SetParent(_ura_hub.transform);
        }

        // void TryComment()
        // {
        // _ura_hub.Commenter?.OpenCommentView();
        // }
    }

    //本当はこんなダサい実装したくないけど時間かけてられないので我慢
    partial class URA_PlayerController
    {
        void InitUIBindings()
        {
            UIViewHub.Instance.Comment.OnCommentSelected += OnCommentSelected;

            var hud = UIViewHub.Instance.GameHUD;
            hud.KeyLeft.RegisterCallback<PointerDownEvent>(_ => _uiMoveDirection = -1f);
            hud.KeyLeft.RegisterCallback<PointerUpEvent>(_ => _uiMoveDirection = 0f);
            hud.KeyLeft.RegisterCallback<PointerLeaveEvent>(_ => _uiMoveDirection = 0f);
            hud.KeyRight.RegisterCallback<PointerDownEvent>(_ => _uiMoveDirection = 1f);
            hud.KeyRight.RegisterCallback<PointerUpEvent>(_ => _uiMoveDirection = 0f);
            hud.KeyRight.RegisterCallback<PointerLeaveEvent>(_ => _uiMoveDirection = 0f);
            hud.MenuOpenBtn.RegisterCallback<ClickEvent>(_ => TryOrder());
        }

        void CleanupUIBindings()
        {
            UIViewHub.Instance.Comment.OnCommentSelected -= OnCommentSelected;
            _uiMoveDirection = 0f;
        }

        void OnCommentSelected(string comment) => _ura_hub.Commenter.TrySay(comment);
    }
}
