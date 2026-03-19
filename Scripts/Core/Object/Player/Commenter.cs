using GameJam_URA.UI;
using UnityEngine;

namespace GameJam_URA
{
    public class Commenter : MonoBehaviour
    {
        public void OpenCommentView()
        {
            UIViewHub.Instance.Comment.SetActive(true);
        }

        public void Say(string comment)
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
        }
    }
}
