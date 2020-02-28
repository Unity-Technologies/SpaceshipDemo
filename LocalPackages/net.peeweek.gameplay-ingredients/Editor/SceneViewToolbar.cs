using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace GameplayIngredients.Editor
{
    public static class SceneViewToolbar
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
                        GUI.color = Styles.lockedLinkColor * 2;
                    }
                    else if (isLinked && LinkGameView.CinemachineActive)
                    {
                        GUI.color = Styles.cineColor * 2;
                    }

                    isLinked = GUILayout.Toggle(isLinked, LinkGameView.CinemachineActive? Contents.linkGameViewCinemachine: Contents.linkGameView, EditorStyles.toolbarButton, GUILayout.Width(64));

                    if (GUI.changed)
                    {
                        if(Event.current.shift)
                        {
                            if (!LinkGameView.Active)
                                LinkGameView.Active = true;

                            LinkGameView.CinemachineActive = !LinkGameView.CinemachineActive;
                        }
                        else
                        {
                            LinkGameView.Active = isLinked;
                            LinkGameView.CinemachineActive = false;
                        }
                    }

                    isLocked = GUILayout.Toggle(isLocked, Contents.lockLinkGameView, EditorStyles.toolbarButton);

                    if (GUI.changed)
                    {
                        if (isLocked)
                        {
                            LinkGameView.CinemachineActive = false;
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

            if (LinkGameView.CinemachineActive)
            {
                DisplayText("CINEMACHINE PREVIEW", Styles.cineColor);
            }
            else if (LinkGameView.Active)
            {
                if (LinkGameView.LockedSceneView == sceneView)
                {
                    DisplayText("GAME VIEW LINKED (LOCKED)", Styles.lockedLinkColor);
                }
                else if(LinkGameView.LockedSceneView == null && SceneView.lastActiveSceneView == sceneView)
                {
                    DisplayText("GAME VIEW LINKED", Color.white);
                }
            }

            Handles.EndGUI();
        }

        static void DisplayText(string text, Color color)
        {
            Rect r = new Rect(16, 24, 512, 32);
            GUI.color = Color.black;
            GUI.Label(r, text);
            r.x--;
            r.y--;
            GUI.color = color;
            GUI.Label(r, text);
            GUI.color = Color.white;
        }

        static class Contents
        {
            public static GUIContent playFromHere;
            public static GUIContent lockLinkGameView;
            public static GUIContent linkGameView;
            public static GUIContent linkGameViewCinemachine;

            static Contents()
            {
                lockLinkGameView = new GUIContent(EditorGUIUtility.IconContent("IN LockButton"));
                linkGameView = new GUIContent(EditorGUIUtility.Load("Packages/net.peeweek.gameplay-ingredients/Icons/GUI/Camera16x16.png") as Texture);
                linkGameView.text = " Game";

                linkGameViewCinemachine = new GUIContent(EditorGUIUtility.Load("Packages/net.peeweek.gameplay-ingredients/Icons/GUI/Camera16x16.png") as Texture);
                linkGameViewCinemachine.text = " Cine";



                playFromHere = new GUIContent(EditorGUIUtility.IconContent("Animation.Play"));
                playFromHere.text = "Here";
            }
        }

        static class Styles
        {
            public static GUIStyle toolbar;
            public static Color lockedLinkColor = new Color(0.5f, 1.0f, 0.1f, 1.0f);
            public static Color cineColor = new Color(1.0f, 0.5f, 0.1f, 1.0f);

            static Styles()
            {
                toolbar = new GUIStyle(EditorStyles.inspectorFullWidthMargins);                
            }
        }
    }
}

