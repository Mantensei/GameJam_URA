using System;
using System.Collections.Generic;
using System.Linq;
using MantenseiLib;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameJam_URA.UI
{
    public class JudgeScreenView : UIViewBase
    {
        public override UIViewType ViewType => UIViewType.JudgeScreen;
        protected override bool ClosableByEscape => false;

        public event Action onCleared;
        public event Action onGameOver;

        const string ListItemClass = "judge-popup-list-item";
        const string ItemLabelClass = "judge-popup-list-item-label";
        const string ItemPriceClass = "judge-item-price";
        const string SoldClass = "judge-item-sold";

        const int TargetScore = 100;

        List<IDishItem> menuItems;
        Dictionary<IDishItem, VisualElement> menuItemMap;
        HashSet<IDishItem> orderedItems;
        List<IDishItem> currentOrder;
        VisualElement menuListContainer;
        VisualElement orderListContainer;
        Button orderButton;
        int totalScore;

        enum JudgeResult { Safe, Dobon, Cleared }
        JudgeResult lastResult;

        protected override void OnShown()
        {
            var stage = GameManager.Instance.CurrentStage;
            menuItems = stage.MenuList;
            menuItemMap = new Dictionary<IDishItem, VisualElement>();
            orderedItems = new HashSet<IDishItem>();
            currentOrder = new List<IDishItem>();
            totalScore = 0;

            menuListContainer = Root.Q("judge-menu-list");
            orderListContainer = Root.Q("judge-order-list");
            orderButton = Root.Q<Button>("judge-order-btn");

            orderButton.clicked += OnOrderSubmit;

            BuildMenuList();
            RefreshOrderList();
        }

        void BuildMenuList()
        {
            menuListContainer.Clear();
            menuItemMap.Clear();

            foreach (var dish in menuItems)
            {
                var item = new VisualElement();
                item.AddToClassList(ListItemClass);

                var prefix = UIViewHub.Instance.Menu.GetMarkPrefix(dish);
                var nameLabel = new Label(prefix + dish.Name + dish.CategorySymbol());
                nameLabel.AddToClassList(ItemLabelClass);
                nameLabel.style.color = dish.CategoryColor();

                var priceLabel = new Label("¥" + dish.Price);
                priceLabel.AddToClassList(ItemPriceClass);

                item.Add(nameLabel);
                item.Add(priceLabel);

                var captured = dish;
                item.RegisterCallback<PointerDownEvent>(e => OnMenuItemClicked(captured));

                menuItemMap[dish] = item;
                menuListContainer.Add(item);
            }
        }

        void OnMenuItemClicked(IDishItem dish)
        {
            if (orderedItems.Contains(dish)) return;
            if (currentOrder.Contains(dish))
                currentOrder.Remove(dish);
            else
                currentOrder.Add(dish);
            RefreshOrderList();
        }

        void RefreshOrderList()
        {
            orderListContainer.Clear();

            foreach (var dish in currentOrder)
            {
                var row = new VisualElement();
                row.AddToClassList("judge-order-item");

                var label = new Label(dish.Name);
                label.AddToClassList("judge-order-item-label");
                label.style.color = dish.CategoryColor();

                var remove = new Label("×");
                remove.AddToClassList("judge-order-item-remove");

                row.Add(label);
                row.Add(remove);

                var captured = dish;
                row.RegisterCallback<PointerDownEvent>(e =>
                {
                    currentOrder.Remove(captured);
                    RefreshOrderList();
                });

                orderListContainer.Add(row);
            }

            orderButton.SetEnabled(currentOrder.Count > 0);
        }

        void OnOrderSubmit()
        {
            if (currentOrder.Count == 0) return;

            var messages = new List<string>();
            messages.Add($"店主「えーと...？」");
            messages.Add($"店主「{currentOrder.Select(x => x.Name).JoinToString("と、")}ね...」");
            var dobon = currentOrder.FirstOrDefault(d => d.TaskType == UraTaskType.Dobon);

            if (dobon != null)
            {
                lastResult = JudgeResult.Dobon;
                messages.Add($"店主「{dobon.Name}...？」");
                messages.Add("店主「...こんなもん頼む舌バカに食わせるものはないわ！」");
                messages.Add("店主「帰ってちょうだい！」");
            }
            else
            {
                int earned = CalcBulkScore(currentOrder.Count);
                totalScore += earned;

                foreach (var dish in currentOrder)
                {
                    orderedItems.Add(dish);
                    dish.Complete();
                    if (menuItemMap.TryGetValue(dish, out var element))
                        element.AddToClassList(SoldClass);
                }

                if (totalScore >= TargetScore)
                {
                    lastResult = JudgeResult.Cleared;
                    messages.Add($"店主「{currentOrder.FirstOrDefault()?.Name}...？」");
                    messages.Add("店主「アナタ...見込みあるわね！」");
                    messages.Add("コトッ");
                    messages.Add("店主「...これはサービスよ」");
                }
                else
                {
                    lastResult = JudgeResult.Safe;
                    messages.Add("店主「おいしい？  ふん、当然ね」");
                    messages.Add("店主「まだまだ食べられるわよね？  もっと頼みなさい」");
                }
            }

            currentOrder.Clear();
            RefreshOrderList();

            var novel = UIViewHub.Instance.NovelPopup;
            novel.onCompleted += OnNovelCompleted;
            novel.ShowMessages(messages);
        }

        void OnNovelCompleted()
        {
            var novel = UIViewHub.Instance.NovelPopup;
            novel.onCompleted -= OnNovelCompleted;

            switch (lastResult)
            {
                case JudgeResult.Dobon:
                    Hide();
                    onGameOver?.Invoke();
                    break;
                case JudgeResult.Cleared:
                    Hide();
                    onCleared?.Invoke();
                    break;
                case JudgeResult.Safe:
                    break;
            }
        }

        static int CalcBulkScore(int k)
        {
            return Mathf.RoundToInt(0.5f * k * (k + 19));
        }
    }
}
