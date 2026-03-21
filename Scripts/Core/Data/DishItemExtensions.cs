using UnityEngine;

namespace GameJam_URA
{
    public static class DishItemExtensions
    {
        static readonly Color MainColor = new Color32(0xE8, 0x91, 0x3A, 0xFF);
        static readonly Color SideColor = new Color32(0x5D, 0xAE, 0x5D, 0xFF);
        static readonly Color DrinkColor = new Color32(0x4A, 0x90, 0xD9, 0xFF);

        public static Color CategoryColor(this IDishItem dish)
        {
            switch (dish.Category)
            {
                case "メイン": return MainColor;
                case "サイド": return SideColor;
                case "ドリンク": return DrinkColor;
                default: return Color.white;
            }
        }

        public static string CategorySymbol(this IDishItem dish)
        {
            switch (dish.Category)
            {
                case "メイン": return "☆";
                case "サイド": return "⊿";
                case "ドリンク": return "◇";
                default: return "・";
            }
        }

        public static PriceRange PriceRange(this IDishItem dish)
        {
            if (dish.Price <= 500) return GameJam_URA.PriceRange.Low;
            if (dish.Price <= 1499) return GameJam_URA.PriceRange.Medium;
            return GameJam_URA.PriceRange.High;
        }
    }
}
