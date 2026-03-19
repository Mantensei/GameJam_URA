using System;
using UnityEngine;

namespace GameJam_URA
{
    [Serializable]
    public class Comment
    {
        [SerializeField] string text;

        public string Text => text;
    }

    [CreateAssetMenu(fileName = "NewCommentNorma", menuName = "GameJam/CommentNorma")]
    public class CommentNorma : Norma
    {
        [SerializeField] Comment comment;

        public Comment Comment => comment;

        public override string ToString() => comment.Text;
    }
}
