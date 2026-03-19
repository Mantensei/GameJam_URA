using System.Collections.Generic;
using GameJam_URA.UI;
using MantenseiLib;
using UnityEngine;

namespace GameJam_URA
{
    public class CustomerAI : MonoBehaviour
    {
        enum Phase { SitDown, Order, Eat, Leave, Done }

        [Parent]
        URA_PlayerReferenceHub hub;
        IMoverEntity Mover => hub.PlayerHub.MoverReference;

        Phase phase = Phase.SitDown;
        float phaseTimer;
        bool phaseEntered;
        bool moving;
        Vector2 moveTarget;
        string moveLabel;

        bool isRegular;
        List<MenuItem> orderQueue = new List<MenuItem>();
        int orderIndex;

        const float SitDownDuration = 1f;
        const float OrderDuration = 1.5f;
        const float EatDuration = 2f;
        const float LeaveDuration = 1f;
        const float ArrivalThreshold = 0.3f;

        const int DummyOrderMin = 1;
        const int DummyOrderMax = 3;

        List<ICustomerInterrupt> interrupts = new List<ICustomerInterrupt>();

        public void Setup(CustomerData data, StageData stage)
        {
            isRegular = data.IsRegular;
            BuildOrderQueue(stage);
        }

        void BuildOrderQueue(StageData stage)
        {
            orderQueue.Clear();
            orderIndex = 0;

            var normaMenus = new List<MenuItem>();
            var dobonMenus = new List<MenuItem>();
            var dummyMenus = new List<MenuItem>();

            var normaSet = new HashSet<string>();
            foreach (var n in stage.Normas)
                if (n is MenuNorma mn)
                    normaSet.Add(mn.MenuItem.Name);

            var dobonSet = new HashSet<string>();
            foreach (var d in stage.Dobons)
                if (d is MenuNorma mn)
                    dobonSet.Add(mn.MenuItem.Name);

            foreach (var menu in stage.MenuList)
            {
                var item = menu.MenuItem;
                if (normaSet.Contains(item.Name))
                    normaMenus.Add(item);
                else if (dobonSet.Contains(item.Name))
                    dobonMenus.Add(item);
                else
                    dummyMenus.Add(item);
            }

            if (isRegular)
            {
                orderQueue.AddRange(normaMenus);
                int dummyCount = Random.Range(DummyOrderMin, DummyOrderMax + 1);
                Shuffle(dummyMenus);
                for (int i = 0; i < dummyCount && i < dummyMenus.Count; i++)
                    orderQueue.Add(dummyMenus[i]);
            }
            else
            {
                int count = Random.Range(DummyOrderMin, DummyOrderMax + 1);
                var pool = new List<MenuItem>();
                pool.AddRange(dummyMenus);
                pool.AddRange(dobonMenus);
                Shuffle(pool);
                for (int i = 0; i < count && i < pool.Count; i++)
                    orderQueue.Add(pool[i]);
            }

            Shuffle(orderQueue);
        }

        static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public void RegisterInterrupt(ICustomerInterrupt interrupt)
        {
            interrupts.Add(interrupt);
        }

        public void UnregisterInterrupt(ICustomerInterrupt interrupt)
        {
            interrupts.Remove(interrupt);
        }

        void Update()
        {
            if (phase == Phase.Done) return;

            var activeInterrupt = GetActiveInterrupt();
            if (activeInterrupt != null)
            {
                activeInterrupt.Execute();
                return;
            }

            if (moving)
            {
                UpdateMove();
                return;
            }

            ExecuteMainFlow();
        }

        ICustomerInterrupt GetActiveInterrupt()
        {
            ICustomerInterrupt best = null;
            int bestPriority = -1;
            foreach (var interrupt in interrupts)
            {
                if (interrupt.IsActive && interrupt.Priority > bestPriority)
                {
                    best = interrupt;
                    bestPriority = interrupt.Priority;
                }
            }
            return best;
        }

        void ExecuteMainFlow()
        {
            switch (phase)
            {
                case Phase.SitDown: UpdateSitDown(); break;
                case Phase.Order: UpdateOrder(); break;
                case Phase.Eat: UpdateEat(); break;
                case Phase.Leave: UpdateLeave(); break;
            }
        }

        void EnterPhase(Phase next)
        {
            phase = next;
            phaseTimer = 0f;
            phaseEntered = false;
        }

        void ShowBubble(string text, Color color)
        {
            var offset = new Vector3(Random.Range(-0.5f, 0.5f), 0.5f + Random.Range(0f, 0.5f), 0f);
            UIViewHub.Instance.SpeechBubble.Show(new SpeechBubbleCommand
            {
                Parent = transform,
                Text = text,
                Offset = offset,
                TextColor = color,
            });
        }

        void ShowDebugBubble(string text) => ShowBubble(text, Color.red);

        void MoveTo(Vector2 target, string label)
        {
            moveTarget = target;
            moveLabel = label;
            moving = true;
            ShowDebugBubble($"{label}に向かう");
        }

        void UpdateMove()
        {
            float diff = moveTarget.x - transform.position.x;
            if (Mathf.Abs(diff) < ArrivalThreshold)
            {
                Mover?.Move(new MoveCommand(0, 0));
                moving = false;
                ShowDebugBubble($"{moveLabel}に着いた");
                return;
            }
            float dir = diff > 0 ? 1f : -1f;
            Mover?.Move(new MoveCommand(dir, 0));
        }

        Seat mySeat;

        Vector2 GetPosition(MonoBehaviour target)
        {
            return target.transform.position;
        }

        Seat PickSeat()
        {
            var seats = StageObjectHub.Instance.Seats;
            return seats[Random.Range(0, seats.Count)];
        }

        Exit PickExit()
        {
            var exits = StageObjectHub.Instance.Exits;
            return exits[Random.Range(0, exits.Count)];
        }

        void UpdateSitDown()
        {
            if (!phaseEntered)
            {
                phaseEntered = true;
                mySeat = PickSeat();
                MoveTo(GetPosition(mySeat), "席");
                return;
            }

            phaseTimer += Time.deltaTime;
            if (phaseTimer >= SitDownDuration)
                EnterPhase(Phase.Order);
        }

        void UpdateOrder()
        {
            if (!phaseEntered)
            {
                phaseEntered = true;
                if (orderIndex < orderQueue.Count)
                {
                    var item = orderQueue[orderIndex];
                    ShowDebugBubble($"注文：{item.Name}");
                }
            }

            phaseTimer += Time.deltaTime;
            if (phaseTimer >= OrderDuration)
                EnterPhase(Phase.Eat);
        }

        void UpdateEat()
        {
            if (!phaseEntered)
            {
                phaseEntered = true;
                if (orderIndex < orderQueue.Count)
                    ShowDebugBubble($"食事中：{orderQueue[orderIndex].Name}");
            }

            phaseTimer += Time.deltaTime;
            if (phaseTimer >= EatDuration)
            {
                orderIndex++;
                if (orderIndex < orderQueue.Count)
                    EnterPhase(Phase.Order);
                else
                {
                    MoveTo(GetPosition(PickExit()), "出口");
                    EnterPhase(Phase.Leave);
                }
            }
        }

        void UpdateLeave()
        {
            if (!phaseEntered)
            {
                phaseEntered = true;
                ShowDebugBubble("退店");
            }

            phaseTimer += Time.deltaTime;
            if (phaseTimer >= LeaveDuration)
            {
                phase = Phase.Done;
                Destroy(gameObject);
            }
        }
    }

    public interface ICustomerInterrupt
    {
        int Priority { get; }
        bool IsActive { get; }
        void Execute();
    }
}
