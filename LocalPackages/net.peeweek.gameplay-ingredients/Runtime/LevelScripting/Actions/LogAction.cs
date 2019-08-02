using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace GameplayIngredients.Actions
{
    public class LogAction : ActionBase
    {
        const string kHelpText = @"Wildcards:
%INSTIGATOR% : Instigator Name
%TIME% : Time (since startup)
";

        [Multiline, InfoBox(kHelpText, InfoBoxType.Normal)]
        public string LogText = "Instigator = %INSTIGATOR%";
        public LogType type = LogType.Log;

        public override void Execute(GameObject instigator = null)
        {
            Debug.unityLogger.Log(type, FormatString(instigator));            
        }

        string FormatString(GameObject instigator)
        {
            string text = LogText;
            if(text.Contains("%INSTIGATOR%"))
            {
                text = text.Replace("%INSTIGATOR%", instigator == null ? "NULL" : instigator.name);
            }
            if(text.Contains("%TIME%"))
            {
                text = text.Replace("%TIME%", Time.time.ToString());
            }

            return text;
        }
    }
}
