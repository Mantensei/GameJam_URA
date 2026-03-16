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
            OnMoneyChanged?.Invoke(money);
            StartVisit();
        }

        public void LogAction(string conditionId)
        {
            actionLog.Add(conditionId);
            OnActionLogged?.Invoke(conditionId);
        }

        public bool SpendMoney(int amount)
        {
            if (money < amount) return false;
            money -= amount;
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
    }
}
