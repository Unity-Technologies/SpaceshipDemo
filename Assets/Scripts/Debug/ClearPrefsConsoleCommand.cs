using ConsoleUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AutoRegisterConsoleCommand]
public class ClearPrefsConsoleCommand : IConsoleCommand
{
    public string name => "clearprefs";

    public string summary => "Clears all PlayerPrefs()";

    public string help => "clearprefs";

    public IEnumerable<Console.Alias> aliases => null;

    public void Execute(string[] args)
    {
        if(args.Length == 0)
        {
            PlayerPrefs.DeleteAll();
            Console.Log("ClearPrefs", "Preferences Cleared.", LogType.Log);
        }
        else
        {
            Console.Log(name, help, LogType.Warning);
        }
    }
}
