using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameJam_URA.Prototype
{
    public class CommentUI : MonoBehaviour
    {
        [SerializeField] GameObject commentPanel;
        [SerializeField] Transform buttonContainer;
        [SerializeField] Button commentButtonPrefab;

        bool isOpen;
        bool initialized;

        public bool IsOpen => isOpen;

        void Start()
        {
            commentPanel.SetActive(false);
        }

        public void Open()
        {
            isOpen = true;
            commentPanel.SetActive(true);
            if (!initialized) PopulateComments();
        }

        public void Close()
        {
            isOpen = false;
            commentPanel.SetActive(false);
        }

        void PopulateComments()
        {
            initialized = true;
            var stage = GameManager.Instance.CurrentStage;
            foreach (var comment in stage.AvailableComments)
                CreateCommentButton(comment);
        }

        void CreateCommentButton(string comment)
        {
            if (commentButtonPrefab == null) return;
            var btn = Instantiate(commentButtonPrefab, buttonContainer);
            var text = btn.GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = comment;

            btn.onClick.AddListener(() => OnCommentClicked(comment));
        }

        void OnCommentClicked(string comment)
        {
            GameManager.Instance.LogAction("comment:" + comment);

            var player = FindAnyObjectByType<RestaurantInputHandler>();
            if (player != null)
                SpeechBubble.GetOrCreate(player.transform.root).Show(comment);

            Close();
        }
    }
}
