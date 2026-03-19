using System.Collections.Generic;
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

    [CreateAssetMenu(fileName = "NewCustomerTable", menuName = "GameJam/CustomerTable")]
    public class CustomerTable : ScriptableObject
    {
        [SerializeField] CustomerData[] customers;

        public void GetAllCustomerData(List<CustomerData> result)
        {
            foreach (var customer in customers)
                result.Add(Instantiate(customer));
        }
    }
}
