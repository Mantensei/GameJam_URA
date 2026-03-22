using System.Collections.Generic;
using UnityEngine;

namespace GameJam_URA
{
    public class OrderRecord
    {
        public IDishItem Dish;
        public bool IsRevealed;

        public string DisplayName => IsRevealed ? Dish.CategorySymbol() + " " + Dish.Name : Dish.CategorySymbol();
    }

    public static class OrderLog
    {
        static readonly List<OrderRecord> records = new List<OrderRecord>();

        public static IReadOnlyList<OrderRecord> Records => records;

        public static void Add(IDishItem dish)
        {
            records.Add(new OrderRecord { Dish = dish });
        }

        public static void Reveal(IDishItem dish)
        {
            foreach (var record in records)
            {
                if (record.Dish == dish)
                    record.IsRevealed = true;
            }
        }

        public static void Clear()
        {
            records.Clear();
        }
    }
}
