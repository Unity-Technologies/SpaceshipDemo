using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Actions
{
    public class SetGlobalAction : ActionBase
    {
        public Globals.Scope scope = Globals.Scope.Global;
        public Globals.Type type = Globals.Type.Boolean;
        public string Variable = "SomeVariable";

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

        bool isBool() { return type == Globals.Type.Boolean; }
        bool isInt() { return type == Globals.Type.Integer; }
        bool isFloat() { return type == Globals.Type.Float; }
        bool isString() { return type == Globals.Type.String; }
        bool isGameObject() { return type == Globals.Type.GameObject; }

        public override void Execute(GameObject instigator = null)
        {
            switch (type)
            {
                default:
                case Globals.Type.Boolean:
                    Globals.SetBool(Variable, boolValue, scope);
                    break;
                case Globals.Type.Integer:
                    Globals.SetInt(Variable, intValue, scope);
                    break;
                case Globals.Type.String:
                    Globals.SetString(Variable, stringValue, scope);
                    break;
                case Globals.Type.Float:
                    Globals.SetFloat(Variable, floatValue, scope);
                    break;
                case Globals.Type.GameObject:
                    Globals.SetObject(Variable, gameObjectValue, scope);
                    break;
            }
        }
    }
}
