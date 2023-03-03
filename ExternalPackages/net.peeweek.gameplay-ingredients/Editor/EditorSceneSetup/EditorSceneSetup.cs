using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.Callbacks;

namespace GameplayIngredients.Editor
{
    public class EditorSceneSetup : ScriptableObject
    {
        [MenuItem("File/Save Scene Setup As... #%&S", priority = 171)]
        static void SaveSetup()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save EditorSceneSetup", "New EditorSceneSetup", "asset", "Save EditorSceneSetup?");
            if(path != string.Empty)
            {
                EditorSceneSetup setup = GetCurrentSetup();
                AssetDatabase.CreateAsset(setup, path);
            }
            
        }

        public delegate void EditorSceneSetupLoadedDelegate(EditorSceneSetup setup);
        public static event EditorSceneSetupLoadedDelegate onSetupLoaded;

        [OnOpenAsset]
        static bool OnOpenAsset(int instanceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            if(obj is EditorSceneSetup)
            {
                EditorSceneSetup setup = (EditorSceneSetup)obj;
                int active = setup.ActiveScene;

                try
                {
                    EditorUtility.DisplayProgressBar("Loading Scenes", string.Format("Loading Scene Setup {0}....", setup.name), 1.0f);
                    RestoreSetup(setup);
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
                return true;
            }
            return false;
        }

        [MenuItem("Assets/Create/Editor Scene Setup", priority = 200)]
        static void CreateAsset()
        {
            AssetFactory.CreateAssetInProjectWindow<EditorSceneSetup>("SceneSet Icon", "New SceneSetup.asset");
        }
        
        public int ActiveScene;
        public EditorScene[] LoadedScenes;

        [System.Serializable]
        public struct EditorScene
        {
            public SceneAsset Scene;
            public bool Loaded;
        }

        public static EditorSceneSetup GetCurrentSetup()
        {
            var scenesetups = EditorSceneManager.GetSceneManagerSetup();

            var editorSetup = CreateInstance<EditorSceneSetup>();

            int i = 0;
            editorSetup.LoadedScenes = new EditorScene[scenesetups.Length];
            foreach(var setup in scenesetups)
            {
                if (setup.isActive)
                    editorSetup.ActiveScene = i;

                editorSetup.LoadedScenes[i].Scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(setup.path);
                editorSetup.LoadedScenes[i].Loaded = setup.isLoaded;

                i++;
            }
            return editorSetup;
        }

        public static void RestoreSetup(EditorSceneSetup editorSetup)
        {
            SceneSetup[] setups = new SceneSetup[editorSetup.LoadedScenes.Length];

            for(int i = 0; i < setups.Length; i++)
            {
                setups[i] = new SceneSetup();
                string path = AssetDatabase.GetAssetPath(editorSetup.LoadedScenes[i].Scene);
                setups[i].path = path;
                setups[i].isLoaded = editorSetup.LoadedScenes[i].Loaded;
                setups[i].isActive = (editorSetup.ActiveScene == i);
            }

            EditorSceneManager.RestoreSceneManagerSetup(setups);

            if(onSetupLoaded != null)
                onSetupLoaded.Invoke(editorSetup);
        }

    }
}


