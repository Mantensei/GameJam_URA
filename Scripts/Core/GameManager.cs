using UnityEngine;

namespace GameJam_URA
{
    public class GameManager
    {
        static GameManager instance;
        public static GameManager Instance => instance;

        readonly StageData[] stages;
        StageData currentStage;
        int currentMoney;

        public StageData CurrentStage => currentStage;
        public int CurrentMoney => currentMoney;

        public void AddMoney(int amount) => currentMoney += amount;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            instance = new GameManager();
        }

        GameManager()
        {
            stages = Resources.LoadAll<StageData>("Stages");
            LoadStage(StageId.Stage0);
        }

        public void LoadStage(StageId id)
        {
            foreach (var stage in stages)
            {
                if (stage.Id != id) continue;
                LoadStage(stage);
                return;
            }
        }

        public void LoadStage(int index)
        {
            LoadStage(stages[index]);
        }

        void LoadStage(StageData stage)
        {
            currentStage = stage;
            stage.Init();
            currentMoney = stage.InitialMoney;
        }
    }
}
