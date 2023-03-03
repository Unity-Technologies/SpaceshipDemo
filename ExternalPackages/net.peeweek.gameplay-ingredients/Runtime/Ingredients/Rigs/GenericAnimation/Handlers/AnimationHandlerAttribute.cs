using System;

namespace GameplayIngredients.Rigs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AnimationHandlerAttribute : Attribute
    {
        public string menuPath;
        public Type type;

        public AnimationHandlerAttribute(string menuPath, Type type)
        {
            this.type = type;
            this.menuPath = menuPath;
        }
    }
}


