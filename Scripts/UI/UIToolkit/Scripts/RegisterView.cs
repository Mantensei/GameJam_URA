using System;
using System.Linq;
using UnityEngine.UIElements;

namespace GameJam_URA.UI
{
    public class RegisterView : UIViewBase
    {
        public override UIViewType ViewType => UIViewType.Register;

        protected override void OnShown()
        {
            BuildOrderSummary();
        }

        VisualElement _orderSummaryContainer;
        MenuView.TempMenuItem[] _orderedItems;

        void BuildOrderSummary()
        {
            ClearContainer();
            FetchOrderedItems();
            DisplayOrderItems();
        }

        void ClearContainer()
        {
            _orderSummaryContainer = Root.Q<VisualElement>("register-order-summary");
            _orderSummaryContainer.Clear();
        }

        ///TODO:データ層に分離してそっちを参照する
        void FetchOrderedItems()
        {
            var menuItems = UIViewHub.Instance.Menu.MenuItems;
            if (menuItems == null)
            {
                _orderedItems = Array.Empty<MenuView.TempMenuItem>();
                return;
            }
            _orderedItems = menuItems.Where(item => item.sold).ToArray();
        }

        void DisplayOrderItems()
        {
            if (_orderedItems.Length == 0)
            {
                _orderSummaryContainer.Add(new Label("未注文") { name = "register-no-order" });
                return;
            }

            int total = 0;
            foreach (var item in _orderedItems)
            {
                var row = new VisualElement();
                row.AddToClassList("register-order-item");
                row.Add(new Label(item.Name));
                row.Add(new Label("¥" + item.Price));
                _orderSummaryContainer.Add(row);
                total += item.Price;
            }

            var totalRow = new VisualElement();
            totalRow.AddToClassList("register-order-total");
            totalRow.Add(new Label("合計"));
            totalRow.Add(new Label("¥" + total));
            _orderSummaryContainer.Add(totalRow);
        }
    }
}
