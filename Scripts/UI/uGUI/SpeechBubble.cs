using System.Collections;
using MantenseiLib;
using TMPro;
using UnityEngine;

namespace GameJam_URA.UI
{
    public class SpeechBubble : MonoBehaviour
    {
        [GetComponent(HierarchyRelation.Children)]
        TextMeshProUGUI Label { get; set; }

        [GetComponent]
        CanvasGroup CanvasGroup { get; set; }

        public void Show(SpeechBubbleCommand command)
        {
            transform.SetParent(command.Parent, worldPositionStays: true);
            gameObject.SetActive(true);
            UIViewHub.Instance.StartCoroutine(ShowRoutine(command));
        }

        IEnumerator ShowRoutine(SpeechBubbleCommand command)
        {
            yield return null;
            transform.localPosition = command.Offset;
            transform.SetParent(null);
            Label.text = command.Text;
            CanvasGroup.alpha = 1f;

            yield return new WaitForSeconds(command.Duration);
            Destroy(gameObject);
        }
    }
}
