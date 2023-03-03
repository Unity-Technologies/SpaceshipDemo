using System;
using UnityEngine;

namespace GameplayIngredients.Rigs
{

    public abstract class GenericAnimationRig : Rig
    {
        public override UpdateMode defaultUpdateMode => UpdateMode.Update;

        public override int defaultPriority => 0;

        public override bool canChangeUpdateMode => true;

        public abstract Type animatedType { get; }

        [Header("Target Property / Field"), ReflectedMember("animatedType")]
        public ReflectedMember property;

        public override void UpdateRig(float deltaTime)
        {
            if (property.targetObject != null)
            {
                property.SetValue(UpdateAndGetValue(deltaTime));
            }
        }

        protected abstract object UpdateAndGetValue(float deltaTime);

    }
}
