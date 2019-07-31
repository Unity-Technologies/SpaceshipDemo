using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Actions
{
    public class GameSaveLoadSaveAction : ActionBase
    {
        public enum Action
        {
            Load,
            Save,
        }

        public GameSaveManager.Location saveLocation = GameSaveManager.Location.System;
        public Action action = Action.Load;
        public byte UserSaveIndex = 0;

        public override void Execute(GameObject instigator = null)
        {
            if(action == Action.Load)
            {
                if (saveLocation == GameSaveManager.Location.System)
                    Manager.Get<GameSaveManager>().LoadSystemSave();
                else
                    Manager.Get<GameSaveManager>().LoadUserSave(UserSaveIndex);
            }
            else
            {
                if (saveLocation == GameSaveManager.Location.System)
                    Manager.Get<GameSaveManager>().SaveSystemSave();
                else
                    Manager.Get<GameSaveManager>().SaveUserSave(UserSaveIndex);
            }
        }
    }
}
