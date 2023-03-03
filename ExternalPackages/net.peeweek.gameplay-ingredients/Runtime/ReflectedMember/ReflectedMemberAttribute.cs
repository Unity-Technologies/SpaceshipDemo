using UnityEngine;

namespace GameplayIngredients
{
    public class ReflectedMemberAttribute : PropertyAttribute
    {
        public string typeMemberName;

        public ReflectedMemberAttribute(string typeMemberName)
        {
            this.typeMemberName = typeMemberName;
        }
    }
}
