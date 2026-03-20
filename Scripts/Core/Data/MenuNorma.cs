using System;
using UnityEngine;

namespace GameJam_URA
{
    [Obsolete("SO基底。今後はDishItemを直接使用すること。IUraTaskProvider経由で取得")]
    [CreateAssetMenu(fileName = "NewMenuNorma", menuName = "GameJam/MenuNorma")]
    public class MenuNorma : Norma, IDishItem
    {
        [SerializeField] string itemName;
        [SerializeField] int price;
        [SerializeField] string category;

        int IDishItem.Price => price;
        string IDishItem.Category => category;
        public bool IsSecretMenu { get; set; }

        public override string ToString() => itemName;
    }
}
