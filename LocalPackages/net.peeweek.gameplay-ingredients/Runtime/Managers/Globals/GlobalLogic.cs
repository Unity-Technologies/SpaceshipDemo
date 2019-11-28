using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Logic
{
    public class GlobalLogic : LogicBase
    {
        [Header("Base Value")]
        public Globals.Scope scope = Globals.Scope.Global;
        public Globals.Type type = Globals.Type.Boolean;
        public string Variable = "SomeVariable";
        public Evaluation evaluation = Evaluation.Equal;

        [Header("Compare To...")]
        [ShowIf("isCompareToOther")]
        public CompareTo compareTo = CompareTo.Value;
        
        [ShowIf("isBool")]
        public bool boolValue = true;
        [ShowIf("isInt")]
        public int intValue = 1;
        [ShowIf("isString")]
        public string stringValue = "Value";
        [ShowIf("isFloat")]
        public float floatValue = 1.0f;
        [ShowIf("isGameObject")]
        public GameObject gameObjectValue;
        [ShowIf("isGlobal")]
        public string compareToVariable = "OtherVariable";
        [ShowIf("isGlobal")]
        public Globals.Scope compareToScope = Globals.Scope.Global;


        public enum Evaluation
        {
            Equal,
            NotEqual,
            Greater,
            GreaterOrEqual,
            Less,
            LessOrEqual,
            Exists
        }

        public enum CompareTo
        {
            Value,
            OtherGlobalVariable,
        }

        bool isBool() { return isValue() && type == Globals.Type.Boolean; }
        bool isInt() { return isValue() && type == Globals.Type.Integer; }
        bool isFloat() { return isValue() && type == Globals.Type.Float; }
        bool isString() { return isValue() && type == Globals.Type.String; }
        bool isGameObject() { return isValue() && type == Globals.Type.GameObject; }
        bool isValue() { return compareTo == CompareTo.Value && isCompareToOther(); }
        bool isGlobal() { return compareTo == CompareTo.OtherGlobalVariable && isCompareToOther(); }
        bool isCompareToOther() { return evaluation != Evaluation.Exists; }

        [ReorderableList]
        public Callable[] OnTestSuccess;
        [ReorderableList]
        public Callable[] OnTestFail;

        public override void Execute(GameObject instigator = null)
        {
            bool result = false;

            if (evaluation == Evaluation.Exists)
            {
                switch (type)
                {
                    case Globals.Type.Boolean: result = Globals.HasBool(Variable, scope); break;
                    case Globals.Type.Float: result = Globals.HasFloat(Variable, scope); break;
                    case Globals.Type.Integer: result = Globals.HasInt(Variable, scope); break;
                    case Globals.Type.String: result = Globals.HasString(Variable, scope); break;
                    case Globals.Type.GameObject: result = Globals.HasObject(Variable, scope); break;
                }
            }
            else
            {
                try
                {
                    switch (type)
                    {
                        case Globals.Type.Boolean:
                            result = TestValue(Globals.GetBool(Variable, scope), GetBoolValue());
                            break;
                        case Globals.Type.Integer:
                            result = TestValue(Globals.GetInt(Variable, scope), GetIntValue());
                            break;
                        case Globals.Type.Float:
                            result = TestValue(Globals.GetFloat(Variable, scope), GetFloatValue());
                            break;
                        case Globals.Type.String:
                            result = TestValue(Globals.GetString(Variable, scope), GetStringValue());
                            break;
                        case Globals.Type.GameObject:
                            result = TestObjectValue(Globals.GetObject(Variable, scope), GetObjectValue());
                            break;
                    }
                }
                catch { }
            }

            if (result)
                Callable.Call(OnTestSuccess, instigator);
            else
                Callable.Call(OnTestFail, instigator);

        }

        bool GetBoolValue()
        {
            switch (compareTo)
            {
                default:
                case CompareTo.Value:
                    return boolValue;
                case CompareTo.OtherGlobalVariable:
                    return Globals.GetBool(compareToVariable, compareToScope);
            }
        }

        int GetIntValue()
        {
            switch (compareTo)
            {
                default:
                case CompareTo.Value:
                    return intValue;
                case CompareTo.OtherGlobalVariable:
                    return Globals.GetInt(compareToVariable, compareToScope);
            }
        }

        float GetFloatValue()
        {
            switch (compareTo)
            {
                default:
                case CompareTo.Value:
                    return floatValue;
                case CompareTo.OtherGlobalVariable:
                    return Globals.GetFloat(compareToVariable, compareToScope);
            }
        }

        string GetStringValue()
        {
            switch (compareTo)
            {
                default:
                case CompareTo.Value:
                    return stringValue;
                case CompareTo.OtherGlobalVariable:
                    return Globals.GetString(compareToVariable, compareToScope);
            }
        }

        GameObject GetObjectValue()
        {
            switch (compareTo)
            {
                default:
                case CompareTo.Value:
                    return gameObjectValue;
                case CompareTo.OtherGlobalVariable:
                    return Globals.GetObject(compareToVariable, compareToScope);
            }
        }

        bool TestValue<T>(T value, T other) where T : System.IComparable<T>
        {
            switch (evaluation)
            {
                case Evaluation.Equal: return value.CompareTo(other) == 0;
                case Evaluation.NotEqual: return value.CompareTo(other) != 0;
                case Evaluation.Greater: return value.CompareTo(other) > 0;
                case Evaluation.GreaterOrEqual: return value.CompareTo(other) >= 0;
                case Evaluation.Less: return value.CompareTo(other) < 0;
                case Evaluation.LessOrEqual: return value.CompareTo(other) <= 0;
            }
            return false;
        }

        bool TestObjectValue(GameObject value, GameObject other)
        {
            switch (evaluation)
            {
                case Evaluation.Equal:
                    return value == other;
                case Evaluation.NotEqual:
                case Evaluation.Greater:
                case Evaluation.GreaterOrEqual:
                case Evaluation.Less:
                case Evaluation.LessOrEqual:
                    return value != other;
            }
            return false;
        }
    }
}
