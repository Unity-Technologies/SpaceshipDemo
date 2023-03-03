using UnityEngine;

namespace GameplayIngredients.Actions
{
    [HelpURL(Help.URL + "factory")]
    [AddComponentMenu(ComponentMenu.factoryPath + "Factory Set Target Action")]
    [Callable("Game", "Misc/ic-factory.png")]
    public class FactorySetTargetAction : ActionBase
    {
        [NonNullCheck]
        public Factory factory;

        [NonNullCheck]
        public GameObject Target;

        public override void Execute(GameObject instigator = null)
        {
            if (factory != null && Target != null)
            {
                factory.SetTarget(Target);
            }
        }
    }
}
