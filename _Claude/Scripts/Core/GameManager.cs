using System;
using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;

namespace GameJam_URA.Prototype
{
    public enum GameState
    {
        Visiting,
        Judgment,
        Result,
        Clear,
        GameOver
    }

    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        /*
        [SerializeField] StageData currentStage;

        GameState state;
        int money;
        List<string> actionLog = new List<string>();

        public GameState State => state;
        public int Money => money;
        public StageData CurrentStage => currentStage;
        public IReadOnlyList<string> ActionLog => actionLog;

        public event Action<int> OnMoneyChanged;
        public event Action<GameState> OnStateChanged;
        public event Action<string> OnActionLogged;

        void Start()
        {
#if UNITY_EDITOR
            MantenseiLib.Editor.DebugFileLogger.Clear("gameplay");
            MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "GM", "Start");
#endif
            InitializeStage();
        }

        public void StartVisit()
        {
            actionLog.Clear();
            SetState(GameState.Visiting);
        }

        public void InitializeStage()
        {
            money = currentStage.InitialMoney;
#if UNITY_EDITOR
            MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "GM", "InitializeStage money=" + money);
#endif
            OnMoneyChanged?.Invoke(money);
            StartVisit();
        }

        public void LogAction(string conditionId)
        {
            actionLog.Add(conditionId);
#if UNITY_EDITOR
            MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "GM", "LogAction: " + conditionId);
#endif
            OnActionLogged?.Invoke(conditionId);
        }

        public bool SpendMoney(int amount)
        {
            if (money < amount)
            {
#if UNITY_EDITOR
                MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "GM", "SpendMoney FAILED amount=" + amount + " money=" + money);
#endif
                return false;
            }
            money -= amount;
#if UNITY_EDITOR
            MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "GM", "SpendMoney amount=" + amount + " remaining=" + money);
#endif
            OnMoneyChanged?.Invoke(money);
            return true;
        }

        public void DeductPenalty(int amount)
        {
            money = Mathf.Max(0, money - amount);
            OnMoneyChanged?.Invoke(money);
        }

        public void SetState(GameState newState)
        {
            state = newState;
#if UNITY_EDITOR
            MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "GM", "SetState: " + newState);
#endif
            OnStateChanged?.Invoke(state);
        }

        public void OnVisitEnd()
        {
            if (money <= 0)
            {
                SetState(GameState.GameOver);
                return;
            }
            StartVisit();
        }
        */
    }
}
