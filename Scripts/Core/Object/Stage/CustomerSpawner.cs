using System.Collections;
using UnityEngine;
using System.Linq;
using MantenseiLib;
using System;

namespace GameJam_URA
{
    public class CustomerSpawner : MonoBehaviour
    {
        StageData stageData => GameManager.Instance.CurrentStage;

        void Start()
        {
            BuildSchedule();
        }

        void BuildSchedule()
        {
            float limit = stageData.TimeLimit;
            var customers = stageData.Customers.Shuffle().ToArray();
            for (int i = 0; i < customers.Length; i++)
            {
                var customer = customers[i];
                //最初の一人は必ず即座に出現させる
                float time = i == 0 ? 0f : GetRandomValue(limit);
                Spawn(customer, time);
            }
        }

        float GetRandomValue(float value)
        {
            return MathF.Max(0f, UnityEngine.Random.Range(0f, value) - value / 2);
        }

        void Spawn(CustomerData data, float delay)
        {
            StartCoroutine(SpawnDelayed(data, delay));
        }

        IEnumerator SpawnDelayed(CustomerData data, float delay)
        {
            yield return new WaitForSeconds(delay);

            var spawnPoint = StageObjectHub.Instance.Interactables
                .Where(x => x is Exit || x is Toilet)
                .GetRandomElementOrDefault();
            var instance = Instantiate(data.Prefab, spawnPoint.transform.position, Quaternion.identity);
            var ai = instance.GetComponentInChildren<CustomerAI>();
            ai.Setup(data, stageData);
        }
    }
}
