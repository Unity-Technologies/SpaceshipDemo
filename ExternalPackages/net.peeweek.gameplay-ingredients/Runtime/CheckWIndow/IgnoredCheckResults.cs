using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients
{
    [AddComponentMenu(ComponentMenu.miscPath + "Ignored Check Results (Do not use directly)")]
    public class IgnoredCheckResults : MonoBehaviour
    {
        [System.Serializable]
        public struct IgnoredCheckResult
        {
            public string check;
            public GameObject gameObject;
        }

        public List<IgnoredCheckResult> ignoredCheckResults;

    }
}


