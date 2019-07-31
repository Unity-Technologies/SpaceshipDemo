using System.Linq;
using UnityEngine;
using NaughtyAttributes;

namespace GameplayIngredients.Actions
{
    public class GameLevelLoadAction : ActionBase
    {
        public enum Target
        {
            MainMenu,
            First,
            Previous,
            Current,
            Next,
            Last,
            SpecifiedLevel,
            FromGameSave,
        }
        public bool ShowUI = true;
        public Target level = Target.First;
        [NonNullCheck, ShowIf("isSpecified"), Tooltip("Which Level to Load/Unload, when selected 'Specified' level")]
        public GameLevel specifiedLevel;

        [ShowIf("isGameSave")]
        public int UserSaveIndex = 0;
        [ShowIf("isGameSave")]
        public string UserSaveName = "Progress";

        public bool SaveProgress = false;

        [ReorderableList]
        public Callable[] OnComplete;

        private bool isSpecified() { return level == Target.SpecifiedLevel; }
        private bool isGameSave() { return level == Target.FromGameSave; }

        public override void Execute(GameObject instigator = null)
        {
            int index = -2;
            var manager = Manager.Get<GameManager>();

            switch (level)
            {
                case Target.MainMenu: index = -1; break;
                case Target.First: index = 0; break;
                case Target.Last: index = manager.MainGameLevels.Length - 1; break;
                case Target.Current: index = manager.currentLevel; break;
                case Target.Previous: index = Mathf.Max(0, manager.currentLevel - 1); break;
                case Target.Next: index = Mathf.Min(manager.MainGameLevels.Length - 1, manager.currentLevel + 1); break;
                case Target.SpecifiedLevel:
                    if (specifiedLevel != null && manager.MainGameLevels.Contains(specifiedLevel))
                    {
                        index = manager.MainGameLevels.ToList().IndexOf(specifiedLevel);
                    }
                    break;
                case Target.FromGameSave: index = manager.currentSaveProgress; break;
            }
            manager.SwitchLevel(index, ShowUI, OnComplete, SaveProgress);
        }
    }
}
