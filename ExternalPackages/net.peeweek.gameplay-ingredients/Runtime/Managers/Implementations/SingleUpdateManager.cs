using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameplayIngredients.Managers
{
    [AddComponentMenu(ComponentMenu.managersPath + "Single Update Manager")]
    [NonExcludeableManager]
    public class SingleUpdateManager : Manager
    {
        public delegate void SingleUpdate();

        List<SingleUpdate> updateList;

        private void OnEnable()
        {
            updateList = new List<SingleUpdate>();
        }

        public void Register(SingleUpdate update)
        {
            if (!updateList.Any(o => o == update))
            {
                updateList.Add(update);
            }
            else
                Debug.LogWarning("SingleUpdateManager: Already found an entry for this SingleUpdate, ignoring."); 
        }

        public void Remove(SingleUpdate update)
        {
            if(updateList.Any(o => o == update))
            {
                updateList.RemoveAll(o => o == update);
            }
            else
                Debug.LogWarning("SingleUpdateManager: Did not found a matching entry for given SingleUpdate, cannot remove.");
        }

        public void Update()
        {
            // Process all Currently Registered (Copy as Array)
            foreach(var update in updateList.ToArray())
            {
                update?.Invoke();
            }

            // Remove all nulls (Destroyed Objects) in updateList
            updateList.RemoveAll(o => o == null);
        }
    }
}


