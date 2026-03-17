using MantenseiLib;
using UnityEngine;

namespace GameJam_URA.Prototype
{
    public class RestaurantInputHandler : MonoBehaviour
    {
        [SerializeField] InteractableDetector detector;

        bool isSitting;
        Seat currentSeat;

        [Parent]
        PlayerReferenceHub playerHub;
        [Sibling]
        PlayerController playerController;
        MenuUI menuUI;
        CommentUI commentUI;

        public bool IsSitting => isSitting;
        public Seat CurrentSeat => currentSeat;

        void Awake()
        {
            menuUI = FindAnyObjectByType<MenuUI>();
            commentUI = FindAnyObjectByType<CommentUI>();
        }

        void OnEnable() => RestaurantInputActions.Enable();
        void OnDisable() => RestaurantInputActions.Disable();

        void Update()
        {
            if (GameManager.Instance.State != GameState.Visiting) return;

            if (menuUI != null && menuUI.IsOpen) return;
            if (commentUI != null && commentUI.IsOpen) return;

            var input = RestaurantInputActions.Instance;
            if (input.Interact.WasPressedThisFrame())
                TryInteract();

            if (input.Comment.WasPressedThisFrame())
                TryOpenCommentUI();
        }

        void TryInteract()
        {
            var target = detector.CurrentTarget;
            if (target == null) return;
            if (!target.CanInteract(this)) return;
#if UNITY_EDITOR
            MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "Input", "Interact: " + target.InteractLabel + " (" + target.gameObject.name + ")");
#endif
            target.Interact(this);
        }

        void TryOpenCommentUI()
        {
            if (commentUI == null) return;
#if UNITY_EDITOR
            MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "Input", "OpenCommentUI");
#endif
            commentUI.Open();
        }

        public void SitDown(Seat seat)
        {
            isSitting = true;
            currentSeat = seat;
#if UNITY_EDITOR
            MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "Input", "SitDown: " + seat.gameObject.name);
#endif
            if (playerController != null)
                playerController.enabled = false;
        }

        public void StandUp()
        {
            isSitting = false;
            currentSeat = null;
#if UNITY_EDITOR
            MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "Input", "StandUp");
#endif
            if (playerController != null)
                playerController.enabled = true;
        }
    }
}
