using System.Collections.Generic;
using UnityEngine;

namespace GameJam_URA.Prototype
{
    public class CustomerSpawner : MonoBehaviour
    {
        [SerializeField] GameObject customerPrefab;

        Seat[] seats;
        Transform shopkeeperPoint;
        EavesdropSystem eavesdrop;
        List<GameObject> spawnedCustomers = new List<GameObject>();

        void Start()
        {
            seats = FindObjectsByType<Seat>(FindObjectsSortMode.None);
            var shopkeeper = FindAnyObjectByType<Shopkeeper>();
            if (shopkeeper != null)
                shopkeeperPoint = shopkeeper.transform;
            eavesdrop = FindAnyObjectByType<EavesdropSystem>();

            GameManager.Instance.OnStateChanged += OnStateChanged;

            if (GameManager.Instance.State == GameState.Visiting)
                SpawnCustomers();
        }

        void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= OnStateChanged;
        }

        void OnStateChanged(GameState state)
        {
            if (state == GameState.Visiting)
                SpawnCustomers();
        }

        void SpawnCustomers()
        {
            ClearCustomers();

            var customers = GameManager.Instance.CurrentStage.Customers;
#if UNITY_EDITOR
            MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "Spawner", "SpawnCustomers count=" + customers.Length + " seats=" + seats.Length);
#endif

            for (int i = 0; i < customers.Length; i++)
            {
                var go = Instantiate(customerPrefab, transform.position, Quaternion.identity);
                spawnedCustomers.Add(go);

                var ai = go.GetComponentInChildren<CustomerAI>();
                Seat seat = seats[i % seats.Length];
#if UNITY_EDITOR
                MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "Spawner", "Spawn: " + customers[i].CustomerName + " → " + seat.gameObject.name);
#endif
                ai.Initialize(customers[i], seat, transform, shopkeeperPoint);

                if (eavesdrop != null)
                    eavesdrop.RegisterCustomer(ai);
            }
        }

        void ClearCustomers()
        {
            foreach (var go in spawnedCustomers)
            {
                if (go != null)
                    Destroy(go);
            }
            spawnedCustomers.Clear();
        }
    }
}
