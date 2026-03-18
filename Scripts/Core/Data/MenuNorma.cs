using UnityEngine;

namespace GameJam_URA
{
    [CreateAssetMenu(fileName = "NewMenuNorma", menuName = "GameJam/MenuNorma")]
    public class MenuNorma : Norma
    {
        [SerializeField] MenuItem menuItem;

        public MenuItem MenuItem => menuItem;
    }
}
