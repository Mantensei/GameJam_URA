using UnityEngine;

namespace GameJam_URA.Prototype
{
    [CreateAssetMenu(fileName = "NewMenuItem", menuName = "GameJam/MenuItemData")]
    public class MenuItemData : ScriptableObject
    {
        [SerializeField] string itemName;
        [SerializeField] int price;
        [SerializeField] bool isSecretMenu;

        public string ItemName => itemName;
        public int Price => price;
        public bool IsSecretMenu => isSecretMenu;
    }
}
