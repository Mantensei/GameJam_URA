using TMPro;
using UnityEngine;

namespace GameJam_URA.Prototype
{
    public class MoneyUI : MonoBehaviour
    {
        TMP_Text moneyText;

        void Start()
        {
            moneyText = GetComponentInChildren<TMP_Text>();
            GameManager.Instance.OnMoneyChanged += UpdateDisplay;
            UpdateDisplay(GameManager.Instance.Money);
        }

        void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnMoneyChanged -= UpdateDisplay;
        }

        void UpdateDisplay(int amount)
        {
            if (moneyText != null)
                moneyText.text = amount + "円";
        }
    }
}
