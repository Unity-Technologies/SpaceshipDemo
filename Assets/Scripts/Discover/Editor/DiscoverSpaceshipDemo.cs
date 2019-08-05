using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using GameplayIngredients.Editor;

static class DiscoverSpaceshipDemo
{
    [MenuItem("Help/Discover Spaceship Demo", priority = 0)]
    static void ShowSpaceshipDiscover()
    {
        var asset = AssetDatabase.LoadAssetAtPath<DiscoverAsset>("Assets/Scripts/Discover/DiscoverSpaceshipDemo.asset");
        DiscoverWindow.ShowDiscoverWindow(asset);
    }
}
