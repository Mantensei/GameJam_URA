using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

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

        Dictionary<MenuNorma, VisualElement> _menuItemMap;

        List<MenuNorma> _menuItems;
        public IReadOnlyList<MenuNorma> MenuItems => _menuItems;

        void InitData()
        {
            var stage = GameManager.Instance.CurrentStage;
            stage.BuildStageData(out _, out _, out _menuItems);
            _menuItemMap = new Dictionary<MenuNorma, VisualElement>();
        }
    }

    #endregion

    #region Behavior

    public partial class MenuView
    {
        public event Action<MenuNorma> OnOrderSelected;

        protected override void OnShown()
        {
            InitData();
            InitBehavior();
        }

        void OnSelectItem(MenuNorma menuItem)
        {
            menuItem.IsCompleted = true;
            _menuItemMap[menuItem].AddToClassList(SoldClass);
            _menuItemMap[menuItem].SetEnabled(false);
        }

        VisualElement CreateMenuItemElement(MenuNorma data)
        {
            var item = new VisualElement();
            item.AddToClassList(ListItemClass);

            var nameLabel = new Label("・" + data.MenuItem.ItemName);
            nameLabel.AddToClassList(ItemLabelClass);

            var priceLabel = new Label("¥" + data.MenuItem.Price);
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
                _menuItemMap[menuItem].RegisterCallback<ClickEvent>(_ =>
                {
                    if (menuItem.IsCompleted) return;
                    OnSelectItem(menuItem);
                    OnOrderSelected?.Invoke(menuItem);
                });
            }
        }
    }

    #endregion
}
