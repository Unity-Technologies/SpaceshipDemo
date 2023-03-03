using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using GameplayIngredients.Comments.Editor;

namespace GameplayIngredients.Editor
{
    public static partial class SceneViewToolbar
    {
        public delegate void SceneViewToolbarDelegate(SceneView sceneView);
        public static event SceneViewToolbarDelegate OnSceneViewToolbarGUI;
    }
}
