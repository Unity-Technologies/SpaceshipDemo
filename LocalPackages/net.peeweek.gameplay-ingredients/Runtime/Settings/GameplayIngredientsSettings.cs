using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace GameplayIngredients
{
    public class GameplayIngredientsSettings : ScriptableObject
    {
        public string[] excludedeManagers { get { return m_ExcludedManagers; } }
        public bool verboseCalls { get { return m_VerboseCalls; } }
        public bool disableWelcomeScreenAutoStart { get { return m_DisableWelcomeScreenAutoStart; } }

        [BoxGroup("Editor")]
        [SerializeField]
        protected bool m_DisableWelcomeScreenAutoStart;

        [BoxGroup("Managers")]
        [SerializeField, ReorderableList, TypeDropDown(typeof(Manager))]
        protected string[] m_ExcludedManagers;

        [BoxGroup("Callables")]
        [SerializeField, InfoBox("Verbose Calls enable logging at runtime, this can lead to performance drop, use only when debugging.",InfoBoxType.Warning, "m_VerboseCalls")]
        protected bool m_VerboseCalls;

        const string kAssetName = "GameplayIngredientsSettings";

        public static GameplayIngredientsSettings currentSettings
        {
            get
            {
                if (hasSettingAsset)
                    return Resources.Load<GameplayIngredientsSettings>(kAssetName);
                else
                    return defaultSettings;
            }
        }

        public static bool hasSettingAsset
        {
            get
            {
                return Resources.Load<GameplayIngredientsSettings>(kAssetName) != null;
            }
        }


        public static GameplayIngredientsSettings defaultSettings
        {
            get
            {
                if (s_DefaultSettings == null)
                    s_DefaultSettings = CreateDefaultSettings();
                return s_DefaultSettings;
            }
        }

        static GameplayIngredientsSettings s_DefaultSettings;

        static GameplayIngredientsSettings CreateDefaultSettings()
        {
            var defaultAsset = CreateInstance<GameplayIngredientsSettings>();
            defaultAsset.m_VerboseCalls = false;
            defaultAsset.m_ExcludedManagers = new string[0];
            defaultAsset.m_DisableWelcomeScreenAutoStart = false;
            return defaultAsset;
        }
    }
}
