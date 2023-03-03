using UnityEngine;
using UnityEditor;
using System.Linq;
using GameplayIngredients.Actions;
using System;
using GameplayIngredients.Events;

namespace GameplayIngredients
{
    static class HierarchyItems
    {
        const string kHierarchyMenu = "GameObject/Gameplay Ingredients/";
        const string kEventsMenu = kHierarchyMenu+"Events/";
        const string kSMMenu = kHierarchyMenu+ "State Machines/";

        static GameObject CreateGameObject(string name, params Type[] components)
        {
            var go = new GameObject(name, components);

            if (Selection.activeGameObject != null)
                go.transform.parent = Selection.activeGameObject.transform;

            Selection.activeGameObject = go;
            return go;
        }

        #region STATE MACHINES
        [MenuItem(kSMMenu+"State Machine (Empty)", false, 10)]
        static void CreateEmptyStateMachine()
        {
            var go = new GameObject("New StateMachine");
            var sm = go.AddComponent<StateMachines.StateMachine>();

            if (Selection.activeGameObject != null)
                go.transform.parent = Selection.activeGameObject.transform;
        }

        [MenuItem(kSMMenu + "State Machine (On|Off)", false, 10)]
        static void CreateTwoStateStateMachine()
        {
            var go = new GameObject("New StateMachine");
            var sm = go.AddComponent<StateMachines.StateMachine>();

            AddState(sm, "On");
            AddState(sm, "Off");

            sm.DefaultState = "On";

            if (Selection.activeGameObject != null)
                go.transform.parent = Selection.activeGameObject.transform;
        }

        [MenuItem(kSMMenu + "State Machine (On|Off|Disabled)", false, 10)]
        static void CreateThreeStateStateMachine()
        {
            var go = new GameObject("New StateMachine");
            var sm = go.AddComponent<StateMachines.StateMachine>();

            AddState(sm, "Disabled");
            AddState(sm, "On");
            AddState(sm, "Off");

            sm.DefaultState = "Disabled";

            if (Selection.activeGameObject != null)
                go.transform.parent = Selection.activeGameObject.transform;
        }

        static StateMachines.State AddState(StateMachines.StateMachine sm, string name)
        {
            var goState = new GameObject(name);
            var state = goState.AddComponent<StateMachines.State>();
            goState.transform.parent = sm.gameObject.transform;
            goState.transform.localPosition = Vector3.zero;
            goState.transform.localRotation = Quaternion.identity;
            goState.transform.localScale = Vector3.one;
            sm.States = sm.States.Concat(new StateMachines.State[] { state }).ToArray();
            return state;
        }

        #endregion

        #region TRIGGERS

        [MenuItem(kEventsMenu + "On Trigger (Box)", false, 10)]
        static void CreateTriggerBox()
        {
            var go = CreateGameObject("On Trigger (Box)", typeof(BoxCollider), typeof(OnTriggerEvent));
            var col = go.GetComponent<BoxCollider>();
            col.isTrigger = true;
        }

        [MenuItem(kEventsMenu + "On Trigger (Sphere)", false, 10)]
        static void CreateTriggerSphere()
        {
            var go = CreateGameObject("On Trigger (Sphere)", typeof(SphereCollider), typeof(OnTriggerEvent));
            var col = go.GetComponent<SphereCollider>();
            col.isTrigger = true;
        }

        [MenuItem(kEventsMenu + "On Trigger (Capsule)", false, 10)]
        static void CreateTriggerCapsule()
        {
            var go = CreateGameObject("On Trigger (Capsule)", typeof(CapsuleCollider), typeof(OnTriggerEvent));
            var col = go.GetComponent<CapsuleCollider>();
            col.isTrigger = true;
        }
        [MenuItem(kEventsMenu + "On Collider (Box)", false, 10)]
        static void CreateColliderBox()
        {
            var go = CreateGameObject("On Collider (Box)", typeof(BoxCollider), typeof(OnColliderEvent));
        }

        [MenuItem(kEventsMenu + "On Collider (Sphere)", false, 10)]
        static void CreateColliderSphere()
        {
            var go = CreateGameObject("On Collider (Sphere)", typeof(SphereCollider), typeof(OnColliderEvent));
        }

        [MenuItem(kEventsMenu + "On Collider (Capsule)", false, 10)]
        static void CreateColliderCapsule()
        {
            var go = CreateGameObject("On Collider (Capsule)", typeof(CapsuleCollider), typeof(OnColliderEvent));
        }
        #endregion

        #region EVENTS

        [MenuItem(kEventsMenu+"On Awake", false, 30)]
        static void CreateOnAwake() => CreateGameObject("On Awake", typeof(OnAwakeEvent));

        [MenuItem(kEventsMenu + "On Destroyed", false, 31)]
        static void CreateOnDestroyed() => CreateGameObject("On Destroyed", typeof(OnDestroyEvent));

        [MenuItem(kEventsMenu + "On Enable~Disable", false, 32)]
        static void CreateOnEnableDisable() => CreateGameObject("On Enable/Disable", typeof(OnEnableDisableEvent));

        [MenuItem(kEventsMenu + "On Start", false, 33)]
        static void CreateOnStart() => CreateGameObject("On Start", typeof(OnStartEvent));

        [MenuItem(kEventsMenu + "On Update", true, 34)]
        static bool CanCreateOnUpdate() => GameplayIngredientsSettings.currentSettings.allowUpdateCalls;

        [MenuItem(kEventsMenu + "On Update", false, 34)]
        static void CreateOnUpdate() => CreateGameObject("On Update", typeof(OnUpdateEvent));

        [MenuItem(kEventsMenu + "On Game Manager Start", false, 50)]
        static void CreateOnGameManagerStart() => CreateGameObject("On Level Start", typeof(OnGameManagerLevelStart));

        [MenuItem(kEventsMenu + "On Message Received", false, 51)]
        static void CreateOnMessageReceived() => CreateGameObject("On Message Received", typeof(OnMessageEvent));

        [MenuItem(kEventsMenu + "On Button Down", false, 70)]
        static void CreateOnButtonDownEvent() => CreateGameObject("On Button Down", typeof(OnButtonDownEvent));

        [MenuItem(kEventsMenu + "On Key Down", false, 71)]
        static void CreateOnKewDownEvent() => CreateGameObject("On Key Down", typeof(OnKeyDownEvent));

        [MenuItem(kEventsMenu + "On Mouse Down", false, 72)]
        static void CreateOnMouseDownEvent() => CreateGameObject("On Mouse Down", typeof(OnMouseDownEvent));

        [MenuItem(kEventsMenu + "On Mouse Hover (Sphere)", false, 73)]
        static void CreateOnMouseHoverEventSphere() => CreateGameObject("On Mouse Hover", typeof(SphereCollider), typeof(OnMouseHoverEvent));

        [MenuItem(kEventsMenu + "On Mouse Hover (Box)", false, 73)]
        static void CreateOnMouseHoverEventBox() => CreateGameObject("On Mouse Hover", typeof(BoxCollider), typeof(OnMouseHoverEvent));

        #endregion

        #region UTILS
        [MenuItem(kHierarchyMenu + "Factory", false, 10)]
        static void CreateFactory()
        {
            var go = CreateGameObject("Factory", typeof(Factory), typeof(FactorySpawnAction));
            var fact = go.GetComponent<Factory>();
            fact.SpawnTarget = go;
            var sa = go.GetComponent<FactorySpawnAction>();
            sa.factory = fact;
        }

        [MenuItem(kHierarchyMenu + "Counter", false, 10)]
        static void CreateCounter()
        {
            var go = CreateGameObject("Counter", typeof(Counter), typeof(CounterAction));
            var ca = go.GetComponent<CounterAction>();
            ca.Counters = new Counter[] { go.GetComponent<Counter>() };
        }

        [MenuItem(kHierarchyMenu + "Timer", false, 10)]
        static void CreateTimer()
        {
            var go = CreateGameObject("Timer", typeof(Timer), typeof(TimerAction));
            var ca = go.GetComponent<TimerAction>();
            ca.timer = go.GetComponent<Timer>();
        }

        [MenuItem(kHierarchyMenu + "Pickup Item (Sphere)", false, 10)]
        static void CreatePickup()
        {
            var go = CreateGameObject("Pickup", typeof(SphereCollider), typeof(Pickup.PickupItem));
            go.GetComponent<SphereCollider>().isTrigger = true;
        }


        #endregion
    }
}

