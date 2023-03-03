using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConsoleUtility
{
    [AutoRegisterConsoleCommand]
    public class StatCommand : IConsoleCommand
    {
        public string name => "stat";

        public string summary => "Display Statistics about current game session";

        public string help => "stat";

        public IEnumerable<Console.Alias> aliases => null;

        public void Execute(string[] args)
        {
            Console.RegisterView<StatView>();
        }

        class StatView : View
        {
            public StatView() : base(15f) { }

            public override string GetDebugViewString()
            {
                return $@"
    Current Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}

    Time Since Level Load : {Time.timeSinceLevelLoad} seconds
    FPS : {1f / Time.deltaTime}
    Delta Time : {Time.smoothDeltaTime * 1000}ms 
    Time Scale : {Time.timeScale}x

    Camera : {Camera.main?.name}
    Position : {Camera.main?.transform.position}

";
            }
        }
    }
}
