using UnityEngine;

namespace GameJam_URA
{
    [CreateAssetMenu(fileName = "Customer_", menuName = "GameJam/CustomerData")]
    public class CustomerData : ScriptableObject
    {
        [SerializeField] ActionTimelineData customerTimeline;
        [SerializeField] URA_PlayerReferenceHub customerPrefab;

        public URA_PlayerReferenceHub Prefab => customerPrefab;
        public ActionTimelineData Timeline => customerTimeline;
        public bool IsRegular { get; set; }
    }
}
