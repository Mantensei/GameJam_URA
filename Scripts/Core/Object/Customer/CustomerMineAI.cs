using System.Collections.Generic;
using GameJam_URA.UI;
using MantenseiLib;
using UnityEngine;

namespace GameJam_URA
{
    public class CustomerMineAI : MonoBehaviour
    {
        static readonly List<CustomerMineAI> instances = new List<CustomerMineAI>();
        public static IReadOnlyList<CustomerMineAI> Instances => instances;

        void OnEnable() => instances.Add(this);
        void OnDisable() => instances.Remove(this);

        enum Phase { SitDown, Order, Eat, Reaction, Leave, Done }

        [Parent]
        URA_PlayerReferenceHub hub;
        IMoverEntity Mover => hub.PlayerHub.MoverReference;

        Phase phase = Phase.SitDown;
        float phaseTimer;
        bool phaseEntered;
        bool moving;
        Vector2 moveTarget;
        float moveSpeed;

        public IDishItem Dish { get; private set; }
        public bool IsDobon { get; private set; }

        const float SitDownDuration = 1f;
        const float OrderDuration = 1.5f;
        const float EatDurationMin = 2f;
        const float EatDurationMax = 6f;
        const float ReactionDuration = 1.5f;
        const float ArrivalThreshold = 0.3f;
        const float MogmogInterval = 2f;

        float mogmogTimer;
        float eatDuration;

        List<ICustomerInterrupt> interrupts = new List<ICustomerInterrupt>();

        public void Setup(IDishItem dish, bool isDobon)
        {
            Dish = dish;
            IsDobon = isDobon;
            moveSpeed = Random.Range(0.5f, 2f);
            eatDuration = Random.Range(EatDurationMin, EatDurationMax);
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
                case Phase.Reaction: UpdateReaction(); break;
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
            ShowBubbleAt(transform, text, color);
        }

        void ShowBubbleAt(Transform parent, string text, Color color)
        {
            var offset = new Vector3(Random.Range(-0.5f, 0.5f), 0.5f + Random.Range(0f, 0.5f), 0f);
            UIViewHub.Instance.SpeechBubble.Show(new SpeechBubbleCommand
            {
                Parent = parent,
                Text = text,
                Offset = offset,
                TextColor = color,
            });
        }

        void MoveTo(Vector2 target)
        {
            moveTarget = target;
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
            Mover?.Move(new MoveCommand(dir * moveSpeed, 0));
        }

        Seat mySeat;

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
                MoveTo(mySeat.transform.position);
                return;
            }

            if (moving) return;

            if (mySeat.IsOccupied(gameObject))
            {
                mySeat = PickSeat();
                MoveTo(mySeat.transform.position);
                return;
            }

            if (!mySeat.Occupant)
                mySeat.Occupy(gameObject);

            phaseTimer += Time.deltaTime;
            if (phaseTimer >= SitDownDuration)
                EnterPhase(Phase.Order);
        }

        void UpdateOrder()
        {
            if (!phaseEntered)
            {
                phaseEntered = true;
                ShowBubble(Dish.Name, Dish.CategoryColor());
                OrderLog.Add(Dish);
            }

            phaseTimer += Time.deltaTime;
            if (phaseTimer >= OrderDuration)
                EnterPhase(Phase.Eat);
        }

        void UpdateEat()
        {
            mogmogTimer += Time.deltaTime;
            if (mogmogTimer >= MogmogInterval)
            {
                mogmogTimer = 0f;
                ShowBubble("もぐ...", Dish.CategoryColor());
            }

            phaseTimer += Time.deltaTime;
            if (phaseTimer >= eatDuration)
                EnterPhase(Phase.Reaction);
        }

        void UpdateReaction()
        {
            if (!phaseEntered)
            {
                phaseEntered = true;
                mySeat.Release();
                var symbol = Dish.CategorySymbol();
                var color = Dish.CategoryColor();
                ShowBubble($"{symbol}{ReactionLine(Dish.PriceRange(), IsDobon)}", color);
            }

            phaseTimer += Time.deltaTime;
            if (phaseTimer >= ReactionDuration)
            {
                MoveTo(PickExit().transform.position);
                EnterPhase(Phase.Leave);
            }
        }
        void UpdateLeave()
        {
            if (!moving)
            {
                DestroySelf();
            }
        }

        string ReactionLine(PriceRange range, bool dobon)
        {
            switch (dobon)
            {
                case true:
                    switch (range)
                    {
                        case PriceRange.Low: return "店主「はぁ...注文これだけ？」";
                        case PriceRange.Medium: return "店主「チッ...手間なのわかってる？」";
                        case PriceRange.High: return "店主「ぷっ...見る目ないわね？」";
                        default: return "店主「あいよ」";
                    }
                default:
                    switch (range)
                    {
                        case PriceRange.Low: return "店主「いっぱい食べなさい！」";
                        case PriceRange.Medium: return "店主「あなた、見る目あるのね！」";
                        case PriceRange.High: return "店主「素材にこだわってるのよ！」";
                        default: return "店主「お待ちどうさま～」";
                    }
            }
        }

        void DestroySelf()
        {
            phase = Phase.Done;
            Destroy(hub.gameObject);
        }
    }
}
