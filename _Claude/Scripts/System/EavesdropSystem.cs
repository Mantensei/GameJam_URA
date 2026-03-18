using UnityEngine;

namespace GameJam_URA.Prototype
{
    public class EavesdropSystem : MonoBehaviour
    {
        // [Legacy] RestaurantInputHandler依存のため一時無効化
        /*
        [SerializeField] GameObject eavesdropPanel;
        [SerializeField] float displayDuration = 3f;

        TMP_Text eavesdropText;
        float hideTimer;

        void Start()
        {
            if (eavesdropPanel != null)
            {
                eavesdropText = eavesdropPanel.GetComponentInChildren<TMP_Text>(true);
                eavesdropPanel.SetActive(false);
            }

            var customers = FindObjectsByType<CustomerAI>(FindObjectsSortMode.None);
            foreach (var customer in customers)
                customer.OnWhisperStart += OnCustomerWhisper;
        }

        public void RegisterCustomer(CustomerAI customer)
        {
            customer.OnWhisperStart += OnCustomerWhisper;
        }

        void OnCustomerWhisper(CustomerAI customer)
        {
            if (!customer.TimelineData.IsRegular) return;

            var player = FindAnyObjectByType<RestaurantInputHandler>();
            bool sitting = player != null && player.IsSitting;
#if UNITY_EDITOR
            MantenseiLib.Editor.DebugFileLogger.Log("gameplay", "Eavesdrop", "Whisper: " + customer.TimelineData.CustomerName + " playerSitting=" + sitting);
#endif
            if (player == null || !player.IsSitting) return;

            string secretMenuName = "";
            foreach (var item in customer.TimelineData.OrderItems)
            {
                if (!item.IsSecretMenu) continue;
                secretMenuName = item.ItemName;
                break;
            }

            if (string.IsNullOrEmpty(secretMenuName)) return;

            ShowEavesdropInfo(customer.TimelineData.CustomerName + "「" + secretMenuName + "をください…」");
        }

        void ShowEavesdropInfo(string text)
        {
            if (eavesdropPanel == null) return;
            eavesdropText.text = text;
            eavesdropPanel.SetActive(true);
            hideTimer = displayDuration;
        }

        void Update()
        {
            if (hideTimer <= 0f) return;
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f && eavesdropPanel != null)
                eavesdropPanel.SetActive(false);
        }
        */
    }
}
