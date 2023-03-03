using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Logic
{
    [AddComponentMenu(ComponentMenu.logicPath + "Input System Logic")]
    [Callable("Application", "Logic/ic-generic-logic.png")]
    public class InputSystemLogic : LogicBase
    {
        [ShowIf("checkForLegacyInput")]
        public Callable[] OnLegacyInputPresent;
        [ShowIf("checkForLegacyInput")]
        public Callable[] OnLegacyInputNotPresent;

        [ShowIf("checkForNewInput")]
        public Callable[] OnNewInputPresent;
        [ShowIf("checkForNewInput")]

        public Callable[] OnNewInputNotPresent;

        public override void Execute(GameObject instigator = null)
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            Call(OnLegacyInputPresent, instigator);
#else
            Call(OnLegacyInputNotPresent, instigator);
#endif

#if ENABLE_INPUT_SYSTEM
            Call(OnNewInputPresent, instigator);
#else
            Call(OnLegacyInputNotPresent, instigator);
#endif
        }
    }
}

