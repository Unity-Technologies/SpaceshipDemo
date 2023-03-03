using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameplayIngredients.Rigs
{
    [AddComponentMenu(ComponentMenu.managersPath + "Rig Manager")]
    [NonExcludeableManager]
    public class RigManager : Manager
    {
        Dictionary<int, List<Rig>> m_UpdateRigs;
        Dictionary<int, List<Rig>> m_LateUpdateRigs;
        Dictionary<int, List<Rig>> m_FixedUpdateRigs;

        private void OnEnable()
        {
            m_UpdateRigs = new Dictionary<int, List<Rig>>();
            m_LateUpdateRigs = new Dictionary<int, List<Rig>>();
            m_FixedUpdateRigs = new Dictionary<int, List<Rig>>();
        }

        public void RegistedRig(Rig rig)
        {
            Rig.UpdateMode updateMode;
            if (rig.canChangeUpdateMode)
                updateMode = rig.updateMode;
            else
                updateMode = rig.defaultUpdateMode;

            Dictionary<int, List<Rig>> dict;
            switch (updateMode)
            {
                default:
                case Rig.UpdateMode.Update:
                    dict = m_UpdateRigs;
                    break;
                case Rig.UpdateMode.LateUpdate:
                    dict = m_LateUpdateRigs;
                    break;
                case Rig.UpdateMode.FixedUpdate:
                    dict = m_FixedUpdateRigs;
                    break;
            }

            if(!dict.ContainsKey(rig.rigPriority))
            {
                dict.Add(rig.rigPriority, new List<Rig>());
            }

            dict[rig.rigPriority].Add(rig);
        }

        public void RemoveRig(Rig rig)
        {
            Dictionary<int, List<Rig>> dict;

            Rig.UpdateMode updateMode;
            if (rig.canChangeUpdateMode)
                updateMode = rig.updateMode;
            else
                updateMode = rig.defaultUpdateMode;

            switch (updateMode)
            {
                default:
                case Rig.UpdateMode.Update:
                    dict = m_UpdateRigs;
                    break;
                case Rig.UpdateMode.LateUpdate:
                    dict = m_LateUpdateRigs;
                    break;
                case Rig.UpdateMode.FixedUpdate:
                    dict = m_FixedUpdateRigs;
                    break;
            }

            int priority = rig.rigPriority;

            if (dict.ContainsKey(priority) && dict[priority].Contains(rig))
            {
                dict[priority].Remove(rig);
            }
            else
            {
                Debug.LogError($"Could not remove rig {rig.gameObject.name} ({rig.GetType().Name}) from {updateMode}/#{priority.ToString()}");
            }

            if(dict.ContainsKey(priority) && dict[priority].Count == 0)
            {
                dict.Remove(priority);
            }
        }

        void UpdateRigDictionary(Dictionary<int, List<Rig>> dict, float deltaTime)
        {
            var priorities = dict.Keys.OrderBy(i => i);
            foreach(int priority in priorities)
            {
                if(dict.ContainsKey(priority))
                {
                    foreach (var rig in dict[priority].ToList())
                    {
                        rig.UpdateRig(deltaTime);
                    }
                }

            }
        }

        public void Update()
        {
            UpdateRigDictionary(m_UpdateRigs, Time.deltaTime);
        }

        public void FixedUpdate()
        {
            UpdateRigDictionary(m_FixedUpdateRigs, Time.fixedDeltaTime);
        }

        public void LateUpdate()
        {
            UpdateRigDictionary(m_LateUpdateRigs, Time.deltaTime);
        }
    }
}

