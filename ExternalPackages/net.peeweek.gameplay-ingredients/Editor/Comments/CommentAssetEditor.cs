using UnityEditor;
using UnityEngine;

namespace GameplayIngredients.Comments.Editor
{
    [CustomEditor(typeof(CommentAsset))]
    public class CommentAssetEditor : UnityEditor.Editor
    {
        [SerializeField]
        CommentAsset commentAsset;
        [SerializeField]
        SerializedProperty m_Comment;

        [SerializeField]
        CommentEditor m_CommentEditor;

        public static void RequestEdit() { m_NeedEditNext = true; }
        public static bool m_NeedEditNext = false;

        private void OnEnable()
        {
            UpdateComment();
            if (commentAsset.comment.focus)
                EditorGUIUtility.PingObject(commentAsset);
        }

        void UpdateComment()
        {
            if (m_Comment == null)
                m_Comment = serializedObject.FindProperty("m_Comment");

            if (m_CommentEditor == null)
                m_CommentEditor = new CommentEditor(serializedObject, m_Comment, true);

            commentAsset = (serializedObject.targetObject as CommentAsset);

            if (commentAsset.firstTimeEdit)
                RequestEdit();
        }

        public override void OnInspectorGUI()
        {
            UpdateComment();
            GUILayout.Space(4);
            m_CommentEditor.DrawComment(commentAsset.comment, m_NeedEditNext);

            if (m_NeedEditNext)
            {
                m_NeedEditNext = false;
                commentAsset.firstTimeEdit = false;
            }

            GUILayout.Space(16);
        }

    }
}
