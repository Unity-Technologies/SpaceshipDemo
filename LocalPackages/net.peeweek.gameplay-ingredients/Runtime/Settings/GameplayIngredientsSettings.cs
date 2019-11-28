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
        public bool allowUpdateCalls { get { return m_AllowUpdateCalls; } }

        public bool disableWelcomeScreenAutoStart { get { return m_DisableWelcomeScreenAutoStart; } }

        [BoxGroup("Editor")]
        [SerializeField]
        protected bool m_DisableWelcomeScreenAutoStart;

        [BoxGroup("Managers")]
        [SerializeField, ReorderableList, TypeDropDown(typeof(Manager))]
        protected string[] m_ExcludedManagers;

        [BoxGroup("Callables")]
        [SerializeField, InfoBox("Verbose Calls enable logging at runtime, this can lead to performance drop, use only when debugging.", InfoBoxType.Warning, "m_VerboseCalls")]
        private bool m_VerboseCalls = false;

        [BoxGroup("Callables")]
        [SerializeField, InfoBox("Per-update calls should be avoided due to high performance impact. Enable and use with care, only if strictly necessary.", InfoBoxType.Warning, "m_AllowUpdateCalls")]
        private bool m_AllowUpdateCalls = false;

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
