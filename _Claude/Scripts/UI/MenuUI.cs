using UnityEngine;

namespace GameJam_URA.Prototype
{
    public class MenuUI : MonoBehaviour
    {
        // [Legacy] RestaurantInputHandler依存のため一時無効化
        /*
        [SerializeField] GameObject menuPanel;
        [SerializeField] Transform itemContainer;
        [SerializeField] Button menuItemButtonPrefab;
        [SerializeField] Button closeButton;

        RestaurantInputHandler currentPlayer;
        bool isOpen;

        public bool IsOpen => isOpen;

        void Start()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(Close);
            menuPanel.SetActive(false);
        }

        public void Open(RestaurantInputHandler player)
        {
            currentPlayer = player;
            isOpen = true;
            menuPanel.SetActive(true);
            PopulateMenu();
        }

        public void Close()
        {
            isOpen = false;
            menuPanel.SetActive(false);
            ClearItems();
        }

        void PopulateMenu()
        {
            ClearItems();
            var stage = GameManager.Instance.CurrentStage;
            foreach (var item in stage.AvailableMenuItems)
            {
                if (item.IsSecretMenu) continue;
                CreateMenuButton(item);
            }
        }

        void CreateMenuButton(MenuItemData item)
        {
            if (menuItemButtonPrefab == null) return;
            var btn = Instantiate(menuItemButtonPrefab, itemContainer);
            var text = btn.GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = item.ItemName + " " + item.Price + "円";

            btn.onClick.AddListener(() => OnMenuItemClicked(item));
        }

        void OnMenuItemClicked(MenuItemData item)
        {
            if (currentPlayer == null) return;
            var seat = currentPlayer.CurrentSeat;
            if (seat == null) return;

            OrderSystem.Instance.PlaceOrder(item, seat);
        }

        void ClearItems()
        {
            if (itemContainer == null) return;
            for (int i = itemContainer.childCount - 1; i >= 0; i--)
                Destroy(itemContainer.GetChild(i).gameObject);
        }
        */
    }
}
