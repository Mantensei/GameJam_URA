using UnityEngine;

namespace GameJam_URA.Prototype
{
    public class Shopkeeper : MonoBehaviour
    {
        // [Legacy] RestaurantInputHandler依存のため一時無効化
        /*
        public string InteractLabel => "話しかける";

        public bool CanInteract(RestaurantInputHandler player)
        {
            return GameManager.Instance.State == GameState.Visiting;
        }

        public void Interact(RestaurantInputHandler player)
        {
            var gm = GameManager.Instance;
            gm.SetState(GameState.Judgment);

            var judgment = FindAnyObjectByType<JudgmentSystem>();
            if (judgment != null)
                judgment.ExecuteJudgment();
        }
        */
    }
}
