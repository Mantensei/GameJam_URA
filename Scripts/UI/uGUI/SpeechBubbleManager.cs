using System.Collections.Generic;
using UnityEngine;

namespace GameJam_URA.UI
{
    public struct SpeechBubbleCommand
    {
        public Transform Target;
        public string Text;
        public float Duration;
        public Vector2 Offset;
    }

    public class SpeechBubbleManager : MonoBehaviour
    {
        [SerializeField] SpeechBubble prefab;
        [SerializeField] float defaultDuration = 2.5f;
        [SerializeField] Vector2 defaultOffset = new Vector2(0f, 1.5f);

        readonly List<SpeechBubble> pool = new List<SpeechBubble>();

        public void Show(Transform target, string text)
        {
            Show(new SpeechBubbleCommand
            {
                Target = target,
                Text = text,
                Duration = defaultDuration,
                Offset = defaultOffset,
            });
        }

        public void Show(SpeechBubbleCommand command)
        {
            if (command.Duration <= 0f) command.Duration = defaultDuration;
            if (command.Offset == Vector2.zero) command.Offset = defaultOffset;
            var bubble = GetOrCreate();
            bubble.Show(command);
        }

        SpeechBubble GetOrCreate()
        {
            foreach (var b in pool)
                if (!b.IsActive) return b;

            var bubble = Instantiate(prefab, transform);
            bubble.gameObject.SetActive(false);
            pool.Add(bubble);
            return bubble;
        }
    }
}
