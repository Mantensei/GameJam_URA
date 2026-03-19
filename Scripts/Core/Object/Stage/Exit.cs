using UnityEngine;

namespace GameJam_URA
{
    public class Exit : MonoBehaviour, IInteractable
    {
        public string ActiontLabel => "出口";

        public void Interact(URA_PlayerReferenceHub player) { }
    }
}
