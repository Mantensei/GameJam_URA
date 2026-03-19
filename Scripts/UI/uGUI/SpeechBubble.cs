using System.Collections;
using DG.Tweening;
using MantenseiLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameJam_URA.UI
{
    public partial class SpeechBubble : MonoBehaviour
    {
        [GetComponent(HierarchyRelation.Children)]
        TextMeshProUGUI Label { get; set; }

        [GetComponent(HierarchyRelation.Children)]
        Image Background { get; set; }

        [GetComponent]
        CanvasGroup CanvasGroup { get; set; }

        SpeechBubbleCommand command;

        public void Show(SpeechBubbleCommand command)
        {
            this.command = command;
            transform.SetParent(command.Parent, worldPositionStays: true);
            gameObject.SetActive(true);
            UIViewHub.Instance.StartCoroutine(ShowRoutine());
        }

        IEnumerator ShowRoutine()
        {
            yield return null;
            transform.localPosition = command.Offset;
            transform.SetParent(null);
            Label.text = command.Text;
            Label.color = command.TextColor;
            CanvasGroup.alpha = 1f;

            PlayEffect();
        }
    }

    public partial class SpeechBubble
    {
        float preDelay = 0.01f;
        float charInterval = 0.1f;
        float flyUpDistance = 0.5f;
        Vector2 padding = new Vector2(0.5f, 0.3f);

        void PlayEffect()
        {
            var bgRT = Background.rectTransform;
            bgRT.anchorMin = new Vector2(0.5f, 0.5f);
            bgRT.anchorMax = new Vector2(0.5f, 0.5f);
            bgRT.pivot = new Vector2(0.5f, 0.5f);

            Label.text = "　";
            Label.maxVisibleCharacters = int.MaxValue;
            FitBackground();

            float revealDuration = command.Text.Length * charInterval;
            float revealStart = preDelay;
            float flyStart = revealStart + revealDuration + command.Duration;

            DOTween.Sequence()
                .AppendInterval(revealStart)
                .AppendCallback(() =>
                {
                    Label.text = command.Text;
                    Label.maxVisibleCharacters = 0;
                })
                .Append(DOTween.To(
                    () => Label.maxVisibleCharacters,
                    x =>
                    {
                        Label.maxVisibleCharacters = x;
                        FitBackground();
                    },
                    command.Text.Length,
                    revealDuration
                ).SetEase(Ease.Linear));

            float totalLifetime = revealStart + revealDuration + command.Duration;
            transform.DOMoveY(transform.position.y + flyUpDistance, totalLifetime)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    CanvasGroup.alpha = 0f;
                    Destroy(gameObject);
                });
        }

        void FitBackground()
        {
            Label.ForceMeshUpdate();
            var size = Label.textBounds.size;
            if (size.sqrMagnitude < 0.001f) return;
            Background.rectTransform.sizeDelta = new Vector2(size.x + padding.x, size.y + padding.y);
        }
    }
}
