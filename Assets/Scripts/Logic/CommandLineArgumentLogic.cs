using GameplayIngredients;
using GameplayIngredients.Logic;
using NaughtyAttributes;
using System;
using UnityEngine;

public class CommandLineArgumentLogic : LogicBase
{
    public string Option = "--option";

    [ReorderableList]
    public Callable[] OnArgumentPresent;

    [ReorderableList]
    public Callable[] OnArgumentAbsent;

    public override void Execute(GameObject instigator = null)
    {
        bool found = false;

        string[] args = Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].ToLower() == Option.ToLower())
            {
                found = true;
                break;
            }
        }

        if (found)
            Call(OnArgumentPresent, instigator);
        else
            Call(OnArgumentAbsent, instigator);
    }
}
