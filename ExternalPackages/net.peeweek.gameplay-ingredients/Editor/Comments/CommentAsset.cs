using GameplayIngredients.Editor;
using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Comments.Editor
{
    class CommentAsset : ScriptableObject
    {
        public Comment comment => m_Comment;

        [SerializeField]
        Comment m_Comment;

        [SerializeField, HideInInspector]
        public bool firstTimeEdit;

        private void Reset()
        {
            SetDefault();
            firstTimeEdit = true;
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

        [MenuItem("Assets/Create/Comment")]
        static void CreateAsset()
        {
            AssetFactory.CreateAssetInProjectWindow<CommentAsset>("Packages/net.peeweek.gameplay-ingredients/Icons/Misc/ic-comment.png", "New Comment.asset");
        }

    }
}

