using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConsoleUtility
{
    [AutoRegisterConsoleCommand]
    public class ExitCommand : IConsoleCommand
    {
        public void Execute(string[] args)
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
                Application.Quit();
                
        }

        public string name => "exit";

        public string summary =>"Exits the game";

        public string help => "usage: exit " + summary;
        
        public IEnumerable<Console.Alias> aliases
        {
            get 
            {
                yield return Console.Alias.Get("quit", "exit");
            }
        }
    }

}
