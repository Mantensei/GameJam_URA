using UnityEngine;

namespace GameJam_URA
{
    [CreateAssetMenu(fileName = "NewCommentNorma", menuName = "GameJam/CommentNorma")]
    public class CommentNorma : Norma
    {
        [SerializeField] Comment comment;

        public Comment Comment => comment;
    }
}
