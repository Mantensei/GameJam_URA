using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameJam_URA.Prototype
{
    public class SpeechBubble : MonoBehaviour
    {
        [SerializeField] GameObject bubbleObject;
        [SerializeField] TMP_Text bubbleText;
        [SerializeField] float displayDuration = 2f;

        float hideTimer;

        void Start()
        {
            bubbleObject.SetActive(false);
        }

        void Update()
        {
            if (hideTimer <= 0f) return;
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f)
                Hide();
        }

        public void Show(string text)
        {
            bubbleText.text = text;
            bubbleObject.SetActive(true);
            hideTimer = displayDuration;
        }

        public void Hide()
        {
            bubbleObject.SetActive(false);
        }

        public static SpeechBubble GetOrCreate(Transform target)
        {
            var existing = target.GetComponentInChildren<SpeechBubble>();
            if (existing != null) return existing;

            var rootGo = new GameObject("SpeechBubbleRoot");
            rootGo.transform.SetParent(target);
            rootGo.transform.localPosition = Vector3.zero;

            var canvasGo = new GameObject("SpeechBubbleCanvas");
            canvasGo.transform.SetParent(rootGo.transform);
            canvasGo.transform.localPosition = new Vector3(0, 1.5f, 0);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 10;
            var rt = canvasGo.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(200, 60);
            rt.localScale = new Vector3(0.01f, 0.01f, 1f);

            var bgGo = new GameObject("BubbleBG");
            bgGo.transform.SetParent(canvasGo.transform, false);
            var bgImage = bgGo.AddComponent<Image>();
            bgImage.color = new Color(1f, 1f, 1f, 0.9f);
            var bgRt = bgGo.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.offsetMin = Vector2.zero;
            bgRt.offsetMax = Vector2.zero;

            var textGo = new GameObject("BubbleText");
            textGo.transform.SetParent(canvasGo.transform, false);
            var bubText = textGo.AddComponent<TextMeshProUGUI>();
            bubText.font = TMP_Settings.defaultFontAsset;
            bubText.fontSize = 24;
            bubText.alignment = TextAlignmentOptions.Center;
            bubText.color = Color.black;
            var textRt = textGo.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = new Vector2(5, 5);
            textRt.offsetMax = new Vector2(-5, -5);

            var bubble = rootGo.AddComponent<SpeechBubble>();
            bubble.bubbleObject = canvasGo;
            bubble.bubbleText = bubText;
            canvasGo.SetActive(false);

            return bubble;
        }
    }
}
