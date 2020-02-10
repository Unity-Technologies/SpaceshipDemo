using UnityEngine;
using System.Collections.Generic;
using GameplayIngredients;
using ConsoleUtility;

[AutoRegisterConsoleCommand]
public class POVConsoleCommand : IConsoleCommand
{
    public string name => "pov";

    public string summary => "Debug command to switch between stored POVs";

    public string help => @"pov [index]";

    public IEnumerable<Console.Alias> aliases => null;

    public void Execute(string[] args)
    {
        if(args.Length == 1)
        {
            int index = -1;
            if (int.TryParse(args[0], out index))
            {
                ScenePOVRoot[] allRoots = Object.FindObjectsOfType<ScenePOVRoot>();
                if (allRoots.Length > 0)
                {
                    Transform[] allPOVs = allRoots[0].AllPOV;
                    if (index >= 0 && index < allPOVs.Length)
                    {
                        Manager.Get<DebugPOVManager>().SetCamera(allRoots[0].AllPOV[index].transform);
                    }
                    else
                    {
                        Console.Log("POV", $"Could not set POV #{index} : found  {allPOVs.Length} ScenePOVRoot objects", LogType.Error);
                    }
                }
                else
                {
                    Console.Log("POV", $"Could not find any ScenePOVRoot objects", LogType.Error);
                }
            }
            else
            {
                Console.Log("POV", $"Invalid argument: {args[0]}", LogType.Error);
            }
        }
        else
        {
            Manager.Get<DebugPOVManager>().SetCamera(null);
            Console.Log("POV", $"Disabled POV", LogType.Log);
        }
    }
}
