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
        List<IDishItem> orderQueue = new List<IDishItem>();
        int orderIndex;

        const float SitDownDuration = 1f;
        const float OrderDuration = 1.5f;
        const float EatDuration = 2f;
        const float LeaveDuration = 1f;
        const float ArrivalThreshold = 0.3f;

        int budget;

        List<string> commentQueue = new List<string>();
        int commentIndex;
        float commentTimer;
        const float CommentCoolTime = 5f;
        const float CommentInitialDelay = 3f;
        const int MaxCommentCount = 3;

        List<ICustomerInterrupt> interrupts = new List<ICustomerInterrupt>();

        public void Setup(CustomerData data, StageData stage)
        {
            isRegular = data.IsRegular;
            budget = stage.CustomerBudget;
            BuildOrderQueue(stage);
            BuildCommentQueue(stage);
            commentTimer = CommentInitialDelay;
        }

        void BuildOrderQueue(StageData stage)
        {
            orderQueue.Clear();
            orderIndex = 0;

            var normaMenus = new List<IDishItem>();
            var dobonMenus = new List<IDishItem>();
            var stubMenus = new List<IDishItem>();

            var normaSet = new HashSet<string>();
            foreach (var task in stage.Normas)
                if (task is IDishItem mn)
                    normaSet.Add(mn.Name);

            var dobonSet = new HashSet<string>();
            foreach (var task in stage.Dobons)
                if (task is IDishItem mn)
                    dobonSet.Add(mn.Name);

            foreach (var menu in stage.MenuList)
            {
                if (normaSet.Contains(menu.Name))
                    normaMenus.Add(menu);
                else if (dobonSet.Contains(menu.Name))
                    dobonMenus.Add(menu);
                else
                    stubMenus.Add(menu);
            }

            int remaining = budget;

            if (isRegular)
            {
                foreach (var menu in normaMenus)
                {
                    orderQueue.Add(menu);
                    remaining -= menu.Price;
                }
                FillWithBudget(stubMenus, remaining);
            }
            else
            {
                var pool = new List<IDishItem>();
                pool.AddRange(stubMenus);
                pool.AddRange(dobonMenus);
                FillWithBudget(pool, remaining);
            }

            orderQueue = new List<IDishItem>(orderQueue.Shuffle());
        }

        void FillWithBudget(List<IDishItem> pool, int remaining)
        {
            var used = new HashSet<string>();
            foreach (var item in orderQueue)
                used.Add(item.Name);

            pool = new List<IDishItem>(pool.Shuffle());
            foreach (var item in pool)
            {
                if (remaining < item.Price) continue;
                if (used.Contains(item.Name)) continue;
                orderQueue.Add(item);
                used.Add(item.Name);
                remaining -= item.Price;
            }
        }

        void BuildCommentQueue(StageData stage)
        {
            commentQueue.Clear();
            commentIndex = 0;

            var normaTexts = new HashSet<string>();
            foreach (var task in stage.Normas)
                if (task is ICommentItem ci)
                    normaTexts.Add(ci.Name);

            var dobonTexts = new HashSet<string>();
            foreach (var task in stage.Dobons)
                if (task is ICommentItem ci)
                    dobonTexts.Add(ci.Name);

            var stubTexts = new List<string>();
            foreach (var c in stage.CommentList)
            {
                if (!normaTexts.Contains(c.Name) && !dobonTexts.Contains(c.Name))
                    stubTexts.Add(c.Name);
            }

            if (isRegular)
            {
                foreach (var t in normaTexts)
                    commentQueue.Add(t);

                var shuffled = new List<string>(stubTexts.Shuffle());
                foreach (var t in shuffled)
                {
                    if (commentQueue.Count >= MaxCommentCount) break;
                    commentQueue.Add(t);
                }
            }
            else
            {
                var pool = new List<string>();
                foreach (var t in dobonTexts) pool.Add(t);
                pool.AddRange(stubTexts);
                pool = new List<string>(pool.Shuffle());

                foreach (var t in pool)
                {
                    if (commentQueue.Count >= MaxCommentCount) break;
                    commentQueue.Add(t);
                }
            }

            commentQueue = new List<string>(commentQueue.Shuffle());
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

            // UpdateComment();

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

        Commenter Commenter => hub.Commenter;

        void UpdateComment()
        {
            if (commentIndex >= commentQueue.Count) return;

            commentTimer -= Time.deltaTime;
            if (commentTimer > 0f) return;

            commentTimer = CommentCoolTime;

            if (Commenter.TrySay(commentQueue[commentIndex]))
                commentIndex++;
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

        void ShowDebugBubble(string text) { }
        // void ShowDebugBubble(string text) => ShowBubble(text, Color.red);

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
                    ShowBubble(orderQueue[orderIndex].Name, Color.black);
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
