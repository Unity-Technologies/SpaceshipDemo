using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    partial class WelcomeScreen : EditorWindow
    {
        void OnSetupGUI()
        {
            GUILayout.Label("First Time Setup", EditorStyles.boldLabel);

            using (new GUILayout.VerticalScope(Styles.helpBox))
            {
                pages[currentPage].Invoke();

                GUILayout.FlexibleSpace();
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    EditorGUI.BeginDisabledGroup(currentPage == 0);
                    if (GUILayout.Button("Back"))
                    {
                        currentPage--;
                    }
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.BeginDisabledGroup(currentPage == pages.Length - 1);
                    if (GUILayout.Button("Next"))
                    {
                        currentPage++;
                    }
                    EditorGUI.EndDisabledGroup();
                }
            }
        }

        public int currentPage = 0;

        public Action[] pages = new Action[]
        {
            WelcomePage, SettingAssetPage, UnpackPackagePage
        };

        static void WelcomePage()
        {
            GUILayout.Label("Welcome to Gameplay Ingredients !", Styles.title);
            GUILayout.Space(12);
            GUILayout.Label(@"This wizard will help you set up your project so you can use and customize scripts.", Styles.body);

        }

        public const string kSettingsAssetPath = "Assets/Resources/GameplayIngredientsSettings.asset";

        static void SettingAssetPage()
        {
            GUILayout.Label("Creating a Settings Asset", Styles.title);
            GUILayout.Space(12);
            GUILayout.Label(@"GameplayIngredients is a framework that comes with a variety of features : these can be configured in a <b>GameplayIngredientsSettings</b> asset.

This asset needs to be stored in a Resources folder.
While this is not mandatory we advise you to create it in order to be able to modify it for your project's needs.
", Styles.body);
            GUILayout.Space(16);
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Create GameplayIngredientsSettings Asset"))
                {
                    bool create = true;
                    if(System.IO.File.Exists(Application.dataPath +"/../"+ kSettingsAssetPath))
                    {
                        if (!EditorUtility.DisplayDialog("GameplayIngredientsSettings Asset Overwrite", "A GameplayIngredientsSettings Asset already exists, do you want to overwrite it?", "Yes", "No"))
                            create = false;
                    }

                    if(create)
                    {
                        if(!System.IO.Directory.Exists(Application.dataPath+"/Resources"))
                            AssetDatabase.CreateFolder("Assets", "Resources");

                        GameplayIngredientsSettings asset = Instantiate<GameplayIngredientsSettings>(GameplayIngredientsSettings.defaultSettings);
                        AssetDatabase.CreateAsset(asset, kSettingsAssetPath);
                        Selection.activeObject = asset;
                    }
                }
            }
        }

        static void UnpackPackagePage()
        {
            GUILayout.Label("Unpacking Starter Content", Styles.title);
            GUILayout.Space(12);
            GUILayout.Label(@"In order to customize your project, you can create default assets that you will be able to customize. 

Please select a package depending on your project's render loop. If you do not know about render loops, you will probably need the Legacy Package.", Styles.body);
            GUILayout.Space(16);
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                using (new GUILayout.VerticalScope())
                {
                    if (GUILayout.Button("Legacy Renderer"))
                        AssetDatabase.ImportPackage("Packages/net.peeweek.gameplay-ingredients/StarterAssets/GameplayIngredients-Starter-LegacyRenderer.unitypackage", false);
                    if (GUILayout.Button("HD Render Pipeline"))
                        AssetDatabase.ImportPackage("Packages/net.peeweek.gameplay-ingredients/StarterAssets/GameplayIngredients-Starter-HDRP.unitypackage", false);
                    if (GUILayout.Button("Lightweight Render Pipeline"))
                        AssetDatabase.ImportPackage("Packages/net.peeweek.gameplay-ingredients/StarterAssets/GameplayIngredients-Starter-LWRP.unitypackage", false);
                }
            }
        }

    }
}

