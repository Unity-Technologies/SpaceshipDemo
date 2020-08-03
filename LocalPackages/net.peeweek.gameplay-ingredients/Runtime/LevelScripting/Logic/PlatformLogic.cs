using NaughtyAttributes;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayIngredients.Logic
{
    public class PlatformLogic : LogicBase
    {
        public enum InclusionMode
        {
            IsTarget,
            IsNotTarget
        }

        [ReorderableList]
        public RuntimePlatform[] platforms;

        public InclusionMode inclusionMode = InclusionMode.IsTarget;

        [ReorderableList]
        public Callable[] OnTestValid;

        [ReorderableList]
        public Callable[] OnTestInvalid;


        public override void Execute(GameObject instigator = null)
        {

            if(platforms.Contains(Application.platform) == (inclusionMode == InclusionMode.IsTarget))
                Call(OnTestValid, instigator);
            else
                Call(OnTestInvalid, instigator);
        }
    }
}

