using UnityEngine;


namespace ConsoleUtility
{
    public abstract class ConsoleInput : MonoBehaviour
    {
        public abstract bool toggle { get; }
        public abstract bool cycleView { get; }
        public abstract bool previousCommand { get; }
        public abstract bool nextCommand { get; }
        public abstract bool scrollUp { get; }
        public abstract bool scrollDown { get; }
        public abstract bool validate { get; }

        public abstract bool ctrl { get; }
        public abstract bool shift { get; }
    }
}

