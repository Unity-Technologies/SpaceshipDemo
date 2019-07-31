using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    partial class WelcomeScreen : EditorWindow
    {
        int tipIndex = 0;

        void InitTips()
        {
            tipIndex = Random.Range(0, tips.Count);
        }

        void OnTipsGUI()
        {
            GUILayout.Label("Tip of the Day", EditorStyles.boldLabel);

            using (new GUILayout.VerticalScope(Styles.helpBox))
            {
                var tip = tips[tipIndex];
                GUILayout.Label(tip.Title, Styles.title);
                GUILayout.Space(12);
                GUILayout.Label(tip.Body, Styles.body);
                GUILayout.FlexibleSpace();
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("<<"))
                    {
                        tipIndex--;
                        if (tipIndex < 0)
                            tipIndex = tips.Count - 1;
                    }
                    if (GUILayout.Button(">>"))
                    {
                        tipIndex++;
                        if (tipIndex == tips.Count)
                            tipIndex = 0;
                    }
                }
            }
        }

        struct Tip
        {
            public string Title;
            public string Body;
        }

        static List<Tip> tips = new List<Tip>()
        {
#if UNITY_2018_3
            new Tip(){ Title = "Show Gizmos", Body = "You can toggle Gizmos on and off by using the Ctrl + , key shortcut."},
#endif
            new Tip(){ Title = "Gameplay Ingredients Setttings", Body = "This special file contains all configuration for Gameplay Ingredients in your project. While this asset is not mandatory, it is advised to create it to be able to configure the project.\n\nIn order to create it, use the Setup Wizard window by clicking the 'Window/Gameplay Ingredients' menu."},
            new Tip(){ Title = "Editor Scene Setups", Body = "You can save your current multi-scene setup to an asset by using the File/Save Current Scene Setup As... or create an Editor Scene Setup asset from the Project View. These assets can be double-clicked to restore all scenes at once."},
            new Tip(){ Title = "Find And Replace", Body = "You can use the Find and Replace window to select, add, and/or refine list of objects in your scene based on different criteria (name, components, etc.). You can then turn this search result into a selection, or replace every object from this list by a prefab or a copy of another game object.\n\n Use the 'Edit/Find And Replace' menu to open the window or use the Ctrl+Alt+Shift+F key combo."},
            new Tip(){ Title = "Gameplay Ingredients Window", Body = "This is the window you are currently looking at : if you close it, you can access it later by clicking the 'Window/Gameplay Ingredients' menu, or by restarting the editor (if the 'Show At Startup' Flag is checked.\n\nThis window displays tooltips and enables to perform basic configuration for your project."},
            new Tip(){ Title = "Game View Link", Body = "Game View link enables you to link your scene view and your game view so the scene point of view replicates in the game view. You can define a prefab (named LinkGameViewCamera) containing a camera to setup this camera (for instance if you use HD Render Pipeline and Postprocessing).\n\nYou can Toggle the Game View Linking by cicking the '(Camera) Game' Button on the additional toolbar or use the Ctrl+, key combo. \n\nThe lock icon next to the button enables locking to a particular SceneView window (useful when using multiple Scene Views)."},
            new Tip(){ Title = "Advanced Hierarchy View", Body = "You can toggle Advanced Hierarchy View by selecting them in the Edit menu. This will display hints as icons for most common components on your game objects.\n\nYou can toggle these hints by using the Edit/Advanced Hierarchy View "},
            new Tip(){ Title = "Scene View POVs", Body = "Scene View POVs enable storing custom point of views in your scenes. To use it, select the POV dropdown in the additional Scene View toolbar."},
            new Tip(){ Title = "Selection History", Body = "Selection History keeps track of your previously selected objects. You can also star/unstar objects in order to go back at them more easily.\n\nTo open this window, use the 'Window/Selection History' menu item."},
            new Tip(){ Title = "Play From Here", Body = "Play From Here enables a custom callback when starting your Editor Play Session. Implement the callback and use the scene view camera position and forward vector to generate your custom start function."},
            new Tip(){ Title = "Events, Logic and Actions", Body = "Gameplay Ingredients ships with many Actions, Logic and Events in order to set-up your scenes. Actions perform various actions on your scene objects, Logic trigger actions based on conditions, Events trigger Actions and Logic based on scene interaction (eg: On Trigger Enter)"},
            new Tip(){ Title = "Managers", Body = "Managers are Monobehaviors that instantiate themselves automatically upon startup. You can define a [ManagerDefaultPrefab] attribute to load a prefab containing the manager if it needs to handle other objects and components.\n\nYou can disable certain managers from being created by editing the 'Excluded Managers' list in your GameplayIngredientsSettings asset."},
        };

    }
}
