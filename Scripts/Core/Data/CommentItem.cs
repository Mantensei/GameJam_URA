using System;
using MantenseiLib;

namespace GameJam_URA
{
    public enum CommentPhase { Any, BeforeEat, AfterEat }

    public class CommentItem : ICommentItem
    {
        public string StageId { get; }
        public string Name { get; }
        public CommentPhase Phase { get; }
        public bool IsCompleted { get; private set; }
        public UraTaskType TaskType { get; set; }

        public CommentItem(string stageId, string text, CommentPhase phase)
        {
            StageId = stageId;
            Name = text;
            Phase = phase;
        }

        public void Complete()
        {
            IsCompleted = true;
        }

        public static CommentItem FromCsvRow(CsvRow row)
        {
            var phase = (CommentPhase)Enum.Parse(typeof(CommentPhase), row["phase"]);
            return new CommentItem(row["stage_id"], row["text"], phase);
        }
    }
}
