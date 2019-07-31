using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Actions
{
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
    }
}
