using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConsoleUtility
{
    [AutoRegisterConsoleCommand]
    public class TimeCommand : IConsoleCommand
    {
        public void Execute(string[] args)
        {
            int count = args.Length;

            if(count == 0)
            {
                Console.Log(help);
            }
            else
            {
                switch (args[0].ToLower())
                {
                    case "delta":
                        if (count == 1)
                            Console.Log(name, "Curent DeltaTime: " + Time.deltaTime);
                        break;
                    case "total":
                        if (count == 1)
                            Console.Log(name, "Curent TotalTime: " + Time.deltaTime);
                        break;
                    case "scale":
                        if (count == 1)
                            Console.Log(name, "Curent Time Scale: " + Time.timeScale);
                        if (count == 2)
                        {
                            double scale;
                            if (double.TryParse(args[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out scale))
                            {
                                Time.timeScale = (float)scale;
                                Console.Log(name, "Set Time Scale to : " + scale);
                            }
                            else
                            {
                                Console.Log(name, "Could not set timeScale to " + args[1]);
                            }
                        }
                        break;
                    case "pause":
                        if (count >= 1)
                        {
                            Time.timeScale = 0;
                            Console.Log(name, "Paused the Game");
                        }
                        break;
                    case "unpause":
                        if (count >= 1)
                        {
                            Time.timeScale = 1;
                            Console.Log(name, "Un-paused the Game");
                        }
                        break;
                }
            }
        }

        public string name => "time";

        public string summary => "Sets or gets values for global time";

        public string help =>  @"Command : Time <property> [value]
sets or gets values for global time.
read properties:
* delta : frame current time
* total : total elapsed time
* scale : current timescale
write properties:
* scale : set Current Time Scale
* pause : pauses the game
* unpause : unpause the game and set Timescale to 1x";

        public IEnumerable<Console.Alias> aliases
        {
            get 
            {
                yield return Console.Alias.Get("pause", "time pause");
                yield return Console.Alias.Get("unpause", "time unpause");
                yield return Console.Alias.Get("scale", "time scale");
            }
        }    
    }
}


