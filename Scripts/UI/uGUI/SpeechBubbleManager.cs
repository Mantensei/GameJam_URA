using UnityEngine;

namespace GameJam_URA.UI
{
    public class SpeechBubbleCommand
    {
        public Transform Parent { get; set; }
        public string Text { get; set; }
        public float Duration { get; set; } = 1f;
        public Vector3 Offset { get; set; } = Vector2.up * 0.5f;
        public Color TextColor { get; set; } = Color.black;
        public int SortingOrder { get; set; }
    }

    public class SpeechBubbleManager : MonoBehaviour
    {
        [SerializeField] SpeechBubble prefab;

        static int sortingCounter;

        public void Show(Transform target, string text)
        {
            Show(new SpeechBubbleCommand
            {
                Parent = target,
                Text = text,
            });
        }

        public SpeechBubble Show(SpeechBubbleCommand command)
        {
            command.SortingOrder = sortingCounter++;
            var bubble = Instantiate(prefab, transform);
            bubble.Show(command);
            return bubble;
        }
    }
}
