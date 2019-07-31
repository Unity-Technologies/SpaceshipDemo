using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Reflection;
using System.Linq;

namespace Console
{
    public class Console : MonoBehaviour
    {
        static readonly int MAX_CHARS = 8192;

        [Header("Keys")]
        public KeyCode ToggleKey = KeyCode.F12;
        public KeyCode PreviousCommandKey = KeyCode.UpArrow;
        public KeyCode NextCommandKey = KeyCode.DownArrow;
        public KeyCode ScrollUpKey = KeyCode.PageUp;
        public KeyCode ScrollDownKey = KeyCode.PageDown;
        public KeyCode ValidateKey = KeyCode.Return;

        [Header("Items")]
        public Canvas Canvas;
        public InputField InputField;
        public Text LogText;
        public RectTransform LogContents;
        public ScrollRect ScrollRect;
        public GameObject AutoPanelRoot;
        public Text AutoPanelText;

        [Header("Settings")]
        [Range(1.0f, 30.0f)]
        public float ScrollSpeed = 5.0f;

        private static ConsoleData s_ConsoleData;
        private static Console s_Console;

        private bool bVisible = false;
        private int history = -1;

        void OnEnable()
        {
            if(s_ConsoleData == null)
            {
                s_ConsoleData = new ConsoleData();
                s_ConsoleData.AutoRegisterConsoleCommands();
            }
            s_ConsoleData.OnLogUpdated = UpdateLog;
            s_Console = this;

            Application.logMessageReceived += HandleUnityLog;

            Log("Console initialized successfully");

            UpdateLog();
        }

        void OnDisable()
        {
            s_ConsoleData.OnLogUpdated = null;
            s_Console = null;
            Application.logMessageReceived -= HandleUnityLog;
        }

        public void UpdateAutoPanel()
        {
            AutoPanelRoot.SetActive(InputField.text != "");
            string input = InputField.text;
            string[] words = input.Split(' ');
            if(s_ConsoleData.aliases.ContainsKey(words[0].ToUpper()))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("<color=lime><b>Alias</b></color> <color=orange>{0}</color> => <color=yellow>{1}</color>:\n", words[0], s_ConsoleData.aliases[words[0].ToUpper()]);
                var command = s_ConsoleData.commands[s_ConsoleData.aliases[words[0].ToUpper()].Split(' ').FirstOrDefault().ToUpper()];
                sb.Append(command.help);

                AutoPanelText.text = sb.ToString();
            }
            else if (s_ConsoleData.commands.ContainsKey(words[0].ToUpper()))
            {
                StringBuilder sb = new StringBuilder();
                var command = s_ConsoleData.commands[words[0].ToUpper()];
                sb.Append(command.help);
                AutoPanelText.text = sb.ToString();
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (var word in s_ConsoleData.aliases.Keys.Concat(s_ConsoleData.commands.Keys).Where(o => o.Contains(words[0].ToUpper())).ToArray())
                {
                    bool isAlias = s_ConsoleData.aliases.ContainsKey(word);
                    string append = isAlias ? string.Format("(alias for '<color=yellow>{0}</color>')",s_ConsoleData.aliases[word.ToUpper()]) : s_ConsoleData.commands[word].summary;

                    sb.AppendFormat("<color={0}>", isAlias ? "orange" : "yellow");
                    sb.Append(word.ToLower());
                    sb.Append("</color> <i>");
                    sb.Append(append);
                    sb.Append("</i>\n");
                }
                AutoPanelText.text = sb.ToString();
            }

        }

        void Update()
        {
            if (Input.GetKeyDown(ToggleKey))
                ToggleVisibility();

            if (!bVisible) return;

            if (Input.GetKeyDown(PreviousCommandKey))
            {
                history++;
                if (history > s_ConsoleData.commandHistory.Count - 1)
                    history = s_ConsoleData.commandHistory.Count - 1;

                if (history >= 0)
                    InputField.text = s_ConsoleData.commandHistory[history];

                InputField.MoveTextEnd(false);
            }
            else if (Input.GetKeyDown(NextCommandKey))
            {
                history--;
                if (history <= -1)
                {
                    InputField.text = "";
                    history = -1;
                }
                else
                    InputField.text = s_ConsoleData.commandHistory[history];

                InputField.MoveTextEnd(false);
            }
            else if (Input.GetKeyDown(ValidateKey))
            {
                ValidateCommand();
            }
            else if (Input.GetKeyDown(ScrollUpKey))
            {
                if (ScrollRect != null)
                    ScrollRect.verticalNormalizedPosition += ScrollSpeed / s_ConsoleData.lines.Count;
            }
            else if (Input.GetKeyDown(ScrollDownKey))
            {
                if (ScrollRect != null)
                    ScrollRect.verticalNormalizedPosition -= ScrollSpeed/ s_ConsoleData.lines.Count;
            }
        }

        public static void SetVisible(bool visible)
        {
            s_Console.SetVisibility(visible);
        }

        public static void CaptureScreenshot(string filename, int size)
        {
            s_Console.StartCoroutine(s_Console.Screenshot(filename, size));
        }

        public IEnumerator Screenshot(string filename, int size)
        {
            Console.SetVisible(false);
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(filename, 1);
            Console.SetVisible(true);
        }
        
        public void ToggleVisibility()
        {
            SetVisibility(!bVisible);
        }

        public void SetVisibility(bool visible)
        {
            if (bVisible == visible)
                return;
            bVisible = visible;
            Canvas.gameObject.SetActive(bVisible);
            if (bVisible)
            {
                InputField.text = "";
                InputField.Select();
                InputField.MoveTextStart(false);
                InputField.ActivateInputField();
                UpdateLog();
            }
        }

        public void ValidateCommand()
        {
            if (InputField.text == "") return;
            string command = InputField.text;
            string[] words = command.Split(' ');

            if (s_ConsoleData.commands.ContainsKey(words[0].ToUpper()))
            {
                s_ConsoleData.commands[words[0].ToUpper()].Execute(words.Skip(1).ToArray());
                InputField.text = "";
            }
            else if(s_ConsoleData.aliases.ContainsKey(words[0].ToUpper()))
            {
                string alias = words[0];
                string aliascommand = command.Replace(alias, s_ConsoleData.aliases[alias.ToUpper()]);
                string[] aliaswords = aliascommand.Split(' ');

                s_ConsoleData.commands[aliaswords[0].ToUpper()].Execute(aliaswords.Skip(1).ToArray());
                InputField.text = "";
            }
            else
            {
                Log("Unknown Command: " + words[0]);
                InputField.text = "";
            }
            s_ConsoleData.commandHistory.Insert(0, command);
            InputField.Select();
            InputField.MoveTextStart(false);
            InputField.ActivateInputField();
            history = -1;

            UpdateLog();
        }

        public static void Log(string Message)
        {
            Log(string.Empty, Message, LogType.Log);
        }

        public static void Log(string Command, string Message, LogType type = LogType.Log)
        {
            string prepend = "";
            if (Command != string.Empty)
            {
                string color = "white";
                switch(type)
                {
                    case LogType.Assert:
                    case LogType.Error:
                    case LogType.Exception:
                        color = "red";
                        break;
                    case LogType.Warning:
                        color = "orange";
                        break;
                    default:
                    case LogType.Log:
                        color = "lime";
                        break;
                }
                prepend = string.Format("[<color={1}>{0}</color>] ", Command.ToUpper(), color);
            }

            string[] lines = Message.Split('\n');

            foreach(string line in lines)
                s_ConsoleData.lines.Add("<color=gray>[" + Time.unscaledTime.ToString("F3") + "]</color> " + prepend +line);

            if (s_ConsoleData.OnLogUpdated != null)
                s_ConsoleData.OnLogUpdated.Invoke();
        }

        private static void HandleUnityLog(string logString, string stackTrace, LogType type)
        {
            Log("UNITY", string.Format("[{0}] : {1}", type, logString), type);
            if(type == LogType.Error || type == LogType.Exception)
            {
                Log(stackTrace);
            }
        }

        private int GetCapacity()
        {
            return (int)(LogText.rectTransform.rect.height / (LogText.font.lineHeight + LogText.lineSpacing));
        }

        private void UpdateLog()
        {
            if (s_ConsoleData.lines.Count == 0 || !bVisible) return;

            if (LogText != null)
            {
                //string text = s_ConsoleData.lines.Aggregate((a, b) => a + "\n" + b);
                
                string text = string.Empty;
                for(int i = s_ConsoleData.lines.Count-1; i >= 0; i--)
                {
                    string line = s_ConsoleData.lines[i];
                    if(text != string.Empty)
                        line += "\n"; 
                    
                    if(line.Length + text.Length > MAX_CHARS)
                        break;
                    else
                        text = line + text;
                }

                LogText.text = text; 
                LogText.GraphicUpdateComplete();

                if (LogContents != null)
                {
                    int count = LogText.text.Count(o => o == '\n');
                    float height = Math.Max(count * (LogText.fontSize + 2) * LogText.lineSpacing + 32, 64);
                    LogContents.sizeDelta = new Vector2(LogContents.sizeDelta.x, height);
                }

                if (ScrollRect != null)
                    ScrollRect.verticalNormalizedPosition = 0.0f;
            }
        }
        
        public static void Clear()
        {
            s_ConsoleData.lines.Clear();

            Log("Console","Cleared Output", LogType.Log);

            if (s_ConsoleData.OnLogUpdated != null)
                s_ConsoleData.OnLogUpdated.Invoke();
        }

        public static IConsoleCommand[] listAllCommands()
        {
            return s_ConsoleData.commands.Values.ToArray();
        }

        public static bool HasCommand(string name)
        {
            return s_ConsoleData.commands.ContainsKey(name);
        }

        public static void AddCommand(IConsoleCommand command)
        {
            // Add Command
            s_ConsoleData.commands.Add(command.name.ToUpper(), command);

            // Add Aliases
            var aliases = command.aliases;
            if(aliases != null)
                foreach (var alias in aliases)
                {
                    if (!alias.AliasString.Contains(" "))
                        s_ConsoleData.aliases.Add(alias.AliasString.ToUpper(), alias.Command);
                    else
                        Debug.LogError(string.Format("Cannot add alias for command {0} : alias '{1}' contains spaces", command.name, alias.AliasString));
                }

        }

        public struct Alias
        {
            public string AliasString;
            public string Command;

            public static Alias Get(string alias, string command)
            {
                return new Alias { AliasString = alias, Command = command };
            }
        }

        public class ConsoleData
        {
            public List<string> lines;
            public Dictionary<string, IConsoleCommand> commands;
            public Dictionary<string,string> aliases;
            public System.Action OnLogUpdated;
            public List<string> commandHistory;

            public ConsoleData()
            {
                lines = new List<string>();
                commands = new Dictionary<string, IConsoleCommand>();
                aliases = new Dictionary<string,string>();
                commandHistory = new List<string>();
            }

            public void AutoRegisterConsoleCommands()
            {
                // Use reflection to add automatically console commands
                var autoCommandTypes = new List<Type>();

                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.GetInterfaces().Contains(typeof(IConsoleCommand)) && type.GetCustomAttributes(typeof(AutoRegisterConsoleCommandAttribute), true).Length > 0)
                        {
                            autoCommandTypes.Add(type);
                        }
                    }
                }
                foreach (var type in autoCommandTypes)
                {
                    AddCommand(Activator.CreateInstance(type) as IConsoleCommand);
                }

            }
        }

        [AutoRegisterConsoleCommand]
        public class HelpCommand : IConsoleCommand
        {
            public void Execute(string[] args)
            {
                if (args.Length > 0)
                {
                    var commands = Console.listAllCommands();
                    var command = commands.FirstOrDefault<IConsoleCommand>((o) => o.name == args[0]);
                    if (command != null)
                    {
                        Console.Log(name, "Help for command : " + command.name);
                        Console.Log("Summary : " + command.summary);
                        string[] helpText = command.help.Replace("\r", "").Split('\n');
                        foreach (var line in helpText)
                        {
                            Console.Log("    " + line);
                        }
                    }
                    else
                        Console.Log(name, "Could not find help for command " + args[0]);
                }
                else
                {
                    Console.Log(name, "Available Commands:");
                    foreach (var command in Console.listAllCommands())
                    {
                        Console.Log(command.name + " : " + command.summary);
                    }
                }
            }
            
            public string name => "help";
            
            public string summary =>"Gets a summary of all commands or shows help for a specific command.";

            public string help => @"Usage: <color=yellow>HELP</color> <i>command</i>
Shows help for specific command or list any available command.
Additional arguments are ignored";
            
            public IEnumerable<Alias> aliases => null;
        }
    }

    [AutoRegisterConsoleCommand]
    public class Clear : IConsoleCommand
    {
        public void Execute(string[] args)
        {
            Console.Clear();
        }

        public string name => "clear";

        public string summary => "Clears the console output";

        public string help => @"<color=yellow>Clear</color>
                Usage: clear";

        public IEnumerable<Console.Alias> aliases
        {
            get 
            {
                yield return Console.Alias.Get("cls", "clear");
            }
        }

    }

    public interface IConsoleCommand
    {
        void Execute(string[] args);

        string name { get; }
        string summary { get; }
        string help { get; }
        IEnumerable<Console.Alias> aliases { get; }
    }

    public class AutoRegisterConsoleCommandAttribute : System.Attribute { }
}

