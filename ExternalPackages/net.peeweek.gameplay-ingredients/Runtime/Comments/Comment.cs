using NaughtyAttributes;
using System;
using System.Linq;
using UnityEngine;

namespace GameplayIngredients.Comments
{
    [Serializable]
    public struct Comment
    {
#if UNITY_EDITOR
        const string kUserPreference = "GameplayIngredients.Comments.User";
        public static string currentUser => UnityEditor.EditorPrefs.GetString(kUserPreference, "user");
#endif

        public string title;
        public CommentMessage message; 
        public CommentMessage[] replies;
        public bool focus;

        public string[] users
        {
            get
            {
                var users = replies.Select(r => r.from).ToList();
                users.Add(message.from);
                return users.ToArray();
            }
        }

        public CommentType computedType
        {
            get
            {
                CommentType currentType = message.type;
                if(replies != null)
                {
                    foreach (var reply in replies)
                    {
                        if (reply.changeType)
                            currentType = reply.type;
                    }
                }

                return currentType;
            }
        }
        public CommentState computedState
        {
            get
            {
                CommentState currentState = message.state;
                if (replies != null)
                {
                    foreach (var reply in replies)
                    {
                        if (reply.changeState)
                            currentState = reply.state;
                    }
                }

                return currentState;
            }
        }
        public CommentPriority computedPriority
        {
            get
            {
                CommentPriority currentPriority = message.priority;
                if (replies != null)
                {
                    foreach (var reply in replies)
                    {
                        if (reply.changePriority)
                            currentPriority = reply.priority;
                    }
                }

                return currentPriority;
            }
        }
    }

    [Serializable]
    public struct CommentMessage
    {
        public string from;
        public string URL;
        [Multiline]
        public string body;
        [ReorderableList, NoLabel]
        public UnityEngine.Object[] attachedObjects;

        public bool changeType;
        public bool changeState;
        public bool changePriority;

        public CommentType type;
        public CommentState state;
        public CommentPriority priority;
    }

    [Serializable]
    public enum CommentType
    {
        Info,
        Bug,
        Request,
        ToDo,
    }

    [Serializable]
    public enum CommentState
    {
        Open,
        Blocked,
        Resolved,
        WontFix,
        Closed,
    }

    [Serializable]
    public enum CommentPriority
    {
        High,
        Medium,
        Low,
    }

}
