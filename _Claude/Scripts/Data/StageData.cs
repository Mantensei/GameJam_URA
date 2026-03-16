using System;
using UnityEngine;

namespace GameJam_URA.Prototype
{
    [CreateAssetMenu(fileName = "NewStage", menuName = "GameJam/StageData")]
    public class StageData : ScriptableObject
    {
        [SerializeField] string stageName;
        [SerializeField] int initialMoney;
        [SerializeField] MenuItemData[] availableMenuItems;
        [SerializeField] string[] normaConditions;
        [SerializeField] DobonEntry[] dobonEntries;
        [SerializeField] CustomerTimelineData[] customers;
        [SerializeField] string[] availableComments;

        public string StageName => stageName;
        public int InitialMoney => initialMoney;
        public MenuItemData[] AvailableMenuItems => availableMenuItems;
        public string[] NormaConditions => normaConditions;
        public DobonEntry[] DobonEntries => dobonEntries;
        public CustomerTimelineData[] Customers => customers;
        public string[] AvailableComments => availableComments;
    }

    [Serializable]
    public class DobonEntry
    {
        public string conditionId;
        public int penalty;
    }
}
