using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameJam_URA.Prototype
{
    public class JudgmentUI : MonoBehaviour
    {
        [SerializeField] GameObject judgmentPanel;
        [SerializeField] Button continueButton;

        TMP_Text resultText;
        TMP_Text detailText;
        bool lastCleared;

        void Start()
        {
            var texts = judgmentPanel.GetComponentsInChildren<TMP_Text>(true);
            if (texts.Length >= 2)
            {
                resultText = texts[0];
                detailText = texts[1];
            }

            judgmentPanel.SetActive(false);
            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinue);
        }

        public void ShowResult(bool cleared, int normaCount, int normaTotal, int penalty, List<string> dobons)
        {
            lastCleared = cleared;
            judgmentPanel.SetActive(true);

            if (cleared)
            {
                resultText.text = "裏メニューを教えます";
            }
            else
            {
                resultText.text = "帰んなさい";
            }

            string detail = "ノルマ: " + normaCount + "/" + normaTotal;
            if (penalty > 0)
                detail += "\nドボン罰金: -" + penalty + "円";
            if (dobons.Count > 0)
            {
                detail += "\nドボン: ";
                for (int i = 0; i < dobons.Count; i++)
                {
                    if (i > 0) detail += ", ";
                    detail += dobons[i];
                }
            }
            detailText.text = detail;
        }

        void OnContinue()
        {
            judgmentPanel.SetActive(false);
            var gm = GameManager.Instance;

            if (lastCleared)
            {
                gm.SetState(GameState.Clear);
                return;
            }

            gm.SetState(GameState.Result);
            gm.OnVisitEnd();
        }
    }
}
