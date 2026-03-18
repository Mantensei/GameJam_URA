using System.Collections.Generic;
using UnityEngine;

namespace GameJam_URA
{
    public enum StageId
    {
        Tutorial,
        Stage1,
        Stage2,
        Stage3,
    }

    public static class StageIdExtensions
    {
        public static int DobonPenalty(this StageId id) => id switch
        {
            StageId.Tutorial => 500,
            StageId.Stage1   => 500,
            StageId.Stage2   => 500,
            StageId.Stage3   => 500,
            _                => 500,
        };

        static readonly string[] comments = { "うまい", "量が多い", "また来る", "まずい" };
        public static string[] Comments => comments;
    }

    [CreateAssetMenu(fileName = "NewStage", menuName = "GameJam/StageData")]
    public class StageData : ScriptableObject
    {
        [SerializeField] StageId stageId;
        [SerializeField] int initialMoney;
        [SerializeField] NormaData normaData;
        [SerializeField] int normaCount;
        [SerializeField] NormaData dobonData;
        [SerializeField] int dobonCount;
        [SerializeField] int menuCount;
        [SerializeField] CustomerTimelineData customerTimelineData;

        public StageId Id => stageId;
        public int InitialMoney => initialMoney;
        public int DobonPenalty => stageId.DobonPenalty();
        public string[] Comments => StageIdExtensions.Comments;
        public CustomerTimelineData CustomerTimelineData => customerTimelineData;

        public void BuildStageData(out List<Norma> normas, out List<Norma> dobons, out List<MenuNorma> menuList)
        {
            var normaPool = new List<Norma>();
            normaData.GetAllNormas(normaPool);
            normas = PickRandom(normaPool, normaCount);

            var usedNames = new HashSet<string>();
            foreach (var n in normas)
                usedNames.Add(n.name);

            var dobonPool = new List<Norma>();
            dobonData.GetAllNormas(dobonPool);
            dobonPool.RemoveAll(d => usedNames.Contains(d.name));
            dobons = PickRandom(dobonPool, dobonCount);

            foreach (var d in dobons)
                usedNames.Add(d.name);

            menuList = new List<MenuNorma>();
            foreach (var n in normas)
                if (n is MenuNorma mn) menuList.Add(mn);
            foreach (var d in dobons)
                if (d is MenuNorma mn) menuList.Add(mn);

            var dummyPool = new List<Norma>();
            normaData.GetAllNormas(dummyPool);
            dobonData.GetAllNormas(dummyPool);
            dummyPool.RemoveAll(d => usedNames.Contains(d.name));

            var dummyMenus = new List<MenuNorma>();
            foreach (var d in dummyPool)
                if (d is MenuNorma mn) dummyMenus.Add(mn);

            int dummyNeeded = menuCount - menuList.Count;
            if (dummyNeeded > 0)
            {
                var picked = PickRandom(new List<Norma>(dummyMenus), dummyNeeded);
                foreach (var p in picked)
                    menuList.Add((MenuNorma)p);
            }

            Shuffle(menuList);
        }

        static List<Norma> PickRandom(List<Norma> pool, int count)
        {
            Shuffle(pool);
            if (count >= pool.Count) return pool;
            pool.RemoveRange(count, pool.Count - count);
            return pool;
        }

        static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
