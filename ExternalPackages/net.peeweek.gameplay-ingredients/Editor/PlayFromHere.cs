using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    public static class PlayFromHere
    {
        public delegate void PlayFromHereDelegate(Vector3 position, Vector3 forward);

        public static event PlayFromHereDelegate OnPlayFromHere;

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            EditorApplication.playModeStateChanged -= OnEnterPlayMode;
            EditorApplication.playModeStateChanged += OnEnterPlayMode;
        }

        public static bool IsReady
        {
            get
            {
                return OnPlayFromHere != null &&  OnPlayFromHere.GetInvocationList().Length > 0;
            }
        }

        public static void Play(SceneView sceneView)
        {
            var forward = sceneView.camera.transform.forward;
            var position = sceneView.camera.transform.position;

            EditorPrefs.SetInt("PlayFromHere", 1);
            EditorPrefs.SetFloat("PlayFromHere.position.x", position.x);
            EditorPrefs.SetFloat("PlayFromHere.position.y", position.y);
            EditorPrefs.SetFloat("PlayFromHere.position.z", position.z);
            EditorPrefs.SetFloat("PlayFromHere.forward.x", forward.x);
            EditorPrefs.SetFloat("PlayFromHere.forward.y", forward.y);
            EditorPrefs.SetFloat("PlayFromHere.forward.z", forward.z);

            EditorUtility.DisplayProgressBar("Play From Here", "Entering Play From here mode...", 1.0f);

            EditorApplication.isPlaying = true;
        }

        static void OnEnterPlayMode(PlayModeStateChange state)
        {
            if(state == PlayModeStateChange.ExitingPlayMode)
            {
                PlayerPrefs.SetInt("PlayFromHere", 0);
            }
            if(state == PlayModeStateChange.ExitingEditMode)
            {
                if(EditorPrefs.GetInt("PlayFromHere") == 1)
                {
                    PlayerPrefs.SetInt("PlayFromHere", 1);
                }
                else
                {
                    PlayerPrefs.SetInt("PlayFromHere", 0);
                }
            }

            if (state == PlayModeStateChange.EnteredPlayMode && (PlayerPrefs.GetInt("PlayFromHere") == 1))
            {
                EditorUtility.ClearProgressBar();

                if (OnPlayFromHere != null)
                {
                    Vector3 position = new Vector3(
                                    EditorPrefs.GetFloat("PlayFromHere.position.x"),
                                    EditorPrefs.GetFloat("PlayFromHere.position.y"),
                                    EditorPrefs.GetFloat("PlayFromHere.position.z"));
                    Vector3 forward = new Vector3(
                                    EditorPrefs.GetFloat("PlayFromHere.forward.x"),
                                    EditorPrefs.GetFloat("PlayFromHere.forward.y"),
                                    EditorPrefs.GetFloat("PlayFromHere.forward.z"));
                    OnPlayFromHere.Invoke(position, forward);
                }
                else
                {
                    Debug.LogWarning("Play From Here : No Actions to take. Please add events to PlayFromHere.OnPlayFromHere()");
                }

                EditorPrefs.SetInt("PlayFromHere", 0);
            }

        }
    }
}
