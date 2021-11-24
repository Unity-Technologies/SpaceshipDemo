using System.Collections.Generic;
using ConsoleUtility;
using UnityEngine;

[AutoRegisterConsoleCommand]
public class UpsamplingDebugCommand : IConsoleCommand
{
    public string name => "upsampling-debug";

    public string summary => "Toggles Upsampling debug Panel";

    public string help => @"upsampling-debug
Toggles Upsampling debug Panel";

    public IEnumerable<Console.Alias> aliases => null;

    static GameObject instance;

    public void Execute(string[] args)
    {
        if(instance == null)
        {
            var prefab = Resources.Load<GameObject>("UpsamplingDebug");
            if(prefab != null)
            {
                instance = GameObject.Instantiate(prefab);
                instance.name = "UpsamplingDebug";
                GameObject.DontDestroyOnLoad(instance);
            }
        }
        else
        {
            instance.SetActive(!instance.activeSelf);
        }
    }
}
