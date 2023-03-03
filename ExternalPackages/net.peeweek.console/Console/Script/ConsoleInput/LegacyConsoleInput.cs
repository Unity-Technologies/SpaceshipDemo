using UnityEngine;
#if ENABLE_LEGACY_INPUT_MANAGER

namespace ConsoleUtility
{
    public class LegacyConsoleInput : ConsoleInput
    {
        public KeyCode ToggleKey = KeyCode.Backslash;
        public KeyCode CycleViewKey = KeyCode.Tab;
        public KeyCode PreviousCommandKey = KeyCode.UpArrow;
        public KeyCode NextCommandKey = KeyCode.DownArrow;
        public KeyCode ScrollUpKey = KeyCode.PageUp;
        public KeyCode ScrollDownKey = KeyCode.PageDown;
        public KeyCode ValidateKey = KeyCode.Return;


        public override bool toggle => Input.GetKeyDown(ToggleKey);

        public override bool cycleView => Input.GetKeyDown(CycleViewKey);

        public override bool previousCommand => Input.GetKeyDown(PreviousCommandKey);

        public override bool nextCommand => Input.GetKeyDown(NextCommandKey);

        public override bool scrollUp => Input.GetKeyDown(ScrollUpKey);

        public override bool scrollDown => Input.GetKeyDown(ScrollDownKey);

        public override bool validate => Input.GetKeyDown(ValidateKey);
        
        public override bool ctrl => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        public override bool shift => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }
}
#else
namespace ConsoleUtility
{
    [AddComponentMenu("")] // Hidden
    public class LegacyConsoleInput : ConsoleInput
    {
        public override bool toggle => throw new System.NotImplementedException();

        public override bool cycleView => throw new System.NotImplementedException();

        public override bool previousCommand => throw new System.NotImplementedException();

        public override bool nextCommand => throw new System.NotImplementedException();

        public override bool scrollUp => throw new System.NotImplementedException();

        public override bool scrollDown => throw new System.NotImplementedException();

        public override bool validate => throw new System.NotImplementedException();

        public override bool ctrl => throw new System.NotImplementedException();

        public override bool shift => throw new System.NotImplementedException();
    }
}
#endif
