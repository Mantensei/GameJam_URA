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
            StageId.Stage1 => 500,
            StageId.Stage2 => 500,
            StageId.Stage3 => 500,
            _ => 500,
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

        List<Norma> builtNormas;
        List<Norma> builtDobons;
        List<MenuNorma> builtMenuList;

        public List<Norma> Normas => builtNormas;
        public List<Norma> Dobons => builtDobons;
        public List<MenuNorma> MenuList => builtMenuList;

        public void Init()
        {
            var normaPool = new List<Norma>();
            normaData.GetAllNormas(normaPool);
            builtNormas = PickRandom(normaPool, normaCount);

            var usedNames = new HashSet<string>();
            foreach (var n in builtNormas)
                usedNames.Add(n.name);

            var dobonPool = new List<Norma>();
            dobonData.GetAllNormas(dobonPool);
            dobonPool.RemoveAll(d => usedNames.Contains(d.name));
            builtDobons = PickRandom(dobonPool, dobonCount);

            foreach (var d in builtDobons)
                usedNames.Add(d.name);

            builtMenuList = new List<MenuNorma>();
            foreach (var n in builtNormas)
                if (n is MenuNorma mn) builtMenuList.Add(mn);
            foreach (var d in builtDobons)
                if (d is MenuNorma mn) builtMenuList.Add(mn);

            var dummyPool = new List<Norma>();
            normaData.GetAllNormas(dummyPool);
            dobonData.GetAllNormas(dummyPool);
            dummyPool.RemoveAll(d => usedNames.Contains(d.name));

            var dummyMenus = new List<MenuNorma>();
            foreach (var d in dummyPool)
                if (d is MenuNorma mn) dummyMenus.Add(mn);

            int dummyNeeded = menuCount - builtMenuList.Count;
            if (dummyNeeded > 0)
            {
                var picked = PickRandom(new List<Norma>(dummyMenus), dummyNeeded);
                foreach (var p in picked)
                    builtMenuList.Add((MenuNorma)p);
            }

            Shuffle(builtMenuList);
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
