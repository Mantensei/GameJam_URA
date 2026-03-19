using UnityEngine;

namespace GameJam_URA.UI
{
    public class SpeechBubbleCommand
    {
        public Transform Parent { get; set; }
        public string Text { get; set; }
        public float Duration { get; set; } = 2.5f;
        public Vector3 Offset { get; set; } = Vector2.up * 0.5f;
    }

    public class SpeechBubbleManager : MonoBehaviour
    {
        [SerializeField] SpeechBubble prefab;

        public void Show(Transform target, string text)
        {
            Show(new SpeechBubbleCommand
            {
                Parent = target,
                Text = text,
            });
        }

        public void Show(SpeechBubbleCommand command)
        {
            var bubble = Instantiate(prefab, transform);
            bubble.Show(command);
        }
    }
}
