using System.Collections.Generic;
using GameJam_URA.UI;
using MantenseiLib;
using UnityEngine;

namespace GameJam_URA
{
    public class CustomerAI : MonoBehaviour
    {
        enum Phase { SitDown, Order, Eat, MineTrigger, Leave, Done }

        [Parent]
        URA_PlayerReferenceHub hub;
        IMoverEntity Mover => hub.PlayerHub.MoverReference;

        Phase phase = Phase.SitDown;
        float phaseTimer;
        bool phaseEntered;
        bool moving;
        Vector2 moveTarget;
        string moveLabel;

        IDishItem mineDish;

        const float SitDownDuration = 1f;
        const float OrderDuration = 1.5f;
        const float EatDuration = 2f;
        const float MineTriggerDuration = 1.5f;
        const float LeaveDuration = 1f;
        const float ArrivalThreshold = 0.3f;

        List<ICustomerInterrupt> interrupts = new List<ICustomerInterrupt>();

        public void Setup(CustomerData data, IDishItem mineDish)
        {
            this.mineDish = mineDish;
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
                case Phase.MineTrigger: UpdateMineTrigger(); break;
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

        void MoveTo(Vector2 target, string label)
        {
            moveTarget = target;
            moveLabel = label;
            moving = true;
        }

        void UpdateMove()
        {
            float diff = moveTarget.x - transform.position.x;
            if (Mathf.Abs(diff) < ArrivalThreshold)
            {
                Mover?.Move(new MoveCommand(0, 0));
                moving = false;
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
                ShowBubble(mineDish.Name, Color.black);
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
            }

            phaseTimer += Time.deltaTime;
            if (phaseTimer >= EatDuration)
                EnterPhase(Phase.MineTrigger);
        }

        void UpdateMineTrigger()
        {
            if (!phaseEntered)
            {
                phaseEntered = true;
                ShowBubble("!!!", Color.red);
            }

            phaseTimer += Time.deltaTime;
            if (phaseTimer >= MineTriggerDuration)
            {
                MoveTo(GetPosition(PickExit()), "出口");
                EnterPhase(Phase.Leave);
            }
        }

        void UpdateLeave()
        {
            if (!phaseEntered)
            {
                phaseEntered = true;
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
