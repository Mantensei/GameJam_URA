using System;
using System.Collections.Generic;
using MantenseiLib;
using UnityEngine;

namespace GameJam_URA
{
    [CreateAssetMenu(fileName = "NewStage", menuName = "GameJam/StageData")]
    public class StageData : ScriptableObject
    {
        [SerializeField] StageId stageId;
        [SerializeField] float timeLimit = 60f;
        [SerializeField] int requiredOrders = 3;
        [SerializeField] URA_PlayerReferenceHub[] customerPrefabs;
        [SerializeField, Range(0f, 1f)] float dobonRate = 0.3f;

        int initialMoney => 1000;
        NormaTable normaData => default;
        int normaCount => 3;
        NormaTable dobonData => default;
        int commentNormaCount => 1;
        int commentDobonCount => 1;
        CustomerTable customerTable => default;
        int regularCount => 1;
        int dobonCount => Mathf.RoundToInt(MenuList.Count * dobonRate);

        public StageId Id => stageId;
        public int InitialMoney => initialMoney;
        public float TimeLimit => timeLimit;
        public int DobonPenalty => stageId.DobonPenalty();
        public int DobonCount => dobonCount;
        public int RequiredOrders => requiredOrders;
        public List<ICommentItem> CommentList => builtCommentList;
        public URA_PlayerReferenceHub[] CustomerPrefabs => customerPrefabs;

        List<IUraTask> builtNormas;
        List<IUraTask> builtDobons;
        List<IDishItem> builtMenuList;
        List<ICommentItem> builtCommentList;

        public List<IUraTask> Normas => builtNormas;
        public List<IUraTask> Dobons => builtDobons;

        public void ResetMenu() => builtMenuList = null;

        public List<IDishItem> MenuList
        {
            get
            {
                if (builtMenuList != null) return builtMenuList;
                builtMenuList = new List<IDishItem>(StageDataLoader.LoadMenus(stageId));
                foreach (var item in builtMenuList) item.TaskType = UraTaskType.None;
                var indices = new List<int>();
                for (int i = 0; i < builtMenuList.Count; i++) indices.Add(i);
                indices = new List<int>(indices.Shuffle());
                int count = Mathf.Min(dobonCount, indices.Count);
                for (int i = 0; i < count; i++)
                    builtMenuList[indices[i]].TaskType = UraTaskType.Dobon;
                return builtMenuList;
            }
        }
        [Obsolete("仮データプロパティで代替中。再利用時にSerializeFieldを復元すること")]
        public void Init()
        {
            var customers = new List<CustomerData>();
            customerTable.GetAllCustomerData(customers);
            AssignRegulars(customers);

            builtNormas = new List<IUraTask>();
            builtDobons = new List<IUraTask>();

            builtMenuList = new List<IDishItem>(StageDataLoader.LoadMenus(stageId).Shuffle());
            int menuNormaEnd = Mathf.Min(normaCount, builtMenuList.Count);
            int menuDobonEnd = Mathf.Min(menuNormaEnd + dobonCount, builtMenuList.Count);
            for (int i = 0; i < menuNormaEnd; i++)
                builtNormas.Add(builtMenuList[i]);
            for (int i = menuNormaEnd; i < menuDobonEnd; i++)
                builtDobons.Add(builtMenuList[i]);

            builtCommentList = new List<ICommentItem>(StageDataLoader.LoadComments(stageId).Shuffle());
            int commentNormaEnd = Mathf.Min(commentNormaCount, builtCommentList.Count);
            int commentDobonEnd = Mathf.Min(commentNormaEnd + commentDobonCount, builtCommentList.Count);
            for (int i = 0; i < commentNormaEnd; i++)
                builtNormas.Add(builtCommentList[i]);
            for (int i = commentNormaEnd; i < commentDobonEnd; i++)
                builtDobons.Add(builtCommentList[i]);

            BuildNonMenuTasks();
        }

        void BuildNonMenuTasks()
        {
            var normaPool = new List<IUraTask>();
            normaData.GetAllTasks(normaPool);

            var usedNames = new HashSet<string>();
            foreach (var task in builtNormas) usedNames.Add(task.Name);
            foreach (var task in builtDobons) usedNames.Add(task.Name);

            normaPool.RemoveAll(t => usedNames.Contains(t.Name) || t is IDishItem);
            normaPool = new List<IUraTask>(normaPool.Shuffle());
            builtNormas.AddRange(normaPool);

            var dobonPool = new List<IUraTask>();
            dobonData.GetAllTasks(dobonPool);

            foreach (var task in normaPool) usedNames.Add(task.Name);
            dobonPool.RemoveAll(t => usedNames.Contains(t.Name) || t is IDishItem);
            dobonPool = new List<IUraTask>(dobonPool.Shuffle());
            builtDobons.AddRange(dobonPool);
        }

        void AssignRegulars(List<CustomerData> customers)
        {
            customers = new List<CustomerData>(customers.Shuffle());
            int count = Mathf.Min(regularCount, customers.Count);
            for (int i = 0; i < customers.Count; i++)
                customers[i].IsRegular = i < count;
        }
    }
}
