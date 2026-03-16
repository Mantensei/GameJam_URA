using System;
using UnityEngine;

namespace GameJam_URA.Prototype
{
    [CreateAssetMenu(fileName = "NewCustomerTimeline", menuName = "GameJam/CustomerTimelineData")]
    public class CustomerTimelineData : ScriptableObject
    {
        [SerializeField] string customerName;
        [SerializeField] bool isRegular;
        [SerializeField] MenuItemData[] orderItems;
        [SerializeField] string comment;
        [SerializeField] PhaseTimings timings;

        public string CustomerName => customerName;
        public bool IsRegular => isRegular;
        public MenuItemData[] OrderItems => orderItems;
        public string Comment => comment;
        public PhaseTimings Timings => timings;
    }

    [Serializable]
    public class PhaseTimings
    {
        public float enterToSit = 2f;
        public float sitToOrder = 3f;
        public float orderToEat = 5f;
        public float eatToComment = 4f;
        public float commentToWhisper = 2f;
        public float whisperToLeave = 2f;
    }
}
