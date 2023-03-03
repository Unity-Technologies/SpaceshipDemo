using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Actions
{
    [AddComponentMenu(ComponentMenu.actionsPath + "Destroy Object Action")]
    [Callable("Game Objects", "Actions/ic-action-trash.png")]
    public class DestroyObjectAction : ActionBase
    {
        [ReorderableList]
        public GameObject[] ObjectsToDestroy;
        public bool DestroyInstigator = false;

        public override void Execute(GameObject instigator = null)
        {
            if (ObjectsToDestroy != null )
            {
                foreach(var obj in ObjectsToDestroy)
                    Destroy(obj);
            }

            if(DestroyInstigator && instigator != null)
                Destroy(instigator);
        }
        public override string GetDefaultName()
        {
            return $"Destroy {(DestroyInstigator?"instigator": "objects")}";
        }
    }
}
