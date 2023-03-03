using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameplayIngredients.LevelStreaming;
using NaughtyAttributes;

namespace GameplayIngredients.Actions
{
    [AddComponentMenu(ComponentMenu.actionsPath + "Streaming Level Action")]
    [Callable("Game", "Misc/ic-scene.png")]
    public class StreamingLevelAction : ActionBase
    {
        [ReorderableList, Scene]
        public string[] Scenes;
        public string SceneToActivate;
        public LevelStreamingManager.StreamingAction Action = LevelStreamingManager.StreamingAction.Load;

        public bool ShowUI = false;
        
        public Callable[] OnLoadComplete;

        public override void Execute(GameObject instigator = null)
        {
            List<string> sceneNames = new List<string>();
            foreach (var scene in Scenes)
                sceneNames.Add(scene);
            Manager.Get<LevelStreamingManager>().LoadScenes(Action, sceneNames.ToArray(), SceneToActivate, ShowUI, OnLoadComplete);
        }

        public override string GetDefaultName()
        {
            return $"Streaming : {Action} {Scenes.Length} scene(s)";
        }
    }
}

