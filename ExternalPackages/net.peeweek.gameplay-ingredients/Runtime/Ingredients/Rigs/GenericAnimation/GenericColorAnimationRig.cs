using NaughtyAttributes;
using System;
using UnityEngine;

namespace GameplayIngredients.Rigs
{
    public class GenericColorAnimationRig : GenericAnimationRig
    {
        [Header("Base Value")]
        [SerializeField]
        bool useStoredValueAsBase = true;
        [SerializeField, DisableIf("useStoredValueAsBase")]
        Color baseValue = Color.white;

        [Header("Animation")]
        [SerializeReference, HandlerType(typeof(Color))]
        public ColorAnimationHandler animationHandler = new ColorGradientAnimationHandler();

        public override Type animatedType => typeof(Color);

        private void Awake()
        {
            if (useStoredValueAsBase)
                baseValue = (Color)property.GetValue();
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

