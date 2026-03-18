using UnityEngine;

namespace GameJam_URA
{
    public class Seat : MonoBehaviour, IInteractable
    {
        public string ActiontLabel => "座る";

        public void Interact(URA_PlayerReferenceHub player)
        {
            var sitter = player.Sitter;
            if (sitter.IsSitting)
                sitter.StandUp();
            else
                sitter.SitDown(this);
        }
    }
}
