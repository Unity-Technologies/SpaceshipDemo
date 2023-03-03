using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace GameplayIngredients
{
    public static class InputSystemUtility
    {
#if ENABLE_INPUT_SYSTEM
        public static ButtonControl GetButton(MouseButton b)
        {
            Mouse m = Mouse.current;
            switch (b)
            {
                case MouseButton.Left: return m.leftButton;
                case MouseButton.Right: return m.rightButton;
                case MouseButton.Middle: return m.middleButton;
                case MouseButton.Back: return m.backButton;
                case MouseButton.Forward: return m.forwardButton;
                default:
                    throw new System.NotImplementedException();
            }
        }
        public static ButtonControl GetButton(GamepadButton b)
        {
            Gamepad p = Gamepad.current;
            switch (b)
            {
                case GamepadButton.A: return p.aButton;
                case GamepadButton.B: return p.bButton;
                case GamepadButton.X: return p.xButton;
                case GamepadButton.Y: return p.yButton;
                case GamepadButton.LeftShoulder: return p.leftShoulder;
                case GamepadButton.LeftTrigger: return p.leftTrigger;
                case GamepadButton.RightShoulder: return p.rightShoulder;
                case GamepadButton.RightTrigger: return p.rightTrigger;
                case GamepadButton.LeftThumbStick: return p.leftStickButton;
                case GamepadButton.RightThumbStick: return p.rightStickButton;
                case GamepadButton.Start: return p.startButton;
                case GamepadButton.Select: return p.selectButton;
                default:
                    throw new System.NotImplementedException();
            }
        }
        public static ButtonControl GetButton(Key k)
        {
            Keyboard kb = Keyboard.current;
            switch (k)
            {
                case Key.Space: return kb.spaceKey;
                case Key.Enter: return kb.enterKey;
                case Key.Tab: return kb.tabKey;
                case Key.Backquote: return kb.backquoteKey;
                case Key.Quote: return kb.quoteKey;
                case Key.Semicolon: return kb.semicolonKey;
                case Key.Comma: return kb.commaKey;
                case Key.Period: return kb.periodKey;
                case Key.Slash: return kb.slashKey;
                case Key.Backslash: return kb.backslashKey;
                case Key.LeftBracket: return kb.leftBracketKey;
                case Key.RightBracket: return kb.rightBracketKey;
                case Key.Minus: return kb.minusKey;
                case Key.Equals: return kb.equalsKey;
                case Key.A: return kb.aKey;
                case Key.B: return kb.bKey;
                case Key.C: return kb.cKey;
                case Key.D: return kb.dKey;
                case Key.E: return kb.eKey;
                case Key.F: return kb.fKey;
                case Key.G: return kb.gKey;
                case Key.H: return kb.hKey;
                case Key.I: return kb.iKey;
                case Key.J: return kb.jKey;
                case Key.K: return kb.kKey;
                case Key.L: return kb.lKey;
                case Key.M: return kb.mKey;
                case Key.N: return kb.nKey;
                case Key.O: return kb.oKey;
                case Key.P: return kb.pKey;
                case Key.Q: return kb.qKey;
                case Key.R: return kb.rKey;
                case Key.S: return kb.sKey;
                case Key.T: return kb.tKey;
                case Key.U: return kb.uKey;
                case Key.V: return kb.vKey;
                case Key.W: return kb.wKey;
                case Key.X: return kb.xKey;
                case Key.Y: return kb.yKey;
                case Key.Z: return kb.zKey;
                case Key.Digit1: return kb.digit1Key;
                case Key.Digit2: return kb.digit2Key;
                case Key.Digit3: return kb.digit3Key;
                case Key.Digit4: return kb.digit4Key;
                case Key.Digit5: return kb.digit5Key;
                case Key.Digit6: return kb.digit6Key;
                case Key.Digit7: return kb.digit7Key;
                case Key.Digit8: return kb.digit8Key;
                case Key.Digit9: return kb.digit9Key;
                case Key.Digit0: return kb.digit0Key;
                case Key.LeftShift: return kb.leftShiftKey;
                case Key.RightShift: return kb.rightShiftKey;
                case Key.LeftAlt: return kb.leftAltKey;
                case Key.RightAlt: return kb.rightAltKey;
                case Key.LeftCtrl: return kb.leftCtrlKey;
                case Key.RightCtrl: return kb.rightCtrlKey;
                case Key.LeftMeta: return kb.leftMetaKey;
                case Key.RightMeta: return kb.rightMetaKey;
                case Key.ContextMenu: return kb.contextMenuKey;
                case Key.Escape: return kb.escapeKey;
                case Key.LeftArrow: return kb.leftArrowKey;
                case Key.RightArrow: return kb.rightArrowKey;
                case Key.UpArrow: return kb.upArrowKey;
                case Key.DownArrow: return kb.downArrowKey;
                case Key.Backspace: return kb.backspaceKey;
                case Key.PageDown: return kb.pageDownKey;
                case Key.PageUp: return kb.pageUpKey;
                case Key.Home: return kb.homeKey;
                case Key.End: return kb.endKey;
                case Key.Insert: return kb.insertKey;
                case Key.Delete: return kb.deleteKey;
                case Key.CapsLock: return kb.capsLockKey;
                case Key.NumLock: return kb.numLockKey;
                case Key.PrintScreen: return kb.printScreenKey;
                case Key.ScrollLock: return kb.scrollLockKey;
                case Key.Pause: return kb.pauseKey;
                case Key.NumpadEnter: return kb.numpadEnterKey;
                case Key.NumpadDivide: return kb.numpadDivideKey;
                case Key.NumpadMultiply: return kb.numpadMultiplyKey;
                case Key.NumpadPlus: return kb.numpadPlusKey;
                case Key.NumpadMinus: return kb.numpadMinusKey;
                case Key.NumpadPeriod: return kb.numpadPeriodKey;
                case Key.NumpadEquals: return kb.equalsKey;
                case Key.Numpad0: return kb.numpad0Key;
                case Key.Numpad1: return kb.numpad1Key;
                case Key.Numpad2: return kb.numpad2Key;
                case Key.Numpad3: return kb.numpad3Key;
                case Key.Numpad4: return kb.numpad4Key;
                case Key.Numpad5: return kb.numpad5Key;
                case Key.Numpad6: return kb.numpad6Key;
                case Key.Numpad7: return kb.numpad7Key;
                case Key.Numpad8: return kb.numpad8Key;
                case Key.Numpad9: return kb.numpad9Key;
                case Key.F1: return kb.f1Key;
                case Key.F2: return kb.f2Key;
                case Key.F3: return kb.f3Key;
                case Key.F4: return kb.f4Key;
                case Key.F5: return kb.f5Key;
                case Key.F6: return kb.f6Key;
                case Key.F7: return kb.f7Key;
                case Key.F8: return kb.f8Key;
                case Key.F9: return kb.f9Key;
                case Key.F10: return kb.f10Key;
                case Key.F11: return kb.f11Key;
                case Key.F12: return kb.f12Key;
                case Key.OEM1: return kb.oem1Key;
                case Key.OEM2: return kb.oem2Key;
                case Key.OEM3: return kb.oem3Key;
                case Key.OEM4: return kb.oem4Key;
                case Key.OEM5: return kb.oem5Key;
                case Key.IMESelected: return kb.imeSelected;
                default:
                    throw new System.NotImplementedException();
            }
        }

#endif
    }

    public enum Device
    {
        Gamepad,
        Keyboard,
        Mouse
    }

    public enum MouseButton
    {
        Left,
        Right,
        Middle,
        Back,
        Forward
    }

    public enum GamepadButton
    {
        A, B, X, Y,
        LeftShoulder, LeftTrigger, RightShoulder, RightTrigger,
        LeftThumbStick, RightThumbStick,
        Start, Select,
    }

}


