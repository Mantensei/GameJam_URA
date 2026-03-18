using GameJam_URA.UI;
using UnityEngine;

namespace GameJam_URA
{
    public class Register : MonoBehaviour, IInteractable
    {
        public string ActiontLabel => "会計";

        public void Interact(URA_PlayerReferenceHub player)
        {
            UIViewHub.Instance.Register.Show();
        }
    }
}
