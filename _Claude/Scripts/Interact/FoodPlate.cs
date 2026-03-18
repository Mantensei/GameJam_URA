using UnityEngine;

namespace GameJam_URA.Prototype
{
    public class FoodPlate : MonoBehaviour
    {
        // [Legacy] RestaurantInputHandler依存のため一時無効化
        /*
        MenuItemData menuItem;

        public string InteractLabel => "食べる";
        public MenuItemData MenuItem => menuItem;

        public void Setup(MenuItemData item)
        {
            menuItem = item;
        }

        public bool CanInteract(RestaurantInputHandler player)
        {
            return player.IsSitting;
        }

        public void Interact(RestaurantInputHandler player)
        {
            GameManager.Instance.LogAction("eat:" + menuItem.ItemName);
            Destroy(gameObject);
        }
        */
    }
}
