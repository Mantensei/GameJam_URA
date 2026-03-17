using UnityEngine;

namespace GameJam_URA.Prototype
{
    public class Seat : MonoBehaviour, IInteractable
    {
        [SerializeField] Transform sitPosition;

        RestaurantInputHandler occupant;
        bool menuOpened;

        public bool IsOccupied => occupant != null;
        public Transform SitPosition => sitPosition != null ? sitPosition : transform;

        public string InteractLabel
        {
            get
            {
                if (occupant != null && menuOpened) return "立つ";
                if (occupant != null) return "メニュー";
                return "座る";
            }
        }

        public bool CanInteract(RestaurantInputHandler player)
        {
            if (player.IsSitting && player.CurrentSeat == this) return true;
            if (!player.IsSitting && !IsOccupied) return true;
            return false;
        }

        public void Interact(RestaurantInputHandler player)
        {
#if UNITY_EDITOR
            MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "Seat", gameObject.name + " Interact label=" + InteractLabel);
#endif
            if (player.IsSitting && player.CurrentSeat == this)
            {
                if (!menuOpened)
                {
                    menuOpened = true;
                    var menuUI = Object.FindAnyObjectByType<MenuUI>();
                    if (menuUI != null) menuUI.Open(player);
                    return;
                }

                occupant = null;
                menuOpened = false;
                player.StandUp();
                return;
            }

            occupant = player;
            menuOpened = false;
            player.SitDown(this);
            player.transform.position = sitPosition != null ? sitPosition.position : transform.position;
        }

        public void ClearOccupant()
        {
            occupant = null;
            menuOpened = false;
        }
    }
}
