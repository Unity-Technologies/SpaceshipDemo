using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Actions
{
    public class ResetGlobalAction : ActionBase
    {
        public ResetType resetType = ResetType.Locals;
        public enum ResetType
        {
            Locals = 1,
            Globals = 2,
            All = Locals | Globals,
        }
        public override void Execute(GameObject instigator = null)
        {
            if (resetType == ResetType.Locals || resetType == ResetType.All)
                Globals.ResetLocals();

            if (resetType == ResetType.Globals || resetType == ResetType.All)
                Globals.ResetGlobals();
        }
    }
}
