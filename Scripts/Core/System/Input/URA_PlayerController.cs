using System.Collections;
using GameJam_URA;
using GameJam_URA.UI;
using MantenseiDebug;
using MantenseiLib;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
            new CustomerHoverManager(gameObject);

            DebugFileLogger.Log("transition", "Start", $"TransitionType={StageTransitionData.TransitionType}");
            if (StageTransitionData.TransitionType != StageTransitionType.None)
            {
                StartCoroutine(ApplyTransitionNextFrame());
            }
        }

        void OnDestroy()
        {
            RestaurantInputActions.SetActive(false);
            CleanupUIBindings();
        }

        float _uiMoveDirection;
        float _elapsed;
        bool _timeUp;

        void Update()
        {
            var input = RestaurantInputActions.Instance;

            var stage = GameManager.Instance.CurrentStage;

            if (!_timeUp)
            {
                _elapsed += Time.deltaTime;
                if (_elapsed >= stage.TimeLimit)
                {
                    _timeUp = true;
                    OnTimeUp();
                }
            }

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

        void OnTimeUp()
        {
            UIViewHub.Instance.OrderScreen.Show();
        }

        void TryOrder()
        {
            var menu = UIViewHub.Instance.Menu;
            if (menu.gameObject.activeSelf)
                menu.Hide();
            else
                menu.Show();
        }

        IEnumerator ApplyTransitionNextFrame()
        {
            yield return null;
            DebugFileLogger.Log("transition", "ApplyTransition", $"type={StageTransitionData.TransitionType}");
            if (StageTransitionData.TransitionType == StageTransitionType.Retry
                && StageTransitionData.SavedMarks != null)
            {
                DebugFileLogger.Log("transition", "ApplyTransition", $"ImportMarks count={StageTransitionData.SavedMarks.Count}");
                var menu = UIViewHub.Instance.Menu;
                menu.ImportMarks(StageTransitionData.SavedMarks);
                menu.BuildMenu();
            }
            UIViewHub.Instance.TitleScreen.OnStartPressed();
            StageTransitionData.Clear();
        }

        // void TryInteract()
        // {
        //     Interactor?.Interact();
        // }

        void TryInspectCustomer()
        {
            var ai = CustomerHoverManager.Instance?.HoveredCustomer;
            if (!ai || ai.Dish == null) return;
            if (Vector2.Distance(transform.position, ai.transform.position) > 3f) return;

            var dish = ai.Dish;
            OrderLog.Reveal(dish);
            var offset = new Vector3(0f, 1f, 0f);
            UIViewHub.Instance.SpeechBubble.Show(new SpeechBubbleCommand
            {
                Parent = transform,
                Text = $"(『{dish.Name}』のニオイがする...)",
                Offset = offset,
                TextColor = dish.CategoryColor(),
            });
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
            UIViewHub.Instance.OrderScreen.onOrderConfirmed += OnOrderConfirmed;
            UIViewHub.Instance.OrderScreen.onRetry += OnRetry;
            UIViewHub.Instance.JudgeScreen.onCleared += OnCleared;
            UIViewHub.Instance.JudgeScreen.onGameOver += OnGameOver;
            UIViewHub.Instance.TitleScreen.onStartClicked += OnGameStart;

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
            UIViewHub.Instance.OrderScreen.onOrderConfirmed -= OnOrderConfirmed;
            UIViewHub.Instance.OrderScreen.onRetry -= OnRetry;
            UIViewHub.Instance.JudgeScreen.onCleared -= OnCleared;
            UIViewHub.Instance.JudgeScreen.onGameOver -= OnGameOver;
            UIViewHub.Instance.TitleScreen.onStartClicked -= OnGameStart;
            _uiMoveDirection = 0f;
        }

        void OnCommentSelected(string comment) => _ura_hub.Commenter.TrySay(comment);

        void OnOrderConfirmed()
        {
            UIViewHub.Instance.JudgeScreen.Show();
        }

        void OnRetry()
        {
            var marks = UIViewHub.Instance.Menu.ExportMarks();
            DebugFileLogger.Log("transition", "OnRetry", $"ExportMarks count={marks.Count}");
            foreach (var kvp in marks)
                DebugFileLogger.Log("transition", "OnRetry", $"  {kvp.Key.Name} = {kvp.Value}");
            StageTransitionData.SetupRetry(marks);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        void OnGameStart()
        {
            if (StageTransitionData.TransitionType != StageTransitionType.None) return;
            StageTransitionData.SetupNewGame();
            var stage = GameManager.Instance.CurrentStage;
            stage.ResetMenu();
            GameManager.Instance.LoadStage(stage.Id);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        void OnCleared()
        {
            UIViewHub.Instance.TitleScreen.Show();
        }

        void OnGameOver()
        {
            UIViewHub.Instance.TitleScreen.Show();
        }
    }
}
