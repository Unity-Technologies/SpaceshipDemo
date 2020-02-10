using ConsoleUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[AutoRegisterConsoleCommand]
public class SceneCommand : IConsoleCommand
{
    public string name => "scene";

    public string summary => "scene loading tools";

    public string help => @"scene [command] {scene name}
commands : load / add / unload";

    public IEnumerable<Console.Alias> aliases => null;

    public void Execute(string[] args)
    {
        if(args.Length == 1 && args[0].ToLower() == "list" )
        {
            Console.Log("SCENE", "Enumerating Loaded Scenes....");
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                bool active = SceneManager.GetActiveScene() == scene;
                Console.Log("SCENE", $"#{scene.buildIndex} - {scene.name} : {(scene.isLoaded ? "Loaded" : "Unloaded")} {(active ? "ACTIVE" : "")}");
            }
        }
        else if (args.Length == 2 && args[0].ToLower() == "load")
        {
            SceneManager.LoadSceneAsync(args[1], LoadSceneMode.Single);
            Console.Log("SCENE", $"Loading Single Scene {args[1]}");
        }
        else if (args.Length == 2 && args[0].ToLower() == "add")
        {
            SceneManager.LoadSceneAsync(args[1], LoadSceneMode.Additive);
            Console.Log("SCENE", $"Loading Additive Scene {args[1]}");

        }
        else if (args.Length == 2 && args[0].ToLower() == "unload")
        {
            SceneManager.UnloadSceneAsync(args[1]);
            Console.Log("SCENE", $"Unloading Scene {args[1]}");
        }
    }
}
