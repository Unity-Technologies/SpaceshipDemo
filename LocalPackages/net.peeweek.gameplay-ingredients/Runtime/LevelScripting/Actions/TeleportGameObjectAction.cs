using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Actions
{
    public class TeleportGameObjectAction : ActionBase
    {
        [ReorderableList]
        public GameObject[] ObjectsToTeleport;
        public bool TeleportInstigator = false;

        public Transform TeleportTarget;

        public override void Execute(GameObject instigator = null)
        {
            if(TeleportTarget == null)
            {
                Debug.LogWarning("No Teleport Target");
                return;
            }
            if(ObjectsToTeleport != null)
            {
                foreach(var obj in ObjectsToTeleport)
                {
                    Teleport(obj, TeleportTarget.position, TeleportTarget.rotation);
                }
            }
            if (TeleportInstigator && instigator != null)
                Teleport(instigator, TeleportTarget.position, TeleportTarget.rotation);
        }

        static void Teleport(GameObject obj, Vector3 worldPosition, Quaternion rotation)
        {
            obj.transform.position = worldPosition;
            obj.transform.rotation = rotation;
        }
    }
}
