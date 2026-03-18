using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GameJam_URA.Prototype
{
    public class JudgmentSystem : MonoBehaviour
    {
        /*
        public void ExecuteJudgment()
        {
            var gm = GameManager.Instance;
            var stage = gm.CurrentStage;
            var log = gm.ActionLog;

            int normaCount = 0;
            int normaTotal = stage.NormaConditions.Length;

            foreach (var norma in stage.NormaConditions)
            {
                if (log.Contains(norma))
                    normaCount++;
            }

            int totalPenalty = 0;
            List<string> triggeredDobons = new List<string>();
            foreach (var dobon in stage.DobonEntries)
            {
                if (log.Contains(dobon.conditionId))
                {
                    totalPenalty += dobon.penalty;
                    triggeredDobons.Add(dobon.conditionId);
                }
            }

            if (totalPenalty > 0)
                gm.DeductPenalty(totalPenalty);

            bool cleared = normaCount >= normaTotal;

            var judgmentUI = FindAnyObjectByType<JudgmentUI>();
            if (judgmentUI != null)
                judgmentUI.ShowResult(cleared, normaCount, normaTotal, totalPenalty, triggeredDobons);
        }
        */
    }
}
