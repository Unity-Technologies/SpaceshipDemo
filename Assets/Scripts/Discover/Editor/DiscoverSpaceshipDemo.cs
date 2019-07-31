using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using GameplayIngredients.Editor;

[InitializeOnLoad]
static class DiscoverSpaceshipDemo
{
    static DiscoverAsset asset;

    static DiscoverSpaceshipDemo()
    {
        asset = AssetDatabase.LoadAssetAtPath<DiscoverAsset>("Assets/Scripts/Discover/DiscoverSpaceshipDemo.asset");
    }

    [MenuItem("Help/Discover Spaceship Demo", priority = 0)]
    static void ShowSpaceshipDiscover()
    {
        DiscoverWindow.ShowDiscoverWindow(asset);
    }
}
