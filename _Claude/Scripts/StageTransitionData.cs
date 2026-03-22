using System.Collections.Generic;

namespace GameJam_URA
{
    public enum StageTransitionType { None, NewGame, Retry }

    public static class StageTransitionData
    {
        public static StageTransitionType TransitionType { get; private set; }

        static Dictionary<IDishItem, int> savedMarks;
        public static IReadOnlyDictionary<IDishItem, int> SavedMarks => savedMarks;

        public static void SetupNewGame()
        {
            TransitionType = StageTransitionType.NewGame;
            OrderLog.Clear();
            savedMarks = null;
        }

        public static void SetupRetry(Dictionary<IDishItem, int> marks)
        {
            TransitionType = StageTransitionType.Retry;
            savedMarks = marks;
        }

        public static void Clear()
        {
            TransitionType = StageTransitionType.None;
            savedMarks = null;
        }
    }
}
