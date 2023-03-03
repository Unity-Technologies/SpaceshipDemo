using UnityEngine;

namespace GameplayIngredients.Actions
{
    [HelpURL(Help.URL + "factory")]
    [AddComponentMenu(ComponentMenu.factoryPath + "Factory Spawn Action")]
    [Callable("Game", "Misc/ic-factory.png")]
    public class FactorySpawnAction : ActionBase
    {
        [NonNullCheck]
        public Factory factory;

        public override void Execute(GameObject instigator = null)
        {
            if (factory != null)
                factory.Spawn();
        }
    }
}
