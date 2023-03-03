using System;

namespace GameplayIngredients
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WarnDisabledModuleAttribute : Attribute
    {
        public string module;
        public string fixLocation;

        public WarnDisabledModuleAttribute(string module, string fixLocation = "Package Manager")
        {
            this.module = module;
            this.fixLocation = fixLocation;
        }
    }
}


