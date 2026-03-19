using UnityEngine;

namespace GameJam_URA
{
    public class Toilet : MonoBehaviour, IInteractable
    {
        public string ActiontLabel => "トイレ";

        public void Interact(URA_PlayerReferenceHub player) { }
    }
}
