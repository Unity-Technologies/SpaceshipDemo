using UnityEngine;

namespace GameplayIngredients.Comments
{
    [AddComponentMenu(ComponentMenu.basePath + "Comment")]
    public class SceneComment : MonoBehaviour
    {
#if UNITY_EDITOR
        public Comment comment => m_Comment;
        [SerializeField]
        Comment m_Comment;
        private void Reset()
        {
            m_Comment.message.from = Comment.currentUser; 
            transform.hideFlags = HideFlags.HideInInspector;
        }

        public void SetDefault()
        {
            m_Comment.title = "New Comment";
            m_Comment.message.body = "This is a new Comment, it can describe a problem in the scene, a note to the attention of other user, or a bug encountered.";
            m_Comment.message.from = Comment.currentUser;
            m_Comment.message.type = CommentType.Info;
            m_Comment.message.priority = CommentPriority.Low;
            m_Comment.message.state = CommentState.Open;
        }
#endif
    }
}
