using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Actions
{
    public class SpawnPrefabAction : ActionBase
    {
        [ReorderableList]
        public GameObject[] Prefabs;

        public Transform TargetTransform;

        public bool TargetInstigator = false;
        public bool AttachToTarget = false;
        public bool DontDestroyPrefabsOnLoad = false;

        public override void Execute(GameObject instigator = null)
        {
            foreach (var prefab in Prefabs)
            {
                string name = prefab.name;

                Vector3 position = gameObject.transform.position;
                Quaternion rotation = gameObject.transform.rotation;

                if(TargetInstigator && instigator != null)
                {
                    position = instigator.transform.position;
                    rotation = instigator.transform.rotation;
                }
                else if (TargetTransform != null)
                {
                    position = TargetTransform.position;
                    rotation = TargetTransform.rotation;
                }

                var obj = Instantiate<GameObject>(prefab, position, rotation);
                obj.name = name;

                if (AttachToTarget)
                {
                    if (TargetInstigator && instigator != null)
                        obj.transform.parent = instigator.transform;
                    else
                        obj.transform.parent = TargetTransform;
                }

                if (DontDestroyPrefabsOnLoad)
                    DontDestroyOnLoad(obj);
            }
        }
    }
}
