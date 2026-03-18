using System;
using UnityEngine;
using MantenseiLib;

namespace GameJam_URA.Prototype
{
    public enum CustomerPhase
    {
        Inactive,
        Entering,
        Sitting,
        Ordering,
        Eating,
        Commenting,
        Whispering,
        Leaving,
        Gone
    }

    public class CustomerAI : MonoBehaviour
    {
        /*
        [SerializeField] CustomerTimelineData timelineData;
        [Parent]
        PlayerReferenceHub hub;

        CustomerPhase phase = CustomerPhase.Inactive;
        float phaseTimer;
        Seat assignedSeat;
        Transform exitPoint;
        Transform shopkeeperPoint;
        SpeechBubble speechBubble;

        public CustomerPhase Phase => phase;
        public CustomerTimelineData TimelineData => timelineData;
        public event Action<CustomerAI> OnWhisperStart;

        public void Initialize(CustomerTimelineData data, Seat seat, Transform exit, Transform shopkeeper)
        {
            timelineData = data;
            assignedSeat = seat;
            exitPoint = exit;
            shopkeeperPoint = shopkeeper;
            speechBubble = SpeechBubble.GetOrCreate(transform);
            SetPhase(CustomerPhase.Entering);
        }

        void Update()
        {
            switch (phase)
            {
                case CustomerPhase.Entering:
                    UpdateMoveTo(assignedSeat.transform.position, CustomerPhase.Sitting, timelineData.Timings.sitToOrder);
                    break;
                case CustomerPhase.Sitting:
                    UpdateTimer(CustomerPhase.Ordering);
                    break;
                case CustomerPhase.Ordering:
                    ShowOrderBubble();
                    SetPhaseWithTimer(CustomerPhase.Eating, timelineData.Timings.orderToEat);
                    break;
                case CustomerPhase.Eating:
                    UpdateTimer(CustomerPhase.Commenting);
                    break;
                case CustomerPhase.Commenting:
                    ShowCommentBubble();
                    float nextTime = timelineData.IsRegular ? timelineData.Timings.commentToWhisper : timelineData.Timings.whisperToLeave;
                    CustomerPhase nextPhase = timelineData.IsRegular ? CustomerPhase.Whispering : CustomerPhase.Leaving;
                    SetPhaseWithTimer(nextPhase, nextTime);
                    break;
                case CustomerPhase.Whispering:
                    OnWhisperStart?.Invoke(this);
                    ShowWhisperBubble();
                    SetPhaseWithTimer(CustomerPhase.Leaving, timelineData.Timings.whisperToLeave);
                    break;
                case CustomerPhase.Leaving:
                    if (exitPoint != null)
                        UpdateMoveTo(exitPoint.position, CustomerPhase.Gone, 0f);
                    else
                        SetPhase(CustomerPhase.Gone);
                    break;
                case CustomerPhase.Gone:
                    gameObject.SetActive(false);
                    break;
            }
        }

        void UpdateMoveTo(Vector3 target, CustomerPhase nextPhase, float nextTimer)
        {
            Vector2 diff = target - transform.position;
            if (diff.magnitude < 0.2f)
            {
                StopMoving();
                transform.position = target;
                SetPhaseWithTimer(nextPhase, nextTimer);
                return;
            }
            float dir = diff.x > 0 ? 1f : -1f;
            hub.MoverReference?.Move(new MoveCommand(dir, 0));
        }

        void UpdateTimer(CustomerPhase nextPhase)
        {
            phaseTimer -= Time.deltaTime;
            if (phaseTimer <= 0f)
                SetPhase(nextPhase);
        }

        void SetPhase(CustomerPhase newPhase)
        {
            phase = newPhase;
            phaseTimer = 0f;
#if UNITY_EDITOR
            string name = timelineData != null ? timelineData.CustomerName : "?";
            MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "Customer", name + " → " + newPhase);
#endif
        }

        void SetPhaseWithTimer(CustomerPhase newPhase, float time)
        {
            phase = newPhase;
            phaseTimer = time;
#if UNITY_EDITOR
            string name = timelineData != null ? timelineData.CustomerName : "?";
            MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "Customer", name + " → " + newPhase + " (timer=" + time + "s)");
#endif
        }

        void StopMoving()
        {
            hub.MoverReference?.Move(new MoveCommand(0, 0));
        }

        void ShowOrderBubble()
        {
            if (speechBubble == null) return;
            string orderText = "";
            foreach (var item in timelineData.OrderItems)
            {
                if (orderText.Length > 0) orderText += "、";
                orderText += item.ItemName;
            }
            speechBubble.Show(orderText + "ください");
        }

        void ShowCommentBubble()
        {
            if (speechBubble == null || string.IsNullOrEmpty(timelineData.Comment)) return;
            speechBubble.Show(timelineData.Comment);
        }

        void ShowWhisperBubble()
        {
            if (speechBubble == null) return;
            speechBubble.Show("（コソコソ…）");
        }
        */
    }
}
