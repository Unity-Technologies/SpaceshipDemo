using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConsoleUtility
{
    [AutoRegisterConsoleCommand]
    public class ScreenCommand : IConsoleCommand
    {
        public void Execute(string[] args)
        {
            if (args.Length == 0)
                Console.Log(help);
            else
            {
                switch(args[0].ToLower())
                {
                    case "resolution":
                        Resolution r = Screen.currentResolution;
                        if (args.Length == 1)
                        {
                            Console.Log(name, string.Format("Current resolution is {0}x{1} at {2}Hz", Screen.width, Screen.height, r.refreshRate));
                        }
                        else if(args.Length == 3)
                        {
                            int width, height;
                            if(int.TryParse(args[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out width)
                                && int.TryParse(args[2], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out height))
                            {
                                r.width = width;
                                r.height = height;
                                Screen.SetResolution(r.width,r.height,Screen.fullScreen, r.refreshRate);
                                Console.Log(name, string.Format("Setting resolution to {0}x{1} at {2}Hz", r.width, r.height, r.refreshRate));
                            }
                        }
                        else if(args.Length == 4)
                        {
                            int width, height, rate;
                            if (int.TryParse(args[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out width)
                                && int.TryParse(args[2], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out height)
                                && int.TryParse(args[3], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out rate))
                            {
                                r.width = width;
                                r.height = height;
                                r.refreshRate = rate;
                                Screen.SetResolution(r.width, r.height, Screen.fullScreen, r.refreshRate);
                                Console.Log(name, string.Format("Setting resolution to {0}x{1} at {2}Hz", r.width, r.height, r.refreshRate));
                            }
                        }
                        break;
                    case "fullscreen":
                        bool fullscreen = Screen.fullScreen;
                        if (args.Length == 1)
                        {
                            Console.Log(Screen.fullScreen? "Running FullScreen" : "Running Windowed");
                        }
                        else if (args.Length == 2)
                        {
                            if (bool.TryParse(args[1], out fullscreen))
                            {
                                Screen.fullScreen = fullscreen;
                                Console.Log(name, "Setting screen to " + (fullscreen ? "fullscreen" : "windowed"));
                            }
                        }

                        break;
                    default:
                        Console.Log(name, "Unknown Command : " + args[0], LogType.Error);
                        break;
                }
            }
        }

        public string name => "screen";

        public string summary => "Sets or gets various informations regarding screen";

        public string help => @"usage: screen <i>command</i> [params]
read values
* resolution
* fullscreen
store values
* resolution <i>width</i> <i>height</i> [refreshrate]
* fullscreen [true/false]
";

        public IEnumerable<Console.Alias> aliases
        {
            get {
                yield return Console.Alias.Get("resolution", "screen resolution");
                yield return Console.Alias.Get("fullscreen", "screen fullscreen");
            }
        }

    }

}
