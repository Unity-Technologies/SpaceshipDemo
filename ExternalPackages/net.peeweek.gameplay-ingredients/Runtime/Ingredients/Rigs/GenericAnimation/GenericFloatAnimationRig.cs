using NaughtyAttributes;
using System;
using UnityEngine;

namespace GameplayIngredients.Rigs
{
    public class GenericFloatAnimationRig : GenericAnimationRig
    {
        [Header("Base Value")]
        [SerializeField]
        bool useStoredValueAsBase = true;
        [SerializeField, DisableIf("useStoredValueAsBase")]
        float baseValue = 1.0f;
        
        [Header("Animation")]
        [SerializeReference, HandlerType(typeof(float))]
        public FloatAnimationHandler animationHandler = new FloatContinuousAnimationHandler();


        public override Type animatedType => typeof(float);

        private void Awake()
        {
            if (useStoredValueAsBase)
                baseValue = (float)property.GetValue();    
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

