using GameJam_URA.UI;
using UnityEngine;

namespace GameJam_URA
{
    public class Commenter : MonoBehaviour
    {
        void Start()
        {
            UIViewHub.Instance.Comment.OnCommentSelected += Say;
        }

        void OnDestroy()
        {
            UIViewHub.Instance.Comment.OnCommentSelected -= Say;
        }

        public void OpenCommentView()
        {
            UIViewHub.Instance.Comment.SetActive(true);
        }

        void Say(string comment)
        {
            var stage = GameManager.Instance.CurrentStage;
            foreach (var norma in stage.Normas)
            {
                if (norma is CommentNorma cn && cn.Comment.Text == comment)
                    norma.CompleteNorma();
            }
            foreach (var norma in stage.Dobons)
            {
                if (norma is CommentNorma cn && cn.Comment.Text == comment)
                    norma.CompleteNorma();
            }
            UIViewHub.Instance.SpeechBubble.Show(transform, comment);
            Debug.Log($"[感想] {comment}");
        }
    }
}
