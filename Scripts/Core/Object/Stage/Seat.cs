using MantenseiLib;
using UnityEngine;

namespace GameJam_URA
{
    public class Seat : MonoBehaviour, IInteractable
    {
        public string ActiontLabel => "座る";
        public GameObject Occupant { get; private set; }

        public bool IsOccupied(GameObject self)
        {
            return Occupant && Occupant != self;
        }

        public void Occupy(GameObject occupant)
        {
            Occupant = occupant;
        }

        public void Release()
        {
            Occupant = null;
        }

        public void Interact(URA_PlayerReferenceHub player)
        {
            var sitter = player.Sitter;
            if (sitter.IsSitting)
            {
                var current = sitter.CurrentSeat;
                if (current.IsSafe()) current.Release();
                sitter.StandUp();
            }
            else
            {
                if (IsOccupied(player.gameObject)) return;
                Occupy(player.gameObject);
                sitter.SitDown(this);
            }
        }
    }
}
