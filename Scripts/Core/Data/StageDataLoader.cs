using System.Collections.Generic;
using MantenseiLib;

namespace GameJam_URA
{
    public enum StageId
    {
        Stage0,
        Stage1,
        Stage2,
        Stage3,
    }

    public static class StageIdExtensions
    {
        public static int DobonPenalty(this StageId id) => id switch
        {
            StageId.Stage0 => 500,
            StageId.Stage1 => 500,
            StageId.Stage2 => 500,
            StageId.Stage3 => 500,
            _ => 500,
        };

        public static string TsvKey(this StageId id) => id switch
        {
            StageId.Stage0 => "stage0",
            StageId.Stage1 => "stage1",
            StageId.Stage2 => "stage2",
            StageId.Stage3 => "stage3",
            _ => id.ToString().ToLower(),
        };
    }

    public static class StageDataLoader
    {
        const string MenuCsvPath = "Stages/Tables/menu";
        const string CommentCsvPath = "Stages/Tables/comment";

        public static List<DishItem> LoadMenus()
        {
            var rows = CsvParser.Load(MenuCsvPath);
            var items = new List<DishItem>();
            foreach (var row in rows)
                items.Add(DishItem.FromCsvRow(row));
            return items;
        }

        public static List<CommentItem> LoadComments()
        {
            var rows = CsvParser.Load(CommentCsvPath);
            var items = new List<CommentItem>();
            foreach (var row in rows)
                items.Add(CommentItem.FromCsvRow(row));
            return items;
        }

        public static List<DishItem> LoadMenus(StageId stageId)
        {
            var key = stageId.TsvKey();
            var items = new List<DishItem>();
            foreach (var menu in LoadMenus())
            {
                if (menu.StageId == key)
                    items.Add(menu);
            }
            return items;
        }

        public static List<CommentItem> LoadComments(StageId stageId)
        {
            var key = stageId.TsvKey();
            var items = new List<CommentItem>();
            foreach (var comment in LoadComments())
            {
                if (comment.StageId == key)
                    items.Add(comment);
            }
            return items;
        }
    }
}
