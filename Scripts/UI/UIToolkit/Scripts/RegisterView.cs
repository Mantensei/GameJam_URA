using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameJam_URA.UI
{
    public class RegisterView : UIViewBase
    {
        public override UIViewType ViewType => UIViewType.Register;

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

            int total = 0;
            foreach (var item in order)
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

        void Checkout()
        {
            var gm = GameManager.Instance;
            var stage = gm.CurrentStage;

            int foodCost = stage.MenuList
                .Where(item => item.IsCompleted)
                .Sum(item => item.Price);
            gm.AddMoney(-foodCost);
            Debug.Log($"[会計] 食事代 ¥{foodCost} → 残金 ¥{gm.CurrentMoney}");

            var completedDobons = stage.Dobons.Where(t => t.IsCompleted).ToArray();
            bool dobon = completedDobons.Length > 0;
            if (dobon)
            {
                int penalty = stage.DobonPenalty * completedDobons.Length;
                gm.AddMoney(-penalty);
                foreach (var d in completedDobons)
                    Debug.Log($"[ドボン] {d.Name}");
                Debug.Log($"[罰金] ドボン {completedDobons.Length}件 ¥{penalty} → 残金 ¥{gm.CurrentMoney}");

                Debug.Log($"[当店にふさわしくない振る舞い] ({stage.Normas.Count(t => t.IsCompleted)}/{stage.Normas.Count})");

                bool gameOver = gm.CurrentMoney <= 0;
                if (gameOver)
                    Debug.Log("[ゲームオーバー] 所持金が尽きました");
            }
            else
            {
                bool clear = !dobon && stage.Normas.All(t => t.IsCompleted);

                if (clear)
                    Debug.Log("[判定] 全ノルマ達成！クリア！");
                else
                    Debug.Log($"[判定] ノルマ未達成 ({stage.Normas.Count(t => t.IsCompleted)}/{stage.Normas.Count})");

            }
            Hide();
        }
    }
}
