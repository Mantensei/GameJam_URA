using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using MantenseiLib;

namespace GameJam_URA.UI
{
    #region Data

    public partial class MenuView : UIViewBase
    {
        public override UIViewType ViewType => UIViewType.Menu;

        const string ListItemClass = "popup-list-item";
        const string ItemLabelClass = "popup-list-item-label";
        const string ItemPriceClass = "menu-item-price";
        const string SoldClass = "menu-item-sold";
        const string OrderClass = "menu-item-order";
        const string DobonClass = "menu-item-dobon";

        enum MarkState { Neutral, Order, Dobon }

        Dictionary<IDishItem, VisualElement> _menuItemMap;
        Dictionary<IDishItem, MarkState> _menuItemStates;

        List<IDishItem> _menuItems;
        public IReadOnlyList<IDishItem> MenuItems => _menuItems;

        void InitData()
        {
            var stage = GameManager.Instance.CurrentStage;
            _menuItems = stage.MenuList;
            _menuItemMap = new Dictionary<IDishItem, VisualElement>();
            _menuItemStates ??= new Dictionary<IDishItem, MarkState>();
        }
    }

    #endregion

    #region Behavior

    public partial class MenuView
    {
        public event Action<IDishItem> OnOrderSelected;

        protected override void OnShown()
        {
            InitData();
            InitBehavior();
        }

        const int FavorPerSafe = 10;
        const int FavorPerDobon = -25;

        void OnSelectItem(IDishItem menuItem)
        {
            menuItem.Complete();

            bool isDobon = menuItem.TaskType == UraTaskType.Dobon;
            int delta = isDobon ? FavorPerDobon : FavorPerSafe;
            var gm = GameManager.Instance;
            gm.AddFavor(delta);

            Debug.Log($"[注文] {menuItem.Name} → 好感度{(delta >= 0 ? "+" : "")}{delta} (現在: {gm.CurrentFavor})");

            Hide();
        }

        VisualElement CreateMenuItemElement(IDishItem data)
        {
            var item = new VisualElement();
            item.AddToClassList(ListItemClass);

            var nameLabel = new Label("　" + data.Name + data.CategorySymbol());
            nameLabel.AddToClassList(ItemLabelClass);
            nameLabel.style.color = data.CategoryColor();

            var priceLabel = new Label("¥" + data.Price);
            priceLabel.AddToClassList(ItemPriceClass);

            item.Add(nameLabel);
            item.Add(priceLabel);

            if (data.IsCompleted)
                item.AddToClassList(SoldClass);

            return item;
        }

        void InitBehavior()
        {
            var leftColumn = Root.Q<VisualElement>(className: "menu-list-left");
            var rightColumn = Root.Q<VisualElement>(className: "menu-list-right");
            int half = (_menuItems.Count + 1) / 2;

            for (int i = 0; i < _menuItems.Count; i++)
            {
                var element = CreateMenuItemElement(_menuItems[i]);
                _menuItemMap[_menuItems[i]] = element;
                var column = i < half ? leftColumn : rightColumn;
                column.Add(element);
            }

            foreach (var menuItem in _menuItems)
            {
                if (!_menuItemStates.ContainsKey(menuItem))
                    _menuItemStates[menuItem] = MarkState.Neutral;

                ApplyMark(menuItem);

                _menuItemMap[menuItem].RegisterCallback<PointerDownEvent>(e =>
                {
                    if (menuItem.IsCompleted) return;
                    if (e.pointerType == UnityEngine.UIElements.PointerType.touch)
                    {
                        CycleMark(menuItem);
                        return;
                    }
                    switch (e.button)
                    {
                        case 0: SetMark(menuItem, MarkState.Order); break;
                        case 1: SetMark(menuItem, MarkState.Dobon); break;
                        case 2: SetMark(menuItem, MarkState.Neutral); break;
                    }
                });
            }

            Root.Q<Label>("menu-close-btn").RegisterCallback<ClickEvent>(_ => Hide());

            var overlay = Root.Q("menu-overlay");
            overlay.RegisterCallback<ClickEvent>(e =>
            {
                if (e.target == overlay) Hide();
            });
        }

        void SetMark(IDishItem item, MarkState state)
        {
            _menuItemStates[item] = state;
            ApplyMark(item);
        }

        void CycleMark(IDishItem item)
        {
            var next = _menuItemStates[item] switch
            {
                MarkState.Neutral => MarkState.Order,
                MarkState.Order => MarkState.Dobon,
                _ => MarkState.Neutral,
            };
            SetMark(item, next);
        }

        void ApplyMark(IDishItem item)
        {
            var element = _menuItemMap[item];
            var state = _menuItemStates[item];

            element.RemoveFromClassList(OrderClass);
            element.RemoveFromClassList(DobonClass);

            var label = element.Q<Label>(className: ItemLabelClass);
            var prefix = state switch
            {
                MarkState.Order => "○",
                MarkState.Dobon => "×",
                _ => "　",
            };
            label.text = prefix + item.Name + item.CategorySymbol();

            switch (state)
            {
                case MarkState.Order: element.AddToClassList(OrderClass); break;
                case MarkState.Dobon: element.AddToClassList(DobonClass); break;
            }
        }
    }

    #endregion
}
