using System.Collections.Generic;
using UnityEngine;

namespace GameJam_URA
{
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
