using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Logic
{
    [HelpURL(Help.URL + "counters")]
    [AddComponentMenu(ComponentMenu.counterPath + "Counter Logic")]
    [Callable("Data", "Misc/ic-counter.png")]
    public class CounterLogic : LogicBase
    {
        public enum ValueSourceType
        {
            Property,
            GlobalVariable,
            GameSave,
        }
        public enum Evaluation
        {
            Equal,
            NotEqual,
            Greater,
            GreaterOrEqual,
            Less,
            LessOrEqual
        }

        [NonNullCheck]
        public Counter Counter;
        public Evaluation evaluation = Evaluation.Equal;

        public ValueSourceType ValueSource = ValueSourceType.Property;
        [ShowIf("isValueProperty")]
        public int Value = 1;

        [ShowIf("isValueGameSave")]
        public string GameSaveVariableName = "Variable";
        [ShowIf("isValueGameSave")]
        public GameSaveManager.Location GameSaveLocation = GameSaveManager.Location.System;

        [ShowIf("isValueGlobal")]
        public string GlobalVariableName = "Variable";
        [ShowIf("isValueGlobal")]
        public Globals.Scope GlobalScope = Globals.Scope.Global;

        public Callable[] OnTestSuccess;
        public Callable[] OnTestFail;

        bool isValueProperty() { return ValueSource == ValueSourceType.Property; }
        bool isValueGameSave() { return ValueSource == ValueSourceType.GameSave; }
        bool isValueGlobal() { return ValueSource == ValueSourceType.GlobalVariable; }

        public override void Execute(GameObject instigator = null)
        {
            int value;
            switch (ValueSource)
            {
                default:
                case ValueSourceType.Property:
                    value = Value;
                    break;
                case ValueSourceType.GlobalVariable:
                    if (Globals.HasInt(GlobalVariableName, GlobalScope))
                        value = Globals.GetInt(GlobalVariableName, GlobalScope);
                    else
                    {
                        Debug.LogWarning($"CounterLogic ({name}) : Could not find Global integer {GlobalVariableName}({GlobalScope})");
                        value = 0;
                    }
                    break;
                case ValueSourceType.GameSave:
                    var gsm = Manager.Get<GameSaveManager>();

                    if (gsm.HasInt(GameSaveVariableName, GameSaveLocation))
                        value = gsm.GetInt(GameSaveVariableName, GameSaveLocation);
                    else
                    {
                        Debug.LogWarning($"CounterLogic ({name}) : Could not find Game Save integer {GameSaveVariableName}({GameSaveLocation})");
                        value = 0;
                    }
                    break;
            }

            bool test = false;

            switch (evaluation)
            {
                case Evaluation.Equal:
                    test = Counter.CurrentValue == value;
                    break;
                case Evaluation.NotEqual:
                    test = Counter.CurrentValue != value;
                    break;
                case Evaluation.Greater:
                    test = Counter.CurrentValue > value;
                    break;
                case Evaluation.GreaterOrEqual:
                    test = Counter.CurrentValue >= value;
                    break;
                case Evaluation.Less:
                    test = Counter.CurrentValue < value;
                    break;
                case Evaluation.LessOrEqual:
                    test = Counter.CurrentValue <=  value;
                    break;

                default:
                    break;
            }

            if (test)
                Callable.Call(OnTestSuccess, instigator);
            else
                Callable.Call(OnTestFail, instigator);

        }
    }

}

