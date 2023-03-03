using System;

namespace GameplayIngredients.Rigs
{
    public abstract class AnimationHandler<T> : AnimationHandler
    {
        public sealed override Type animatedType => typeof(T);

        public abstract void OnStart(T defaultValue);

        public abstract T OnUpdate(float deltaTime);
    }

    public abstract class AnimationHandler 
    {
        public abstract Type animatedType { get; }
    }

}
