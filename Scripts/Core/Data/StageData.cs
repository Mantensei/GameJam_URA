using System.Collections.Generic;
using MantenseiLib;
using UnityEngine;

namespace GameJam_URA
{
    [CreateAssetMenu(fileName = "NewStage", menuName = "GameJam/StageData")]
    public class StageData : ScriptableObject
    {
        [SerializeField] StageId stageId;
        [SerializeField] int initialMoney;
        [SerializeField] NormaTable normaData;
        [SerializeField] int normaCount;
        [SerializeField] NormaTable dobonData;
        [SerializeField] int dobonCount;
        [SerializeField] int commentNormaCount;
        [SerializeField] int commentDobonCount;
        [SerializeField] float timeLimit = 180f;
        [SerializeField] CustomerTable customerTable;
        [SerializeField] int regularCount = 1;
        public StageId Id => stageId;
        public int InitialMoney => initialMoney;
        public float TimeLimit => timeLimit;
        public int DobonPenalty => stageId.DobonPenalty();
        public List<ICommentItem> CommentList => builtCommentList;
        public List<CustomerData> Customers => builtCustomers;

        List<CustomerData> builtCustomers;

        List<IUraTask> builtNormas;
        List<IUraTask> builtDobons;
        List<IDishItem> builtMenuList;
        List<ICommentItem> builtCommentList;

        public List<IUraTask> Normas => builtNormas;
        public List<IUraTask> Dobons => builtDobons;
        public List<IDishItem> MenuList => builtMenuList;
        public int CustomerBudget { get; private set; }

        public void Init()
        {
            builtCustomers = new List<CustomerData>();
            customerTable.GetAllCustomerData(builtCustomers);
            AssignRegulars();

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
            CalcCustomerBudget();
        }

        void CalcCustomerBudget()
        {
            int normaTotal = 0;
            foreach (var task in builtNormas)
                if (task is IDishItem dish)
                    normaTotal += dish.Price;
            int margin = 000;
            int unit = 500;
            CustomerBudget = (normaTotal + margin + unit - 1) / unit * unit;
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

        void AssignRegulars()
        {
            builtCustomers = new List<CustomerData>(builtCustomers.Shuffle());
            int count = Mathf.Min(regularCount, builtCustomers.Count);
            for (int i = 0; i < builtCustomers.Count; i++)
                builtCustomers[i].IsRegular = i < count;
        }
    }
}
