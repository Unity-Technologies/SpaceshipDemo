using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    partial class WelcomeScreen : EditorWindow
    {
        const string kShowOnStartupPreference = "GameplayIngredients.Welcome.ShowAtStartup";
        const int WindowWidth = 640;
        const int WindowHeight = 520;

        static bool showOnStartup
        {
            get { return EditorPrefs.GetBool(kShowOnStartupPreference, true); }
            set { if (value != showOnStartup) EditorPrefs.SetBool(kShowOnStartupPreference, value); }
        }

        static Texture2D header
        {
            get
            {
                if (s_Header == null)
                    s_Header = (Texture2D)EditorGUIUtility.Load("Packages/net.peeweek.gameplay-ingredients/Editor/WelcomeScreen/welcome-title.png");

                return s_Header;
            }
        }

        static Texture2D s_Header;

        public static void Reload()
        {
            EditorApplication.update -= ShowAtStartup;
            InitShowAtStartup();
        }

        [InitializeOnLoadMethod]
        static void InitShowAtStartup()
        {
            if (showOnStartup && !GameplayIngredientsSettings.currentSettings.disableWelcomeScreenAutoStart)
                EditorApplication.update += ShowAtStartup;
        }

        static void ShowAtStartup()
        {
            if (!Application.isPlaying)
            {
                ShowFromMenu();
            }
            EditorApplication.update -= ShowAtStartup;
        }

        [MenuItem("Window/Gameplay Ingredients/Welcome Screen", priority = MenuItems.kWindowMenuPriority)]
        static void ShowFromMenu()
        {
            GetWindow<WelcomeScreen>(true, "Gameplay Ingredients");
        }

        private void OnEnable()
        {
            this.position = new Rect((Screen.width / 2.0f) - WindowWidth / 2, (Screen.height / 2.0f) - WindowHeight / 2, WindowWidth, WindowHeight);
            this.minSize = new Vector2(WindowWidth, WindowHeight);
            this.maxSize = new Vector2(WindowWidth, WindowHeight);

            if (!GameplayIngredientsSettings.hasSettingAsset)
                wizardMode = WizardMode.FirstTimeSetup;

            InitTips();
        }

        private void OnDestroy()
        {
            EditorApplication.update -= ShowAtStartup;
        }

        private enum WizardMode
        {
            TipOfTheDay = 0,
            FirstTimeSetup = 1,
            About = 2,
        }

        [SerializeField]
        private WizardMode wizardMode = WizardMode.TipOfTheDay;

        private void OnGUI()
        {
            Rect headerRect = GUILayoutUtility.GetRect(640, 215);
            GUI.DrawTexture(headerRect, header);
            using (new GUILayout.AreaScope(new Rect(160, 180, 320, 32)))
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    using (new GUILayout.HorizontalScope(EditorStyles.miniButton))
                    {
                        bool value = false;

                        value = wizardMode == WizardMode.TipOfTheDay;
                        value = GUILayout.Toggle(value, "  Tips  ", Styles.buttonLeft); 
                        if(value)
                            wizardMode = WizardMode.TipOfTheDay;

                        value = wizardMode == WizardMode.FirstTimeSetup;
                        value = GUILayout.Toggle(value, "  Setup  ", Styles.buttonMid);
                        if (value)
                            wizardMode = WizardMode.FirstTimeSetup;

                        value = wizardMode == WizardMode.About;
                        value = GUILayout.Toggle(value, "  About  ", Styles.buttonRight);
                        if(value)
                            wizardMode = WizardMode.About;
                    }

                    GUILayout.FlexibleSpace();
                }
            }
            GUILayout.Space(8);

            switch (wizardMode)
            {
                case WizardMode.TipOfTheDay:
                    OnTipsGUI();
                    break;
                case WizardMode.FirstTimeSetup:
                    OnSetupGUI();
                    break;
                case WizardMode.About:
                    OnAboutGUI();
                    break;
            }

            Rect line = GUILayoutUtility.GetRect(640, 1);
            EditorGUI.DrawRect(line, Color.black);
            using (new GUILayout.HorizontalScope())
            {
                if(!GameplayIngredientsSettings.currentSettings.disableWelcomeScreenAutoStart)
                {
                    showOnStartup = GUILayout.Toggle(showOnStartup, " Show this window on startup");
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Close"))
                {
                    this.Close();
                }
            }
        }

        void OnAboutGUI()
        {
            GUILayout.Label("About", EditorStyles.boldLabel);

            using (new GUILayout.VerticalScope(Styles.helpBox))
            {

                GUILayout.Label("Gameplay Ingredients", Styles.centeredTitle);
                GUILayout.Label(@"(and cooking ustensils)

A set of Open Source Runtime and Editor Tools for your Unity prototypes and games. These scripts are maintained by Thomas Iche and released under MIT License as a unity package. 

<b>This package also makes use of the following third party components:</b>
- <i>Naughty Attributes</i> by Denis Rizov (https://github.com/dbrizov) 
- <i>Fugue Icons</i> by Yusuke Kamiyamane (https://p.yusukekamiyamane.com/).
- <i>Header art background 'Chef's Station'</i> by Todd Quackenbush (https://unsplash.com/photos/x5SRhkFajrA).
", Styles.centeredBody);

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("  Github Page  ", Styles.buttonLeft))
                        Application.OpenURL("https://github.com/peeweek/net.peeweek.gameplay-ingredients");
                    if (GUILayout.Button("  Report a Bug  ", Styles.buttonMid))
                        Application.OpenURL("https://github.com/peeweek/net.peeweek.gameplay-ingredients/issues");
                    if (GUILayout.Button("  LICENSE  ", Styles.buttonRight))
                        Application.OpenURL("https://github.com/peeweek/net.peeweek.gameplay-ingredients/blob/master/LICENSE");

                    GUILayout.FlexibleSpace();
                }

                GUILayout.FlexibleSpace();
            }
        }

        static class Styles
        {
            public static GUIStyle buttonLeft;
            public static GUIStyle buttonMid;
            public static GUIStyle buttonRight;
            public static GUIStyle title;
            public static GUIStyle body;

            public static GUIStyle centeredTitle;
            public static GUIStyle centeredBody;
            public static GUIStyle helpBox;

            static Styles()
            {
                buttonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
                buttonMid = new GUIStyle(EditorStyles.miniButtonMid);
                buttonRight = new GUIStyle(EditorStyles.miniButtonRight);
                buttonLeft.fontSize = 12;
                buttonMid.fontSize = 12;
                buttonRight.fontSize = 12;

                title = new GUIStyle(EditorStyles.label);
                title.fontSize = 22;

                centeredTitle = new GUIStyle(title);
                centeredTitle.alignment = TextAnchor.UpperCenter;

                body = new GUIStyle(EditorStyles.label);
                body.fontSize = 12;
                body.wordWrap = true;
                body.richText = true;

                centeredBody = new GUIStyle(body);
                centeredBody.alignment = TextAnchor.UpperCenter;

                helpBox = new GUIStyle(EditorStyles.helpBox);
                helpBox.padding = new RectOffset(12, 12, 12, 12);
            }
        }
    }
}
