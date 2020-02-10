using ConsoleUtility;
using GameplayIngredients;
using System.Collections.Generic;
using UnityEngine;

[AutoRegisterConsoleCommand]
public class LoadLevelConsoleCommand : IConsoleCommand
{
    public string name => "load";

    public string summary => "Loads a level";

    public string help => "load <level-Id>";

    public IEnumerable<Console.Alias> aliases
    {
        get
        {
            yield return new Console.Alias() { AliasString = "mainmenu", Command = "load -1"  } ;
        }
    }

    public void Execute(string[] args)
    {
        if(args.Length == 1)
        {
            int idx;
            if(int.TryParse(args[0], out idx))
            {
                var manager = GameplayIngredients.Manager.Get<GameManager>();
                idx = Mathf.Clamp(idx, -1, manager.MainGameLevels.Length-1);
                Console.Log(name, $"Loading Game level #{idx} ...");
                manager.SwitchLevel(idx, true, null);
            }
        }
    }
}
