using MantenseiLib;

namespace GameJam_URA
{
    public class DishItem : IDishItem
    {
        public string StageId { get; }
        public string Name { get; }
        public int Price { get; }
        public string Category { get; }
        public bool IsSecretMenu { get; set; }
        public bool IsCompleted { get; private set; }

        public DishItem(string stageId, string name, int price, string category)
        {
            StageId = stageId;
            Name = name;
            Price = price;
            Category = category;
        }

        public void Complete()
        {
            IsCompleted = true;
        }

        public static DishItem FromCsvRow(CsvRow row)
        {
            return new DishItem(row["stage_id"], row["name"], row.GetInt("price"), row["category"]);
        }
    }
}
