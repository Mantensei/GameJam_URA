using MantenseiLib;
using TMPro;
using UnityEngine;

namespace GameJam_URA.UI
{
    public class SpeechBubble : MonoBehaviour
    {
        [GetComponent(HierarchyRelation.Children)]
        TMP_Text Label { get; set; }

        [GetComponent]
        CanvasGroup CanvasGroup { get; set; }

        float timer;
        Transform target;
        Vector2 offset;

        public bool IsActive => timer > 0f;

        public void Show(SpeechBubbleCommand command)
        {
            target = command.Target;
            offset = command.Offset;
            timer = command.Duration;
            Label.text = command.Text;
            CanvasGroup.alpha = 1f;
            gameObject.SetActive(true);
            FollowTarget();
        }

        public void Release()
        {
            timer = 0f;
            gameObject.SetActive(false);
        }

        void Update()
        {
            if (timer <= 0f) return;
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                Release();
                return;
            }
            FollowTarget();
        }

        void FollowTarget()
        {
            if (target == null) return;
            transform.position = (Vector2)target.position + offset;
        }
    }
}
