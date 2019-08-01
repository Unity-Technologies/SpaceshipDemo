using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameplayIngredients.Editor
{
    public class GameplayIngredientsAssetPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (importedAssets.Contains(WelcomeScreen.kSettingsAssetPath))
            {
                Debug.Log("Imported GameplayIngredientsSettings");
                WelcomeScreen.Reload();
            }

            string[] allDiscovery = AssetDatabase.FindAssets("t:DiscoverAsset");
            bool needDiscoveryReload = false;
            foreach(var guid in allDiscovery)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if(importedAssets.Contains(path))
                {
                    needDiscoveryReload = true;
                    break;
                }
            }
            if (needDiscoveryReload)
                DiscoverWindow.Reload();
        }
    }
}
