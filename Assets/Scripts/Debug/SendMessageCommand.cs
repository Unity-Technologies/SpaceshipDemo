using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ConsoleUtility;
using GameplayIngredients;

[AutoRegisterConsoleCommand]
public class SendMessageCommand : IConsoleCommand
{
    public string name => "send";

    public string summary => "sends a message through the messager system";

    public string help => "usage: send <message>";

    public IEnumerable<Console.Alias> aliases => null;

    public void Execute(string[] args)
    {
        if(args.Length > 0)
        {
            string full = args.Aggregate((i, j) => i + " " + j);
            Messager.Send(full);
        }
        else
        {
            Console.Log(name, help, LogType.Warning);
        }
    }
}
