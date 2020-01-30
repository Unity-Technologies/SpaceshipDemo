using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Actions
{
    public class TimerAction : ActionBase
    {
        public enum Action
        {
            Start,
            SetTime,
            Interrupt
        }

        [NonNullCheck]
        public Timer timer;
        public Action action = Action.Start;

        [ShowIf("isSetTime")]
        public uint Hours = 0;
        [ShowIf("isSetTime"), Range(0,59)]
        public uint Minutes = 1;
        [ShowIf("isSetTime"), Range(0,59)]
        public uint Seconds = 30;
        [ShowIf("isSetTime"), Range(0, 999)]
        public uint Milliseconds = 0;
        [ShowIf("isSetTime")]
        public bool Restart = false;


        public override void Execute(GameObject instigator = null)
        {
            if(timer == null)
            {
                Debug.LogWarning($"{this.gameObject.name}:{this.name} : Null Timer");
            }
            else
            {
                switch (action)
                {
                    default:
                    case Action.Start:
                        timer.Restart(instigator);
                        break;
                    case Action.SetTime:
                        if (timer.isRunning)
                            timer.Interrupt(instigator);

                        timer.Hours = Hours;
                        timer.Minutes = Minutes;
                        timer.Seconds = Seconds;
                        timer.Milliseconds = Milliseconds;

                        if (Restart)
                            timer.Restart(instigator);
                        break;
                    case Action.Interrupt:
                        timer.Interrupt(instigator);
                        break;
                }
            }
        }

        bool isSetTime() { return action == Action.SetTime; }
    }
}

