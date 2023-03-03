using UnityEditor;
using UnityEngine;

namespace GameplayIngredients.Comments.Editor
{
    [CustomEditor(typeof(SceneComment))]
    public class SceneCommentEditor : UnityEditor.Editor
    {
        [SerializeField]
        SceneComment sceneComment;
        [SerializeField]
        SerializedProperty m_Comment;

        [SerializeField]
        CommentEditor m_CommentEditor;


        public static void RequestEdit() { m_NeedEditNext = true; }
        public static bool m_NeedEditNext = false;


        private void OnEnable()
        {
            UpdateComment();
            if(sceneComment.comment.focus && SceneView.lastActiveSceneView != null)
                SceneView.lastActiveSceneView.AlignViewToObject(sceneComment.transform);
        }

        void UpdateComment()
        {
            if (m_Comment == null)
                m_Comment = serializedObject.FindProperty("m_Comment");

            if (m_CommentEditor == null)
                m_CommentEditor = new CommentEditor(serializedObject, m_Comment);

            sceneComment = (serializedObject.targetObject as SceneComment);
        }

        public override void OnInspectorGUI()
        {
            UpdateComment();
            GUILayout.Space(4);
            m_CommentEditor.DrawComment(sceneComment.comment, m_NeedEditNext);

            if (m_NeedEditNext)
                m_NeedEditNext = false;

            GUILayout.Space(16);
        }

        [MenuItem("GameObject/Gameplay Ingredients/Comment", false, 10)]
        public static void CreateComment()
        {
            var go = new GameObject("Comment", typeof(SceneComment));
            if (Selection.activeGameObject != null && Selection.activeGameObject.scene != null)
            {
                go.transform.parent = Selection.activeGameObject.transform;  
            }

            if (SceneView.lastActiveSceneView != null)
            {
                var cam = SceneView.lastActiveSceneView.camera;
                go.transform.position = cam.transform.position;
                go.transform.rotation = cam.transform.rotation;
            }

            Selection.activeGameObject = go;
            go.GetComponent<SceneComment>().SetDefault();
            SceneCommentEditor.RequestEdit();
        }
    }
}
