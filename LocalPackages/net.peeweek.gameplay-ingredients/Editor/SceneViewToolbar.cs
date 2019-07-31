using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace GameplayIngredients.Editor
{
    static class SceneViewToolbar
    {
        public delegate void SceneViewToolbarDelegate(SceneView sceneView);

        public static event SceneViewToolbarDelegate OnSceneViewToolbarGUI;

        [InitializeOnLoadMethod]
        static void Initialize()
        {
           SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            var r = new Rect(Vector2.zero, new Vector2(sceneView.position.width,24));
            Handles.BeginGUI();
            using (new GUILayout.AreaScope(r))
            {
                using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                {

                    if(PlayFromHere.IsReady)
                    {
                        bool play = GUILayout.Toggle(EditorApplication.isPlaying, Contents.playFromHere, EditorStyles.toolbarButton);

                        if(GUI.changed)
                        {
                            if (play)
                                PlayFromHere.Play(sceneView);
                            else
                                EditorApplication.isPlaying = false;
                        }

                        GUILayout.Space(24);
                    }

                    Color backup = GUI.color;

                    bool isLinked = LinkGameView.Active;
                    bool isLocked = LinkGameView.LockedSceneView == sceneView;


                    if(isLinked && isLocked)
                    {
                        GUI.color = Color.green *2;
                    }

                    isLinked = GUILayout.Toggle(isLinked, Contents.linkGameView, EditorStyles.toolbarButton, GUILayout.Width(64));

                    if (GUI.changed)
                    {
                        LinkGameView.Active = isLinked;
                    }

                    isLocked = GUILayout.Toggle(isLocked, Contents.lockLinkGameView, EditorStyles.toolbarButton);

                    if (GUI.changed)
                    {
                        if (isLocked)
                        {
                            LinkGameView.LockedSceneView = sceneView;
                        }
                        else
                        {
                            LinkGameView.LockedSceneView = null;
                        }
                    }

                    GUI.color = backup;

                    // SceneViewPOV
                    GUILayout.Space(16);
                    if(GUILayout.Button("POV", EditorStyles.toolbarDropDown))
                    {
                        Rect btnrect = GUILayoutUtility.GetLastRect();
                        btnrect.yMax += 17;
                        SceneViewPOV.ShowPopup(btnrect, sceneView);
                    }

                    GUILayout.FlexibleSpace();

                    // Custom Code here
                    if (OnSceneViewToolbarGUI != null)
                        OnSceneViewToolbarGUI.Invoke(sceneView);

                    // Saving Space not to overlap view controls
                    GUILayout.Space(96);

                }
            }
            Handles.EndGUI();
        }

        static class Contents
        {
            public static GUIContent playFromHere;
            public static GUIContent lockLinkGameView;
            public static GUIContent linkGameView;

            static Contents()
            {
                lockLinkGameView = new GUIContent(EditorGUIUtility.IconContent("IN LockButton"));
                linkGameView = new GUIContent(EditorGUIUtility.Load("Packages/net.peeweek.gameplay-ingredients/Icons/GUI/Camera16x16.png") as Texture);
                linkGameView.text = " Game";

                playFromHere = new GUIContent(EditorGUIUtility.IconContent("Animation.Play"));
                playFromHere.text = "Here";
            }
        }

        static class Styles
        {
            public static GUIStyle toolbar;

            static Styles()
            {
                toolbar = new GUIStyle(EditorStyles.inspectorFullWidthMargins);                
            }
        }
    }
}

