using UnityEngine;
using MantenseiLib;
using GameJam_URA.UI;

namespace GameJam_URA
{
    public interface IInteractable : IMonoBehaviour
    {
        void Interact(URA_PlayerReferenceHub player);
        string ActiontLabel { get; }
    }

    public class Interactor : MonoBehaviour
    {
        [Parent]
        URA_PlayerReferenceHub _player;

        InteractableDetector Detector => _player.Detector;
        public IInteractable CurrentTarget => Detector.CurrentTarget;

        void Start()
        {
            Detector.OnTargetChanged += OnTargetChanged;
        }

        void OnDestroy()
        {
            Detector.OnTargetChanged -= OnTargetChanged;
        }

        void OnTargetChanged(IInteractable target)
        {
            var label = target.Safe()?.ActiontLabel ?? "...";
#if UNITY_EDITOR
            MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "Interactor", "Target changed: " + label);
#endif
            UIViewHub.Instance.GameHUD.SetActionLabel(label);
        }

        public void Interact()
        {
            CurrentTarget.Safe()?.Interact(_player);
        }
    }
}
