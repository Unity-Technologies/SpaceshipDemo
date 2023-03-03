using NaughtyAttributes;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameplayIngredients.Logic
{
    [AddComponentMenu(ComponentMenu.logicPath + "Platform Logic")]
    [Callable("Application", "Logic/ic-generic-logic.png")]
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

        [FormerlySerializedAs("Calls")]
        public Callable[] OnTestValid;
        public Callable[] OnTestInvalid;


        public override void Execute(GameObject instigator = null)
        {

            if(platforms.Contains(Application.platform) == (inclusionMode == InclusionMode.IsTarget))
                Call(OnTestValid, instigator);
            else
                Call(OnTestInvalid, instigator);
        }

        public override string GetDefaultName()
        {
            return $"If Platform {inclusionMode} : {string.Join(", ",platforms)}";
        }
    }
}

