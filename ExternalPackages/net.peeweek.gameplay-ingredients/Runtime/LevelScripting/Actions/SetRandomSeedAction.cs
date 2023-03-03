using NaughtyAttributes;
using System;
using UnityEngine;

namespace GameplayIngredients.Actions
{
    [AddComponentMenu(ComponentMenu.actionsPath + "Set Random Seed Action")]
    [Callable("Game", "Actions/ic-action-randomseed.png")]
    public class SetRandomSeedAction : ActionBase
    {
        [Tooltip("Whether to set a new random seed, or a fixed seed")]
        public bool newRandomSeed;
        [DisableIf("newRandomSeed")]
        [Tooltip("If New Random Seed is False. the new Random seed to apply")]
        public int newSeed;

        public override void Execute(GameObject instigator = null)
        {
            if(Manager.TryGet(out RandomManager randomManager))
            {
                if (newRandomSeed)
                    randomManager.SetRandomSeed(new System.Random((int)(DateTime.Now.Ticks - new DateTime(2021,1,1).Ticks)).Next());
                else
                    randomManager.SetRandomSeed(newSeed);
            }
        }

        public override string GetDefaultName()
        {
            return $"Set Random Seed :'{(newRandomSeed ? "New Seed at Random":newSeed.ToString())}'";
        }
    }
}


