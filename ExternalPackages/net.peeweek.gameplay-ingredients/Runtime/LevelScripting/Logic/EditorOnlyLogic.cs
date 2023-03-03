using UnityEngine;

namespace GameplayIngredients.Logic
{
    [AddComponentMenu(ComponentMenu.logicPath + "Editor Only Logic")]
    [Callable("Application", "Logic/ic-generic-logic.png")]
    public class EditorOnlyLogic : LogicBase
    {
        public enum Mode
        {
            PlayerAndEditor,
            EditorOnly,
            PlayerOnly
        }

        [Tooltip("Disables when using Play From Here")]
        public bool DisableOnPlayFromHere = false;

        public Mode ExecutionPath = Mode.PlayerAndEditor;

        public Callable[] OnExecute;

        public override void Execute(GameObject instigator = null)
        {
            bool acceptPlayFromHere = !(DisableOnPlayFromHere && (PlayerPrefs.GetInt("PlayFromHere") == 1));

            switch(ExecutionPath)
            {
                case Mode.PlayerAndEditor:
                    if (acceptPlayFromHere)
                        Callable.Call(OnExecute, instigator);
                    break;
                case Mode.EditorOnly:
                    if (Application.isEditor && acceptPlayFromHere)
                        Callable.Call(OnExecute, instigator);
                    break;

                case Mode.PlayerOnly:
                    if (!Application.isEditor && acceptPlayFromHere)
                        Callable.Call(OnExecute, instigator);
                    break;
            }
        }

        public override string GetDefaultName()
        {
            return $"If {ExecutionPath} {(DisableOnPlayFromHere? "(Not Play From Here)":"")}" ;
        }
    }
}
