using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Actions
{
    public class CounterAction : ActionBase
    {
        public enum CounterOperation
        {
            Set,
            Add,
            Subtract,
            Multiply,
            Divide,
            Modulo
        }

        public enum ValueSourceType
        {
            Property,
            GlobalVariable,
            GameSave,
        }

        [ReorderableList]
        public Counter[] Counters;

        public CounterOperation Operation = CounterOperation.Set;
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
                        Debug.LogWarning($"CounterAction ({name}) : Could not find Global integer {GlobalVariableName}({GlobalScope})");
                        value = 0;
                    }
                    break;
                case ValueSourceType.GameSave:
                    var gsm = Manager.Get<GameSaveManager>();

                    if (gsm.HasInt(GameSaveVariableName, GameSaveLocation))
                        value = gsm.GetInt(GameSaveVariableName, GameSaveLocation);
                    else
                    {
                        Debug.LogWarning($"CounterAction ({name}) : Could not find Game Save integer {GameSaveVariableName}({GameSaveLocation})");
                        value = 0;
                    }

                    break;
            }

            foreach(var counter in Counters)
            {
                if (counter == null)
                    continue;

                switch (Operation)
                {
                    default:
                    case CounterOperation.Set:
                        break;
                    case CounterOperation.Add:
                        value = counter.CurrentValue + value;
                        break;
                    case CounterOperation.Subtract:
                        value = counter.CurrentValue - value;
                        break;
                    case CounterOperation.Multiply:
                        value = counter.CurrentValue * value;
                        break;
                    case CounterOperation.Divide:
                        if (value != 0)
                            value = counter.CurrentValue / value;
                        else
                        {
                            Debug.LogWarning($"{this.name} : Division by zero");
                            continue;
                        }
                        break;
                    case CounterOperation.Modulo:
                        if (value != 0)
                            value = counter.CurrentValue % value;
                        else
                        {
                            Debug.LogWarning($"{this.name} : Modulo by zero");
                            continue;
                        }
                        break;
                }
                counter.SetValue(value, instigator);
            }
        }
    }
}
