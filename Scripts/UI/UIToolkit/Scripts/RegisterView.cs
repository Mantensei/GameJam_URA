using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameJam_URA.UI
{
    public class RegisterView : UIViewBase
    {
        public override UIViewType ViewType => UIViewType.Register;

        const int FavorPerSafe = 10;
        const int FavorPerMine = -25;

        VisualElement _orderSummaryContainer;

        protected override void OnShown()
        {
            BuildOrderSummary();
            Root.Q<Label>("register-checkout-btn")
                .RegisterCallback<ClickEvent>(_ => Checkout());
        }

        void BuildOrderSummary()
        {
            _orderSummaryContainer = Root.Q<VisualElement>("register-order-summary");
            _orderSummaryContainer.Clear();
            DisplayOrderItems();
        }

        void DisplayOrderItems()
        {
            var order = GameManager.Instance.CurrentStage
                .MenuList
                .Where(item => item.IsCompleted)
                .ToArray();

            if (order.Length == 0)
            {
                _orderSummaryContainer.Add(new Label("未注文") { name = "register-no-order" });
                return;
            }

            var dobonNames = BuildDobonNames();

            foreach (var item in order)
            {
                bool isMine = dobonNames.Contains(item.Name);
                var row = new VisualElement();
                row.AddToClassList("register-order-item");
                row.Add(new Label(item.Name));
                row.Add(new Label(isMine ? "×" : "○"));
                _orderSummaryContainer.Add(row);
            }
        }

        HashSet<string> BuildDobonNames()
        {
            var set = new HashSet<string>();
            foreach (var task in GameManager.Instance.CurrentStage.Dobons)
                if (task is IDishItem dish)
                    set.Add(dish.Name);
            return set;
        }

        void Checkout()
        {
            var gm = GameManager.Instance;
            var stage = gm.CurrentStage;
            var dobonNames = BuildDobonNames();

            var orderedItems = stage.MenuList.Where(item => item.IsCompleted).ToArray();

            int totalFavor = 0;
            foreach (var item in orderedItems)
            {
                bool isMine = dobonNames.Contains(item.Name);
                int delta = isMine ? FavorPerMine : FavorPerSafe;
                totalFavor += delta;
                gm.AddFavor(delta);
                Debug.Log($"[注文] {item.Name} → 好感度{(delta >= 0 ? "+" : "")}{delta} (現在: {gm.CurrentFavor})");
            }

            Debug.Log($"[会計完了] 好感度変動合計: {totalFavor} → 現在: {gm.CurrentFavor}");

            if (gm.CurrentFavor >= 100)
                Debug.Log("[クリア] 好感度MAX！");
            else if (gm.CurrentFavor <= 0)
                Debug.Log("[ゲームオーバー] 好感度が0になりました");

            Hide();
        }
    }
}
