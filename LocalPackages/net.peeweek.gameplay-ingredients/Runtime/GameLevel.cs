using UnityEngine;
using NaughtyAttributes;

namespace GameplayIngredients
{
    public class GameLevel : ScriptableObject
    {
        [ReorderableList, Scene]
        public string[] StartupScenes;
    }
}

