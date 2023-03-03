using UnityEngine;
using GameObjectToggle = GameplayIngredients.Actions.ToggleGameObjectAction.GameObjectToggle;

namespace GameplayIngredients
{
    [AddComponentMenu(ComponentMenu.basePath + "Discover")]

    public class Discover : MonoBehaviour
    {
        public string Name = "Discover";
        public string Category = "Category";
        public bool DefaultSelected = false;
        public bool AlignViewToTransform = false;

#if UNITY_EDITOR
        public Texture2D image;
#endif

        public GameObjectToggle[] ObjectsToToggle = new GameObjectToggle[0];
        [Multiline]
        public string Description = "Some Description of the Component\n\nCan be set as multiple lines.";
        public string Tags = "";
        public int Priority = 0;
        public DiscoverSection[] Sections = new DiscoverSection[0];

#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Gameplay Ingredients/Discover", priority = 10)]
        static void CreateObject()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;

            var go = new GameObject("Discover");
            if (selected != null)
            {
                go.transform.parent = selected.transform;
                go.transform.position = selected.transform.position;
            }
            go.AddComponent<Discover>();
            UnityEditor.Selection.activeGameObject = go;
        }
#endif
    }

    [System.Serializable]
    public struct DiscoverSection
    {
        public string SectionName;
#if UNITY_EDITOR
        public Texture2D image;
#endif
        [Multiline]
        public string SectionContent;
        public SectionAction[] Actions;
    }

    [System.Serializable]
    public struct SectionAction
    {
        public string Description;
        public Object Target;
    }

}
