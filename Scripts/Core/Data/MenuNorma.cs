using System;
using UnityEngine;

namespace GameJam_URA
{
    [Serializable]
    public class MenuItem
    {
        [SerializeField] string itemName;
        [SerializeField] int price;

        public string Name => itemName;
        public int Price => price;

        public bool IsSecretMenu { get; set; }
    }

    [CreateAssetMenu(fileName = "NewMenuNorma", menuName = "GameJam/MenuNorma")]
    public class MenuNorma : Norma
    {
        [SerializeField] MenuItem menuItem;

        public MenuItem MenuItem => menuItem;

        public override string ToString() => menuItem.Name;

    }
}
