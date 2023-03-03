#if UNITY_2021_2_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using GameplayIngredients.Comments.Editor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine.UIElements;

namespace GameplayIngredients.Editor
{
    public static partial class SceneViewToolbar
    {

        [Overlay(typeof(SceneView), "Gameplay Ingredients", true)]
        public class IngredientsToolbarOverlay : ToolbarOverlay
        {
            const string prefix = "IngredientsToolbarOverlay.";
            public IngredientsToolbarOverlay() : base(
                PlayFromHereButton.id,
                LinkGameViewButton.id,
                PointOfViewButton.id,
                CommentsButton.id,
                CheckResolveButton.id,
                IngredientExplorerButton.id
                )
            { }

            protected override Layout supportedLayouts => Layout.HorizontalToolbar | Layout.VerticalToolbar;

            #region PLAY FROM HERE
            [EditorToolbarElement(id)]
            public class PlayFromHereButton : EditorToolbarButton, IAccessContainerWindow
            {
                public const string id = prefix + "PlayFromHereButton";
                public EditorWindow containerWindow { get; set; }

                public PlayFromHereButton() : base()
                {
                    this.SetEnabled(PlayFromHere.IsReady);
                    icon = EditorApplication.isPlaying ? Contents.playFromHere_Stop : Contents.playFromHere;
                    tooltip = "Play from Here";
                    clicked += OnClick;
                    EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
                }

                private void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
                {
                    if (obj == PlayModeStateChange.EnteredPlayMode)
                        icon = Contents.playFromHere_Stop;
                    else if (obj == PlayModeStateChange.EnteredEditMode)
                        icon = Contents.playFromHere;
                }

                public void OnClick()
                {
                    if (!EditorApplication.isPlaying)
                        PlayFromHere.Play(containerWindow as SceneView);
                    else
                        EditorApplication.isPlaying = false;
                }
            }
            #endregion

            #region LINK GAME VIEW
            [EditorToolbarElement(id)]
            public class LinkGameViewButton : EditorToolbarDropdownToggle, IAccessContainerWindow
            {
                public override bool value
                {
                    get => base.value;
                    set
                    {
                        base.value = value;
                        OnValueChanged(value);
                    }
                }

                public const string id = prefix + "LinkGameViewButton";

                public EditorWindow containerWindow { get; set; }

                static List<LinkGameViewButton> buttons = new List<LinkGameViewButton>();

                public LinkGameViewButton()
                {
                    tooltip = "Link Game View";
                    dropdownClicked += OnClick;

                    SetValueWithoutNotify(LinkGameView.Active && LinkGameView.LockedSceneView == containerWindow as SceneView);

                    buttons.Add(this);
                    EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
                    UpdateIcon();

                }

                private void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
                {
                    if(obj == PlayModeStateChange.EnteredPlayMode || obj == PlayModeStateChange.EnteredEditMode)
                        UpdateIcon();
                }

                ~LinkGameViewButton()
                {
                    if (LinkGameView.LockedSceneView == containerWindow as SceneView)
                    {
                        LinkGameView.LockedSceneView = null;
                        LinkGameView.Active = false;
                    }
                    buttons.Remove(this);
                }

                public void UpdateIcon()
                {
                    if (LinkGameView.CinemachineActive)
                        icon = Contents.linkGameViewCM;
                    else
                    {
                        icon = (LinkGameView.Active && LinkGameView.LockedSceneView == containerWindow as SceneView) ? Contents.linkGameViewActive : Contents.linkGameView;
                    }
                }

                public void OnClick()
                {
                    var m = new GenericMenu();
                    m.AddItem(new GUIContent("Link Camera"), LinkGameView.LockedSceneView == containerWindow as SceneView,
                        () =>
                        {
                            LinkGameView.CinemachineActive = false;
                            value = true;
                            UpdateIcon();
                        });
                    m.AddItem(new GUIContent("Cinemachine Preview"), LinkGameView.CinemachineActive,
                        () =>
                        {
                            LinkGameView.CinemachineActive = true;
                            value = true;
                            UpdateIcon();
                        });
                    m.DropDown(worldBound);

                }

                void OnValueChanged(bool newValue)
                {
                    LinkGameView.Active = newValue;

                    if (LinkGameView.CinemachineActive && newValue == false)
                        LinkGameView.CinemachineActive = false;
                    else if (newValue && !LinkGameView.CinemachineActive)
                        LinkGameView.LockedSceneView = containerWindow as SceneView;
                    else
                        LinkGameView.LockedSceneView = null;

                    UpdateIcon();

                    foreach (var button in buttons)
                    {
                        if (LinkGameView.CinemachineActive)
                        {
                            button.SetValueWithoutNotify(true);

                        }
                        else if (button.containerWindow != containerWindow)
                        {
                            button.SetValueWithoutNotify(false);
                        }
                        button.UpdateIcon();
                    }
                }
            }
            #endregion

            #region POINT OF VIEW
            [EditorToolbarElement(id)]
            public class PointOfViewButton : EditorToolbarDropdown, IAccessContainerWindow
            {
                public const string id = prefix + "PointOfViewButton";

                public EditorWindow containerWindow { get; set; }

                public PointOfViewButton()
                {
                    icon = Contents.pointOfView;
                    tooltip = "Point of View";
                    clicked += OnClick;
                }

                public void OnClick()
                {
                    SceneViewPOV.ShowPopup(worldBound, containerWindow as SceneView);
                }
            }
            #endregion

            #region COMMENTS
            [EditorToolbarElement(id)]
            public class CommentsButton : EditorToolbarButton, IAccessContainerWindow
            {
                public const string id = prefix + "CommentsButton";

                public EditorWindow containerWindow { get; set; }

                public CommentsButton()
                {
                    icon = Contents.commentsWindow;
                    tooltip = "Open Comments Window";
                    clicked += OnClick;
                }

                public void OnClick()
                {
                    CommentsWindow.Open();
                }
            }
            #endregion

            #region CHECK/RESOLVE
            [EditorToolbarElement(id)]
            public class CheckResolveButton : EditorToolbarButton, IAccessContainerWindow
            {
                public const string id = prefix + "CheckResolveButton";

                public EditorWindow containerWindow { get; set; }

                public CheckResolveButton()
                {
                    icon = Contents.checkWindow;
                    tooltip = "Open Check/Resolve Window";
                    clicked += OnClick;
                }

                public void OnClick()
                {
                    CheckWindow.OpenWindow();
                }
            }
            #endregion

            #region INGREDIENT EXPLORER
            [EditorToolbarElement(id)]
            public class IngredientExplorerButton : EditorToolbarButton, IAccessContainerWindow
            {
                public const string id = prefix + "IngredientExplorerButton";

                public EditorWindow containerWindow { get; set; }

                public IngredientExplorerButton()
                {
                    icon = Contents.ingredientsExplorer;
                    tooltip = "Open Ingredients Explorer";
                    clicked += OnClick;
                }

                public void OnClick()
                {
                    IngredientsExplorerWindow.OpenWindow();
                }
            }
            #endregion

            static class Contents
            {
                public static Texture2D playFromHere;
                public static Texture2D playFromHere_Stop;

                public static Texture2D pointOfView;

                public static Texture2D linkGameView;
                public static Texture2D linkGameViewActive;
                public static Texture2D linkGameViewCM;

                public static Texture2D checkWindow;
                public static Texture2D commentsWindow;
                public static Texture2D ingredientsExplorer;

                static Contents()
                {
                    playFromHere = EditorGUIUtility.Load("Packages/net.peeweek.gameplay-ingredients/Icons/SceneViewToolbar/PlayFromHere.png") as Texture2D;
                    playFromHere_Stop = EditorGUIUtility.Load("Packages/net.peeweek.gameplay-ingredients/Icons/SceneViewToolbar/PlayFromHere_Stop.png") as Texture2D;
                    pointOfView = EditorGUIUtility.Load("Packages/net.peeweek.gameplay-ingredients/Icons/SceneViewToolbar/POV.png") as Texture2D;
                    linkGameView = EditorGUIUtility.Load("Packages/net.peeweek.gameplay-ingredients/Icons/SceneViewToolbar/Camera.png") as Texture2D;

                    linkGameViewActive = EditorGUIUtility.Load("Packages/net.peeweek.gameplay-ingredients/Icons/SceneViewToolbar/CameraActive.png") as Texture2D;
                    linkGameViewCM = EditorGUIUtility.Load("Packages/net.peeweek.gameplay-ingredients/Icons/SceneViewToolbar/CameraCM.png") as Texture2D;
                    checkWindow = EditorGUIUtility.Load("Packages/net.peeweek.gameplay-ingredients/Icons/SceneViewToolbar/CheckResolve.png") as Texture2D;
                    commentsWindow = EditorGUIUtility.Load("Packages/net.peeweek.gameplay-ingredients/Icons/SceneViewToolbar/Comments.png") as Texture2D;
                    ingredientsExplorer = EditorGUIUtility.Load("Packages/net.peeweek.gameplay-ingredients/Icons/Misc/ic-callable.png") as Texture2D;
                }
            }
        }

        [Overlay(typeof(SceneView), "Custom (Gameplay Ingredients)", true)]
        public class IngredientsCustomToolbarOverlay : ToolbarOverlay, IAccessContainerWindow
        {
            EditorWindow IAccessContainerWindow.containerWindow { get; set; }

            SceneView sceneView => containerWindow as SceneView;

            protected override Layout supportedLayouts => Layout.HorizontalToolbar;

            public override VisualElement CreatePanelContent()
            {
                return new IMGUIContainer(() =>
                {
                    using(new GUILayout.HorizontalScope(EditorStyles.toolbar))
                    {
                        SceneViewToolbar.OnSceneViewToolbarGUI?.Invoke(sceneView);
                    }
                });
            }

        }
    }
}
#endif
