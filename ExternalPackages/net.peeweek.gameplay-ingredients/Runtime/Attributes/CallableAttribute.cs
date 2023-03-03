using System;

namespace GameplayIngredients
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CallableAttribute : Attribute
    {
        public string category;
        public string iconPath;
        public CallableAttribute(string category = "", string iconName = "")
        {
            this.category = category;
            this.iconPath = iconName;
        }
    }
}


