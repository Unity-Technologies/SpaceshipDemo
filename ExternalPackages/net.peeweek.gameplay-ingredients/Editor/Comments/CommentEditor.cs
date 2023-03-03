using GameplayIngredients.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace GameplayIngredients.Comments.Editor
{
    public class CommentEditor
    {
        SerializedObject serializedObject;
        SerializedProperty rootMessage;
        SerializedProperty replies;
        SerializedProperty title;
        SerializedProperty focus;
        string editMessagePath;

        bool editRoot => editMessagePath == rootMessage.propertyPath;
        bool m_IsAsset;
        public CommentEditor(SerializedObject serializedObject, SerializedProperty comment, bool isAsset = false)
        {
            this.serializedObject = serializedObject;
            title = comment.FindPropertyRelative("title");
            rootMessage = comment.FindPropertyRelative("message");
            replies = comment.FindPropertyRelative("replies");
            focus = comment.FindPropertyRelative("focus");
            m_IsAsset = isAsset;
        }

        public bool DrawEditButton(bool edit)
        {
            return GUILayout.Toggle(edit, edit ? "Close" : "Edit", EditorStyles.miniButton, GUILayout.Width(64));
        }

        public void DrawComment(Comment comment, bool requireEdit = false)
        {
            if(requireEdit)
            {
                editMessagePath = rootMessage.propertyPath;
                requireEdit = false;
            }

            using (new GUILayout.HorizontalScope())
            {
                TypeLabel(comment.computedType);
                StateLabel(comment.computedState);
                PriorityLabel(comment.computedPriority);

                GUILayout.FlexibleSpace();
                EditorGUI.BeginChangeCheck();
                EditorGUI.BeginDisabledGroup(comment.message.from != CommentsWindow.user);
                bool editRoot = DrawEditButton(this.editRoot);
                EditorGUI.EndDisabledGroup();
                if(EditorGUI.EndChangeCheck())
                {
                    if (editRoot)
                        editMessagePath = rootMessage.propertyPath;
                    else
                        editMessagePath = string.Empty;
                }
            }

            GUILayout.Space(6);

            if(editRoot)
            {
                serializedObject.Update();
                EditorGUILayout.PropertyField(title);
                using(new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(focus);

                    if(!m_IsAsset && GUILayout.Button("Align to SceneView", GUILayout.ExpandWidth(true)))
                    {
                        focus.boolValue = true;
                        if(SceneView.lastActiveSceneView != null)
                        {
                            SceneComment c = serializedObject.targetObject as SceneComment;
                            c.gameObject.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
                            c.gameObject.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
                        }
                    }
                }
                serializedObject.ApplyModifiedProperties();
                CommentsWindow.RequestRefresh();
            }
            else
            {
                GUILayout.Space(8);

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label(CommentEditor.GetPriorityContent(" " + title.stringValue, comment.computedPriority), Styles.title);
                    GUILayout.FlexibleSpace();
                }
            }

            GUILayout.Space(6);
            DrawMessage(rootMessage, -1);
            int replyCount = replies.arraySize;
            using(new GUILayout.HorizontalScope())
            {
                GUILayout.Space(16);
                using(new GUILayout.VerticalScope())
                {
                    for (int i = 0; i < replyCount; i++)
                    {
                        DrawMessage(replies.GetArrayElementAtIndex(i), i);
                    }
                }
            }
        }

        void DrawMessage(SerializedProperty message, int replyIndex)
        {
            using(new EditorGUILayout.VerticalScope(Styles.message))
            {
                SerializedProperty body = message.FindPropertyRelative("body");
                SerializedProperty URL = message.FindPropertyRelative("URL");
                SerializedProperty from = message.FindPropertyRelative("from");
                SerializedProperty targets = message.FindPropertyRelative("attachedObjects");
                SerializedProperty changeType = message.FindPropertyRelative("changeType");
                SerializedProperty changeState = message.FindPropertyRelative("changeState");
                SerializedProperty changePriority = message.FindPropertyRelative("changePriority");
                SerializedProperty type = message.FindPropertyRelative("type");
                SerializedProperty state = message.FindPropertyRelative("state");
                SerializedProperty priority = message.FindPropertyRelative("priority");

                if (editMessagePath == message.propertyPath)
                {
                    message.serializedObject.Update();
                    EditorGUILayout.LabelField("Edit Message", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(body);
                    EditorGUILayout.PropertyField(URL);
                    EditorGUILayout.PropertyField(from);
                    using (new EditorGUI.IndentLevelScope(1))
                    {
                        EditorGUILayout.PropertyField(targets);
                    }

                    if(replyIndex >= 0)
                    {
                        EditorGUILayout.PropertyField(changeType);
                        EditorGUILayout.PropertyField(changeState);
                        EditorGUILayout.PropertyField(changePriority);
                    }

                    if(changeType.boolValue || replyIndex == -1)
                        EditorGUILayout.PropertyField(type);
                    if (changeState.boolValue || replyIndex == -1)
                        EditorGUILayout.PropertyField(state);
                    if (changePriority.boolValue || replyIndex == -1)
                        EditorGUILayout.PropertyField(priority);

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        if (editMessagePath == message.propertyPath && GUILayout.Button("Apply", Styles.miniButton))
                        {
                            editMessagePath = string.Empty;
                        }
                        if (replyIndex == replies.arraySize - 1 && from.stringValue == CommentsWindow.user && GUILayout.Button("Delete", Styles.miniButton))
                        {
                            replies.serializedObject.Update();
                            replies.DeleteArrayElementAtIndex(replyIndex);
                            replies.serializedObject.ApplyModifiedProperties();
                            CommentsWindow.RequestRefresh();

                        }
                    }
                    GUILayout.Space(2);
                    message.serializedObject.ApplyModifiedProperties();
                    CommentsWindow.RequestRefresh();

                }
                else // Display Message
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label($"<b>From:</b> <color={(from.stringValue == CommentsWindow.user ? "lime" : "white")}><b>{from.stringValue}</b></color>", Styles.from);
                        GUILayout.FlexibleSpace();

                        if (replyIndex > -1 && from.stringValue == CommentsWindow.user 
                            && GUILayout.Button(Styles.edit, Styles.miniButton, GUILayout.Width(32)))
                        {
                            editMessagePath = message.propertyPath;
                        }
                        if (GUILayout.Button(Styles.reply, Styles.miniButton, GUILayout.Width(32)))
                        {
                            replies.serializedObject.Update();
                            int index = replies.arraySize;
                            replies.InsertArrayElementAtIndex(index);
                            var reply = replies.GetArrayElementAtIndex(index);
                            editMessagePath = reply.propertyPath;
                            reply.FindPropertyRelative("body").stringValue = string.Empty;
                            reply.FindPropertyRelative("URL").stringValue = string.Empty;
                            reply.FindPropertyRelative("from").stringValue = CommentsWindow.user;
                            reply.FindPropertyRelative("attachedObjects").ClearArray();
                            replies.serializedObject.ApplyModifiedProperties();
                            CommentsWindow.RequestRefresh();
                        }
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(body.stringValue, Styles.multiline);

                    if (!string.IsNullOrEmpty(URL.stringValue))
                    {
                        GUILayout.Space(12);

                        GUILayout.Label("URL:", EditorStyles.boldLabel, GUILayout.Width(32));
                        Color b = GUI.backgroundColor;
                        GUI.backgroundColor = new Color(0, 0, 0, 0.2f);
                        if (GUILayout.Button($"<color=#44AAFFFF>{URL.stringValue}</color>", Styles.link))
                        {
                            Application.OpenURL(URL.stringValue);
                        }
                        GUI.backgroundColor = b;

                    }

                    if (targets.arraySize > 0)
                    {
                        GUILayout.Space(12);
                        EditorGUILayout.LabelField("Attached Objects:", EditorStyles.boldLabel);
                        EditorGUI.BeginDisabledGroup(true);
                        for (int i = 0; i < targets.arraySize; i++)
                        {
                            EditorGUILayout.ObjectField(targets.GetArrayElementAtIndex(i));
                        }
                        EditorGUI.EndDisabledGroup();
                    }

                    if(replyIndex == -1 
                        || changeType.boolValue 
                        || changeState.boolValue
                        || changePriority.boolValue)
                    {
                        GUILayout.Space(12);
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label("<b>Set Properties :</b>", Styles.multiline);
                            if (replyIndex == -1 || changeType.boolValue)
                                TypeLabel((CommentType)type.intValue);

                            if (replyIndex == -1 || changeState.boolValue)
                                StateLabel((CommentState)state.intValue);

                            if (replyIndex == -1 || changePriority.boolValue)
                                PriorityLabel((CommentPriority)priority.intValue);

                            GUILayout.FlexibleSpace();
                        }

                    }


                    
                }
            }
            GUI.contentColor = Color.white;
        }

        void Separator()
        {
            Rect r = GUILayoutUtility.GetRect(0, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(r, Color.gray);
        }

        public static Color GetTypeColor(CommentType type)
        {
            float sat = 0.3f;
            float v = 1f;
            switch (type)
            {

                default:
                case CommentType.Info:
                    return Color.HSVToRGB(0.4f, 0, v);
                case CommentType.Bug:
                    return Color.HSVToRGB(0.05f, sat, v);
                case CommentType.Request:
                    return Color.HSVToRGB(0.15f, sat, v);
                case CommentType.ToDo:
                    return Color.HSVToRGB(0.25f, sat, v);
            }
        }

        public static Color GetStateColor(CommentState state)
        {
            float sat = 0.8f;
            float v = 1f;
            switch (state)
            {
                default:
                case CommentState.Open:
                    return Color.HSVToRGB(0.3f, sat, v);
                case CommentState.Blocked:
                    return Color.HSVToRGB(0.05f, sat, v);
                case CommentState.Resolved:
                    return Color.HSVToRGB(0.5f, sat, v);
                case CommentState.WontFix:
                    return Color.HSVToRGB(0.05f, sat, v);
                case CommentState.Closed:
                    return Color.HSVToRGB(0.7f, 0f, v);
            }
        }

        public static Color GetPriorityColor(CommentPriority priority)
        {
            float sat = 0.9f;
            float v = 1f;
            switch (priority)
            {
                default:
                case CommentPriority.High:
                    return Color.HSVToRGB(0.02f, sat, v);
                case CommentPriority.Medium:
                    return Color.HSVToRGB(0.15f, sat, v);
                case CommentPriority.Low:
                    return Color.HSVToRGB(0.3f, sat, v);
            }
        }

        public static void TypeLabel(CommentType value)
        {
            GUIUtils.ColoredLabel(value.ToString(), GetTypeColor(value));
        }

        public static void StateLabel(CommentState value)
        {
            GUIUtils.ColoredLabel(value.ToString(), GetStateColor(value));
        }

        public static void PriorityLabel(CommentPriority value)
        {
            GUIUtils.ColoredLabel(value.ToString(), GetPriorityColor(value));
        }

        public static GUIContent GetPriorityContent(string text, CommentPriority priority)
        {
            return new GUIContent(text, GetPriorityTexture(priority));
        }

        public static Texture GetPriorityTexture(CommentPriority priority)
        {
            switch (priority)
            {
                case CommentPriority.High:
                    return CheckResult.GetIcon(CheckResult.Result.Failed);
                case CommentPriority.Medium:
                    return CheckResult.GetIcon(CheckResult.Result.Warning);
                default:
                case CommentPriority.Low:
                    return CheckResult.GetIcon(CheckResult.Result.Notice);
            }
        }


        static class Styles
        {
            public static GUIStyle title;
            public static GUIStyle from;
            public static GUIStyle link;
            public static GUIStyle multiline;
            public static GUIStyle coloredLabel;
            public static GUIStyle message;
            public static GUIStyle miniButton;

            public static GUIContent reply;
            public static GUIContent edit;
            static Styles()
            {
                title = new GUIStyle(EditorStyles.boldLabel);
                title.fontSize = 16;

                from = new GUIStyle(EditorStyles.label);
                from.fontSize = 12;
                from.richText = true;

                link = new GUIStyle(EditorStyles.label);
                link.fontSize = 12;
                link.richText = true;
                link.margin = new RectOffset(0, 0, 4, 4);
                link.padding = new RectOffset(2, 2, 2, 2);
                SetWhiteBG(link);

                multiline = new GUIStyle(EditorStyles.label);
                multiline.wordWrap = true;
                multiline.richText = true;
                multiline.fontSize = 13;

                coloredLabel = new GUIStyle(EditorStyles.label);
                coloredLabel.fontSize = 12;
                coloredLabel.padding = new RectOffset(12, 12, 2, 2);

                message = new GUIStyle(EditorStyles.helpBox);
                SetWhiteBG(message);

                miniButton = new GUIStyle(EditorStyles.miniButton);
                miniButton.fontSize = 10;
                miniButton.fixedHeight = 16;

                SetWhiteBG(miniButton);

                edit = new GUIContent(EditorGUIUtility.Load("Packages/net.peeweek.gameplay-ingredients/Icons/GUI/edit.png") as Texture);
                reply = new GUIContent(EditorGUIUtility.Load("Packages/net.peeweek.gameplay-ingredients/Icons/GUI/reply.png") as Texture);
            }

            static Texture2D flat;

            static void SetWhiteBG(GUIStyle style)
            {
                if(flat == null)
                {
                    flat = new Texture2D(1, 1, DefaultFormat.LDR, TextureCreationFlags.None);
                    flat.SetPixel(0, 0, new Color(0.5f, 0.5f, 0.5f, 0.3f));
                    flat.Apply();
                }    
                SetBGTexture(style, flat);
            }
            static void SetBGTexture(GUIStyle style, Texture2D texture)
            {
                style.onActive.background = texture;
                style.active.background = texture;
                style.onFocused.background = texture;
                style.focused.background = texture;
                style.onHover.background = texture;
                style.hover.background = texture;
                style.onNormal.background = texture;
                style.normal.background = texture;
            }
        }
    }
}

