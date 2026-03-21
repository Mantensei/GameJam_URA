using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MantenseiLib;
using UnityEngine;

namespace GameJam_URA
{
    public class CustomerSpawner : MonoBehaviour
    {
        StageData stageData => GameManager.Instance.CurrentStage;

        float elapsed;
        float judgeTimer;
        float judgeInterval = 1f;
        float maxSpawnPercent = 0.5f;

        void Start()
        {
            StartCoroutine(LinearSpawn());
        }

        void Update()
        {
            elapsed += Time.deltaTime;
            judgeTimer += Time.deltaTime;
            if (judgeTimer < judgeInterval) return;
            judgeTimer = 0f;

            float t = Mathf.Clamp01(elapsed / stageData.TimeLimit);
            float chance = t * (maxSpawnPercent / 100f);

            if (Random.value < chance)
                SpawnRandom();
        }

        IEnumerator LinearSpawn()
        {
            var menuList = new List<IDishItem>(stageData.MenuList.Shuffle());
            float interval = stageData.TimeLimit / menuList.Count;

            foreach (var dish in menuList)
            {
                SpawnCustomer(dish);
                yield return new WaitForSeconds(interval);
            }
        }

        void SpawnRandom()
        {
            var menuList = stageData.MenuList;
            var dish = menuList[Random.Range(0, menuList.Count)];
            SpawnCustomer(dish);
        }

        void SpawnCustomer(IDishItem dish)
        {
            var prefabs = stageData.CustomerPrefabs;
            var prefab = prefabs[Random.Range(0, prefabs.Length)];
            bool isDobon = dish.TaskType == UraTaskType.Dobon;

            var spawnPoint = StageObjectHub.Instance.Interactables
                .Where(x => x is Exit || x is Toilet)
                .GetRandomElementOrDefault();
            var instance = Instantiate(prefab, spawnPoint.transform.position, Quaternion.identity);
            var ai = instance.GetComponentInChildren<CustomerMineAI>();
            ai.Setup(dish, isDobon);
        }
    }
}
