using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    public static class LinkGameView
    {
        static readonly string kPreferenceName = "GameplayIngredients.LinkGameView";
        static readonly string kLinkCameraName = "___LINK__SCENE__VIEW__CAMERA___";

        public static bool Active
        {
            get
            {
                // Get preference only when not playing
                if (!Application.isPlaying)
                    m_Active = EditorPrefs.GetBool(kPreferenceName, false);

                return m_Active;
            }

            set
            {
                // Update preference only when not playing
                if(!Application.isPlaying)
                    EditorPrefs.SetBool(kPreferenceName, value);

                m_Active = value;

                if(s_GameObject != null)
                    s_GameObject.SetActive(value);

                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
        }

        static bool m_Active = false;


        public static SceneView LockedSceneView
        {
            get
            {
                return s_LockedSceneView;
            }

            set
            {
                s_LockedSceneView = value;
            }
        }

        static SceneView s_LockedSceneView;

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            SceneView.duringSceneGui += Update;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        static void OnPlayModeChanged(PlayModeStateChange state)
        {
            // Reset State when entering editmode or play mode
            if(state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.EnteredPlayMode)
            {
                if (Active)
                    Active = true;
                else
                    Active = false;
            }
            else // Cleanup before switching state
            {
                if (s_GameObject != null)
                    Object.DestroyImmediate(s_GameObject);
            }
        }

        const string kMenuPath = "Edit/Link SceneView and GameView %,";
        public const int kMenuPriority = 230;

        [MenuItem(kMenuPath, priority = kMenuPriority, validate = false)]
        static void Toggle()
        {
            if (Active)
                Active = false;
            else
                Active = true;
        }

        [MenuItem(kMenuPath, priority = kMenuPriority, validate = true)]
        static bool ToggleCheck()
        {
            Menu.SetChecked(kMenuPath, Active);
            return SceneView.sceneViews.Count > 0;
        }
        
        static GameObject s_GameObject;


        static void Update(SceneView sceneView)
        {
            // Check if camera Exists
            if (s_GameObject == null)
            {
                // If disconnected (should not happen, but hey...)
                var result = GameObject.Find(kLinkCameraName);

                if (result != null) // reconnect
                    s_GameObject = result;
                else // Create the camera if it does not exist
                    s_GameObject = CreateLinkedCamera();

                if (Application.isPlaying)
                    Active = false;
            }

            // If we have a VirtualCameraManager, manage its state here
            if(Application.isPlaying && Manager.Has<VirtualCameraManager>())
            {
                var camera = Manager.Get<VirtualCameraManager>().gameObject;

                if(camera.activeInHierarchy && Active) // We need to disable the VirtualCameraManager
                {
                    camera.SetActive(false);
                }
                else if (!camera.activeInHierarchy && !Active) // We need to re-enable the VirtualCameraManager
                {
                    camera.SetActive(true);
                }
            }

            if (Active)
            {
                var sv = s_LockedSceneView == null ? SceneView.lastActiveSceneView : s_LockedSceneView;
                var sceneCamera = sv.camera;
                var camera = s_GameObject.GetComponent<Camera>();
                bool needRepaint = sceneCamera.transform.position != camera.transform.position
                    || sceneCamera.transform.rotation != camera.transform.rotation
                    || sceneCamera.fieldOfView != camera.fieldOfView;

                if(needRepaint)
                {
                    s_GameObject.transform.position = sceneCamera.transform.position;
                    s_GameObject.transform.rotation = sceneCamera.transform.rotation;
                    camera.orthographic = sceneCamera.orthographic;
                    camera.fieldOfView = sceneCamera.fieldOfView;
                    camera.orthographicSize = sceneCamera.orthographicSize;
                    
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    needRepaint = false;
                }
            }
        }

        const string kDefaultPrefabName = "LinkGameViewCamera";

        static GameObject CreateLinkedCamera()
        {
            // Try to find an Asset named as the default name
            string[] assets = AssetDatabase.FindAssets(kDefaultPrefabName);
            if(assets.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[0]);
                GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));

                if (obj != null)
                {
                    var instance = GameObject.Instantiate(obj);
                    if(instance.GetComponent<Camera>() != null)
                    {
                        instance.hideFlags = HideFlags.HideAndDontSave;
                        instance.tag = "MainCamera";
                        instance.name = kLinkCameraName;
                        instance.SetActive(Active);
                        instance.GetComponent<Camera>().depth = int.MaxValue;
                        return instance;
                    }
                    else
                    {
                        Debug.LogWarning("LinkGameView Found default prefab but has no camera!");
                    }
                }
                else
                    Debug.LogWarning("LinkGameView Found default prefab but is not gameobject!");
            }


            // Otherwise ... Create default from code
            var go = new GameObject(kLinkCameraName);
            go.hideFlags = HideFlags.HideAndDontSave;
            go.tag = "MainCamera";
            var camera = go.AddComponent<Camera>();
            camera.depth = int.MaxValue;
            go.SetActive(Active);
            return go;
        }

    }
}

