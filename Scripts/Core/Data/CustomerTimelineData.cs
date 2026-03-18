using System;
using UnityEngine;

namespace GameJam_URA
{
    [Serializable]
    public class CustomerTimeline
    {
        [SerializeField] string customerName;
        bool isRegular;

        public string CustomerName => customerName;
        public bool IsRegular => isRegular;
    }

    [CreateAssetMenu(fileName = "NewCustomerTimelineData", menuName = "GameJam/CustomerTimelineData")]
    public class CustomerTimelineData : ScriptableObject
    {
        [SerializeField] CustomerTimeline[] timelines;

        public CustomerTimeline[] Timelines => timelines;
    }
}
