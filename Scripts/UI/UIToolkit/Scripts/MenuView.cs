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

        public string GetMarkPrefix(IDishItem dish)
        {
            if (_menuItemStates == null) return "　";
            if (!_menuItemStates.TryGetValue(dish, out var state)) return "　";
            return state switch
            {
                MarkState.Order => "○",
                MarkState.Dobon => "×",
                _ => "　",
            };
        }

        List<IDishItem> _menuItems;
        public IReadOnlyList<IDishItem> MenuItems => _menuItems;

        void InitData()
        {
            var stage = GameManager.Instance.CurrentStage;
            _menuItems = stage.MenuList;
            _menuItemMap = new Dictionary<IDishItem, VisualElement>();
            bool wasNull = _menuItemStates == null;
            _menuItemStates ??= new Dictionary<IDishItem, MarkState>();
            MantenseiDebug.DebugFileLogger.Log("transition", "MenuView.InitData",
                $"wasNull={wasNull} statesCount={_menuItemStates.Count} menuItemsCount={_menuItems.Count}");
        }

        public Dictionary<IDishItem, int> ExportMarks()
        {
            var result = new Dictionary<IDishItem, int>();
            if (_menuItemStates == null) return result;
            foreach (var kvp in _menuItemStates)
                result[kvp.Key] = (int)kvp.Value;
            return result;
        }

        public void ImportMarks(IReadOnlyDictionary<IDishItem, int> marks)
        {
            _menuItemStates = new Dictionary<IDishItem, MarkState>();
            foreach (var kvp in marks)
                _menuItemStates[kvp.Key] = (MarkState)kvp.Value;
            MantenseiDebug.DebugFileLogger.Log("transition", "MenuView.ImportMarks",
                $"imported {_menuItemStates.Count} marks");
        }

        public void ClearMarks() => _menuItemStates = null;
    }

    #endregion

    #region Behavior

    public partial class MenuView
    {
        public event Action<IDishItem> OnOrderSelected;

        void Start()
        {
            BuildMenu();
        }

        public void BuildMenu()
        {
            InitData();
            InitBehavior();
        }

        protected override void OnShown()
        {
            BuildMenu();
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
            var listContainer = Root.Q<VisualElement>("menu-list-container");
            listContainer.Clear();

            for (int i = 0; i < _menuItems.Count; i++)
            {
                var element = CreateMenuItemElement(_menuItems[i]);
                _menuItemMap[_menuItems[i]] = element;
                listContainer.Add(element);
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

            Root.Q("menu-overlay").pickingMode = PickingMode.Ignore;

            Root.RegisterCallback<GeometryChangedEvent>(_ => DumpLayoutDebug());
        }

        void DumpLayoutDebug()
        {
            MantenseiDebug.DebugFileLogger.Clear("menu_layout");

            var listContainer = Root.Q<VisualElement>("menu-list-container");
            var popup = Root.Q<VisualElement>("menu-popup");
            var overlay = Root.Q<VisualElement>("menu-overlay");

            MantenseiDebug.DebugFileLogger.Log("menu_layout", "OVERLAY",
                $"layout={overlay.layout} worldBound={overlay.worldBound} picking={overlay.pickingMode}");
            MantenseiDebug.DebugFileLogger.Log("menu_layout", "POPUP",
                $"layout={popup.layout} worldBound={popup.worldBound} picking={popup.pickingMode}");
            MantenseiDebug.DebugFileLogger.Log("menu_layout", "CONTAINER",
                $"layout={listContainer.layout} worldBound={listContainer.worldBound} picking={listContainer.pickingMode}");

            MantenseiDebug.DebugFileLogger.Log("menu_layout", "SCREEN",
                $"Screen={Screen.width}x{Screen.height} panel.scale={Root.panel.visualTree.worldTransform}");

            foreach (var kvp in _menuItemMap)
            {
                var item = kvp.Value;
                var nameLabel = item.Q<Label>(className: ItemLabelClass);
                var priceLabel = item.Q<Label>(className: ItemPriceClass);

                MantenseiDebug.DebugFileLogger.Log("menu_layout", "ITEM",
                    $"{kvp.Key.Name} | layout={item.layout} worldBound={item.worldBound} contentRect={item.contentRect} picking={item.pickingMode} padding=[T:{item.resolvedStyle.paddingTop},B:{item.resolvedStyle.paddingBottom},L:{item.resolvedStyle.paddingLeft},R:{item.resolvedStyle.paddingRight}] margin=[T:{item.resolvedStyle.marginTop},B:{item.resolvedStyle.marginBottom}] border=[T:{item.resolvedStyle.borderTopWidth},B:{item.resolvedStyle.borderBottomWidth}]");
                MantenseiDebug.DebugFileLogger.Log("menu_layout", "  NAME",
                    $"layout={nameLabel.layout} worldBound={nameLabel.worldBound} contentRect={nameLabel.contentRect} fontSize={nameLabel.resolvedStyle.fontSize} padding=[T:{nameLabel.resolvedStyle.paddingTop},B:{nameLabel.resolvedStyle.paddingBottom}] margin=[T:{nameLabel.resolvedStyle.marginTop},B:{nameLabel.resolvedStyle.marginBottom}] unityTextAlign={nameLabel.resolvedStyle.unityTextAlign}");
                MantenseiDebug.DebugFileLogger.Log("menu_layout", "  PRICE",
                    $"layout={priceLabel.layout} worldBound={priceLabel.worldBound} contentRect={priceLabel.contentRect} fontSize={priceLabel.resolvedStyle.fontSize} padding=[T:{priceLabel.resolvedStyle.paddingTop},B:{priceLabel.resolvedStyle.paddingBottom}] margin=[T:{priceLabel.resolvedStyle.marginTop},B:{priceLabel.resolvedStyle.marginBottom}]");
            }
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
