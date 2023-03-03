using UnityEngine;
using NaughtyAttributes;


namespace GameplayIngredients.Actions
{
    public enum AnimatorParameterType { Bool, Float, Int, Trigger };

    [AddComponentMenu(ComponentMenu.actionsPath + "Set Animator Parameter Action")]
    [Callable("Game", "Actions/ic-generic-action.png")]
    public class SetAnimatorParameterAction : ActionBase
    {
        public Animator animator;
        public string parameterName;
        [OnValueChanged("OnParameterTypeChanged")]
        public AnimatorParameterType parameterType = AnimatorParameterType.Bool;

        bool showFloat;
        bool showInt;
        bool showBool = true;
        [ShowIf("showFloat")]
        public float floatValue;
        [ShowIf("showInt")]
        public int intValue;
        [ShowIf("showBool")]
        public bool boolValue;

        public override void Execute(GameObject instigator = null)
        {
            if (animator == null)
                return;

            switch (parameterType)
            {
                case AnimatorParameterType.Bool:
                    animator.SetBool(parameterName, boolValue);
                    break;
                case AnimatorParameterType.Float:
                    animator.SetFloat(parameterName, floatValue);
                    break;
                case AnimatorParameterType.Int:
                    animator.SetInteger(parameterName, intValue);
                    break;
                case AnimatorParameterType.Trigger:
                    animator.SetTrigger(parameterName);
                    break;
            }       
        }

        private void OnParameterTypeChanged()
        {
            showBool = (parameterType == AnimatorParameterType.Bool);
            showFloat = (parameterType == AnimatorParameterType.Float);
            showInt = (parameterType == AnimatorParameterType.Int);
        }

        public override string GetDefaultName()
        {
            string value = "";
            switch (parameterType)
            {
                case AnimatorParameterType.Bool: 
                    value = boolValue.ToString(); 
                    break;
                case AnimatorParameterType.Float:
                    value = floatValue.ToString(); 
                    break;
                case AnimatorParameterType.Int:
                    value = intValue.ToString();
                    break;
                case AnimatorParameterType.Trigger:
                    value = "Trigger";
                    break;
                default:
                    break;
            }

            return $"Set Animator '{parameterName}' {parameterType} Parameter = '{value}' ({animator?.gameObject.name})";
        }
    }
}