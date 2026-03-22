using System;
using UnityEngine.UIElements;

namespace GameJam_URA.UI
{
    public class OrderScreenView : UIViewBase
    {
        public override UIViewType ViewType => UIViewType.OrderScreen;
        protected override bool ClosableByEscape => false;

        public event Action onOrderConfirmed;
        public event Action onRetry;

        protected override void OnShown()
        {
            Root.Q<Button>("order-confirm-btn").clicked += OnConfirm;
            Root.Q<Button>("order-retry-btn").clicked += OnRetry;

            BuildOrderLog();
        }

        void BuildOrderLog()
        {
            var logList = Root.Q("order-log-list");
            logList.Clear();

            var records = OrderLog.Records;
            if (records.Count == 0)
            {
                var empty = new Label("履歴なし");
                empty.name = "order-log-empty";
                logList.Add(empty);
                return;
            }

            var menu = UIViewHub.Instance.Menu;
            foreach (var record in records)
            {
                var row = new VisualElement();
                row.AddToClassList("order-log-item");

                var prefix = menu.GetMarkPrefix(record.Dish);
                var symbolLabel = new Label(prefix + record.DisplayName);
                symbolLabel.AddToClassList("order-log-symbol");
                symbolLabel.style.color = record.Dish.CategoryColor();

                row.Add(symbolLabel);
                logList.Add(row);
            }
        }

        void OnConfirm()
        {
            Hide();
            onOrderConfirmed?.Invoke();
        }

        void OnRetry()
        {
            Hide();
            onRetry?.Invoke();
        }
    }
}
