using NaughtyAttributes;
using System;
using UnityEngine;

namespace GameplayIngredients.Rigs
{
    public class GenericVector3AnimationRig : GenericAnimationRig
    {
        [Header("Base Value")]
        [SerializeField]
        bool useStoredValueAsBase = true;
        [SerializeField, DisableIf("useStoredValueAsBase")]
        Vector3 baseValue = Vector3.one;

        [Header("Animation")]
        [SerializeReference, HandlerType(typeof(Vector3))]
        public Vector3AnimationHandler animationHandler = new Vector3ContinuousAnimationHandler();

        public override Type animatedType => typeof(Vector3);

        private void Awake()
        {
            if (useStoredValueAsBase)
                baseValue = (Vector3)property.GetValue();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            animationHandler?.OnStart(baseValue);
        }

        protected override object UpdateAndGetValue(float deltaTime)
        {
            if (animationHandler != null)
            {
                return animationHandler.OnUpdate(deltaTime);
            }
            else
            {
                Debug.LogWarning("Float Animation Rig has no Animation Handler", this);
                return baseValue;
            }

        }
    }
}

