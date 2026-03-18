using System;
using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;

namespace GameJam_URA.Prototype
{
    public class OrderSystem : SingletonMonoBehaviour<OrderSystem>
    {
        //         [SerializeField] FoodPlate foodPlatePrefab;
        //         [SerializeField] Transform plateSpawnPoint;
        //         [SerializeField] float cookingTime = 3f;

        //         List<OrderEntry> pendingOrders = new List<OrderEntry>();

        //         public event Action<MenuItemData> OnOrderPlaced;
        //         public event Action<MenuItemData> OnFoodReady;

        //         public bool PlaceOrder(MenuItemData item, Seat seat)
        //         {
        //             var gm = GameManager.Instance;
        //             if (!gm.SpendMoney(item.Price)) return false;
        // #if UNITY_EDITOR
        //             MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "Order", "PlaceOrder: " + item.ItemName + " price=" + item.Price);
        // #endif

        //             gm.LogAction("order:" + item.ItemName);
        //             OnOrderPlaced?.Invoke(item);

        //             pendingOrders.Add(new OrderEntry
        //             {
        //                 item = item,
        //                 seat = seat,
        //                 remainingTime = cookingTime
        //             });
        //             return true;
        //         }

        //         void Update()
        //         {
        //             for (int i = pendingOrders.Count - 1; i >= 0; i--)
        //             {
        //                 pendingOrders[i].remainingTime -= Time.deltaTime;
        //                 if (pendingOrders[i].remainingTime <= 0f)
        //                 {
        //                     SpawnFoodPlate(pendingOrders[i]);
        //                     pendingOrders.RemoveAt(i);
        //                 }
        //             }
        //         }

        //         void SpawnFoodPlate(OrderEntry entry)
        //         {
        //             if (foodPlatePrefab == null) return;

        //             Vector3 spawnPos = plateSpawnPoint != null
        //                 ? plateSpawnPoint.position
        //                 : entry.seat.transform.position + Vector3.up * 0.5f;

        //             var plate = Instantiate(foodPlatePrefab, spawnPos, Quaternion.identity);
        //             plate.Setup(entry.item);
        // #if UNITY_EDITOR
        //             MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "Order", "FoodReady: " + entry.item.ItemName);
        // #endif
        //             OnFoodReady?.Invoke(entry.item);
        //         }

        //         class OrderEntry
        //         {
        //             public MenuItemData item;
        //             public Seat seat;
        //             public float remainingTime;
        //         }
    }
}
