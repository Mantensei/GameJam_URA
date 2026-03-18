using GameJam_URA.UI;
using UnityEngine;

namespace GameJam_URA
{
    public class Sitter : MonoBehaviour
    {
        public bool IsSitting { get; private set; }
        public Seat CurrentSeat { get; private set; }

        public void SitDown(Seat seat)
        {
            UIViewHub.Instance.Menu.SetActive(true);

            IsSitting = true;
            CurrentSeat = seat;
        }

        public void StandUp()
        {
            UIViewHub.Instance.Menu.SetActive(false);

            IsSitting = false;
            CurrentSeat = null;
        }
    }
}
