using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Reflection;
using System.Linq;

namespace ConsoleUtility
{
    public class Console : MonoBehaviour
    {
        [SerializeField]
        ConsoleInput consoleInput;

        [Header("Items")]
        public Canvas Canvas;
        public InputField InputField;
        public Button ExecuteButton;
        public Button ClearButton;
        public Text LogText;
        public Text ScrollInfo;
        public GameObject AutoPanelRoot;
        public Text AutoPanelText;

        [Header("Peek")]
        public GameObject peekRoot;
        public Text peekText;
        public bool showOnReleaseBuilds = false;
        public bool showOnEditor = false;
        public float peekDuration = 5.0f;

        [Header("Settings")]
        [Range(1.0f, 30.0f)]
        public float ScrollSpeed = 5.0f;

        private static ConsoleData s_ConsoleData;
        private static Console s_Console;

        private bool bConsoleVisible = false;
        private int history = -1;

        public static event ConsoleDelegate onConsoleToggle;
        public delegate void ConsoleDelegate(bool visible);

        void OnEnable()
        {
            s_ConsoleData = new ConsoleData();
            s_ConsoleData.AutoRegisterConsoleCommands();

            s_ConsoleData.OnLogUpdated = UpdateLog;
            s_Console = this;

            Application.logMessageReceived += HandleUnityLog;

            LogText.font.RequestCharactersInTexture("qwertyuiopasdfghjklzxcvbnmQWERYTUIOPASDFGHJKLZXCVBNM1234567890~`!@#$%^&*()_+{}[]:;\"'/.,?><");

            Log("Console initialized successfully");
            UpdateLog();

            ClearPeek();
        }

        void OnDisable()
        {
            if (s_Console == null && s_ConsoleData == null)
                return;

            ClearViews();
            s_ConsoleData.OnLogUpdated = null;
            s_Console = null;
            s_ConsoleData = null;
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
            if (consoleInput && consoleInput.toggle)
                ToggleVisibility();

            if (!bConsoleVisible) return;

            if (consoleInput && consoleInput.cycleView)
            {
                if(consoleInput.ctrl)
                {
                    if(m_CurrentView >= 0)
                        RemoveView(m_CurrentView);
                }
                else
                {
                    // If Shift, cycle reverse
                    if (consoleInput.shift)
                        SetView(m_CurrentView - 1);
                    else
                        SetView(m_CurrentView + 1);
                }
            }

            if (m_CurrentView != -1)
            {
                if (s_ConsoleData.views[m_CurrentView].Update())
                {
                    LogText.text = s_ConsoleData.views[m_CurrentView].GetDebugViewString();
                    LogText.Rebuild(CanvasUpdate.Layout);
                }
                return;
            }

            if (consoleInput && consoleInput.previousCommand)
            {
                history++;
                if (history > s_ConsoleData.commandHistory.Count - 1)
                    history = s_ConsoleData.commandHistory.Count - 1;

                if (history >= 0)
                    InputField.text = s_ConsoleData.commandHistory[history];

                InputField.MoveTextEnd(false);
            }
            else if (consoleInput && consoleInput.nextCommand)
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
            else if (consoleInput && consoleInput.validate)
            {
                ValidateCommand();
            }
            else if (consoleInput && consoleInput.scrollUp)
            {
                ScrollUp();
            }
            else if (consoleInput && consoleInput.scrollDown)
            {
                ScrollDown();
            }
        }

        #region SCREENSHOT

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
        #endregion

        #region VIEWS

        int m_CurrentView = -1;

        void SetView(int index)
        {
            if (index >= s_ConsoleData.views.Count)
                index = -1;
            else if (index < -1)
                index = s_ConsoleData.views.Count - 1;

            if (index == m_CurrentView)
                return;

            if (m_CurrentView >= 0)
                s_ConsoleData.views[m_CurrentView].OnDisable();

            m_CurrentView = index;

            if (m_CurrentView == -1)
            {
                InputField?.gameObject.SetActive(true);
                ExecuteButton?.gameObject.SetActive(true);
                ClearButton?.gameObject.SetActive(true);
                UpdateLog();
                InputField.ActivateInputField();
            }
            else
            {
                s_ConsoleData.views[m_CurrentView].OnEnable();
                InputField?.gameObject.SetActive(false);
                ExecuteButton?.gameObject.SetActive(false);
                ClearButton?.gameObject.SetActive(false);
            }
        }

        public static void RegisterView<TView>() where TView : View, new()
        {
            // If not yet created, create an instance
            if(!s_ConsoleData.views.Any(o => o.GetType() == typeof(TView)))
            {
                Log("Views", $"Created new View of type {typeof(TView).Name}");
                var view = new TView();
                view.OnCreate();
                s_ConsoleData.views.Add(view);
            }

            // Switch to view
            int index = s_ConsoleData.views.IndexOf(s_ConsoleData.views.First(o => o.GetType() == typeof(TView)));
            s_Console.SetView(index);
        }

        public static void RemoveView(int index)
        {
            if(index >= 0 && index < s_ConsoleData.views.Count)
            {
                var view = s_ConsoleData.views[index];

                if (s_Console.m_CurrentView >= index)
                    s_Console.SetView(s_Console.m_CurrentView - 1);
                view.OnDestroy();
                s_ConsoleData.views.Remove(view);
                Log("Views", $"Removed View of type {view.GetType().Name}");
            }
        }

        public static void RemoveView<TView>() where TView:View
        {
            if (s_ConsoleData.views.Any(o => o.GetType() == typeof(TView)))
            {
                var view = s_ConsoleData.views.First(o => o.GetType() == typeof(TView));
                int index = s_ConsoleData.views.IndexOf(view);

                if (s_Console.m_CurrentView >= index)
                    s_Console.SetView(s_Console.m_CurrentView - 1);

                view.OnDestroy();
                s_ConsoleData.views.Remove(view);
                Log("Views", $"Removed View of type {view.GetType().Name}");
            }
        }

        public static void ClearViews()
        {
            s_Console.SetView(-1);

            foreach (var view in s_ConsoleData.views)
                view.OnDestroy();

            s_ConsoleData.views.Clear();
            Log("Views", $"Cleared All Views");
        }

        #endregion

        #region VISIBILITY

        public static void SetVisible(bool visible)
        {
            s_Console.SetVisibility(visible);
            s_Console.SetPeekVisible(!visible);
        }

        public void ToggleVisibility()
        {
            SetVisibility(!bConsoleVisible);
        }

        public void SetVisibility(bool visible)
        {
            if (bConsoleVisible == visible)
                return;
            bConsoleVisible = visible;
            Canvas.gameObject.SetActive(bConsoleVisible);
            if (bConsoleVisible)
            {
                InputField.text = "";
                InputField.Select();
                InputField.MoveTextStart(false);
                InputField.ActivateInputField();
                UpdateLog();
            }

            onConsoleToggle?.Invoke(visible);
            SetPeekVisible(!visible);
        }

        #endregion

        #region CONSOLE BEHAVIOR

        public static void ExecuteCommand(string command)
        {
            string[] words = command.Split(' ');

            if (s_ConsoleData.commands.ContainsKey(words[0].ToUpper()))
            {
                s_ConsoleData.commands[words[0].ToUpper()].Execute(words.Skip(1).ToArray());
            }
            else if (s_ConsoleData.aliases.ContainsKey(words[0].ToUpper()))
            {
                string alias = words[0];
                string aliascommand = command.Replace(alias, s_ConsoleData.aliases[alias.ToUpper()]);
                string[] aliaswords = aliascommand.Split(' ');

                s_ConsoleData.commands[aliaswords[0].ToUpper()].Execute(aliaswords.Skip(1).ToArray());
            }
            else
            {
                Log("Unknown Command: " + words[0]);
            }

            // Ensure no duplicates in history
            if (s_ConsoleData.commandHistory.Count == 0 || command != s_ConsoleData.commandHistory[0])
                s_ConsoleData.commandHistory.Insert(0, command);

        }

        public void ValidateCommand()
        {
            if (InputField.text == "") 
                return;

            string command = InputField.text;
            ExecuteCommand(command);
            InputField.text = "";
            InputField.Select();
            InputField.MoveTextStart(false);
            InputField.ActivateInputField();
            history = -1;

            UpdateLog();
        }

        public static void Log(string Message, bool logToPeek = true)
        {
            Log(string.Empty, Message, LogType.Log);
        }

        public static void Log(string Command, string Message, LogType type = LogType.Log, bool logToPeek = true)
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

            if (logToPeek && lines.Length >= 1)
                s_Console?.LogToPeek(prepend + lines[0]);

        }

        private static void HandleUnityLog(string logString, string stackTrace, LogType type)
        {
            Log("UNITY", string.Format("[{0}] : {1}", type, logString), type);
            if(type == LogType.Error || type == LogType.Exception)
            {
                Log(stackTrace, false);
            }
        }

        private int m_Scroll=-1;

        private int GetCapacity()
        {
            return (int)(LogText.rectTransform.rect.height / (LogText.font.lineHeight + LogText.lineSpacing));
        }

        private int GetScrollPageSize()
        {
            float lineSize = LogText.lineSpacing + LogText.font.fontSize;
            int count = Mathf.FloorToInt(LogText.rectTransform.rect.height / lineSize);
            return count;
        }

        private void ScrollUp()
        {
            int pageSize = GetScrollPageSize();

            if (s_ConsoleData.lines.Count < pageSize)
                return;

            if(m_Scroll == -1)
            {
                m_Scroll = s_ConsoleData.lines.Count - pageSize;
            }
            
            m_Scroll = Math.Max(0, m_Scroll - pageSize);
            UpdateLog();
        }

        private void ScrollDown()
        {
            int pageSize = GetScrollPageSize();

            if (s_ConsoleData.lines.Count < pageSize)
                return;

            if (m_Scroll == -1)
                return;

            m_Scroll += pageSize;

            if (m_Scroll >= (s_ConsoleData.lines.Count - pageSize))
                m_Scroll = -1; // Snap Again

            UpdateLog();
        }

        private string GetScrolledText()
        {
            int pageSize = GetScrollPageSize();
            int count = s_ConsoleData.lines.Count;

            CharacterInfo info;
            LogText.font.GetCharacterInfo('X', out info);

            int maxCharsInLine = Mathf.FloorToInt(LogText.rectTransform.rect.width / info.advance);

            int init = m_Scroll == -1 ? count - pageSize : m_Scroll;

            StringBuilder sb = new StringBuilder();

            for (int i = init; i < (init + pageSize); i++)
            {
                if (i < 0)
                    continue;
                string line = s_ConsoleData.lines[i];

                if (line.Length > maxCharsInLine)
                    line = line.Substring(0, maxCharsInLine - 4) + " ...";

                sb.AppendLine(line);
            }

            return sb.ToString();
        }

        private void UpdateLog()
        {
            if (s_ConsoleData.lines.Count == 0 || !bConsoleVisible) return;

            // Ensure m_Scroll is Consistent when clearing;
            if (m_Scroll > s_ConsoleData.lines.Count)
                m_Scroll = -1;

            // Update Log Text
            if (LogText != null)
            {
                LogText.text = GetScrolledText(); 
                LogText.GraphicUpdateComplete();
            }

            // Update Scroll Info Text
            if (ScrollInfo != null)
            {
                int pageSize = GetScrollPageSize();
                int count = s_ConsoleData.lines.Count;

                string text = string.Empty;
                if (m_Scroll == -1)
                {
                    ScrollInfo.text = $"*[{Mathf.FloorToInt(count / pageSize)}/{Mathf.CeilToInt(count / pageSize)}]";
                }
                else
                {
                    ScrollInfo.text = $"[{Mathf.FloorToInt(m_Scroll / pageSize)}/{Mathf.CeilToInt(count / pageSize)}]";
                }
                ScrollInfo.GraphicUpdateComplete();
            }
        }
        
        public static void Clear()
        {
            s_ConsoleData.lines.Clear();
            Log("Console","Cleared Output", LogType.Log);

            if (s_ConsoleData.OnLogUpdated != null)
                s_ConsoleData.OnLogUpdated.Invoke();
        }

        #endregion

        #region PEEK

        Dictionary<int, (string,int)> m_PeekData;

        void SetPeekVisible(bool visible)
        {
            if (peekRoot == null)
                return;

            peekRoot.SetActive(visible);
        }

        void ClearPeek()
        {
            if (m_PeekData == null)
                m_PeekData = new Dictionary<int, (string, int)>();
            else
                m_PeekData.Clear();

            UpdatePeek();
        }

        void LogToPeek(string message)
        {
            if (bConsoleVisible || !gameObject.activeSelf)
                return;

            if (!(Debug.isDebugBuild) && !showOnReleaseBuilds)
                return;

            if (Application.isEditor && !showOnEditor)
                return;

            if (m_PeekData == null)
                m_PeekData = new Dictionary<int, (string, int)>();

            int id = Shader.PropertyToID(message);
            if (!m_PeekData.ContainsKey(id))
                m_PeekData.Add(id, (message, 1));
            else
            {
                var val = m_PeekData[id];
                m_PeekData[id]= (val.Item1, val.Item2+1);
            }

            StartCoroutine(RemoveElement(id));
            UpdatePeek();
        }

        IEnumerator RemoveElement(int id)
        {
            yield return new WaitForSeconds(peekDuration);

            if(m_PeekData.ContainsKey(id))
            {
                var val = m_PeekData[id];
                val.Item2--;

                if (val.Item2 > 0)
                    m_PeekData[id] = val;
                else
                    m_PeekData.Remove(id);
            }

            UpdatePeek();
        }

        void UpdatePeek()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var kvp in m_PeekData)
            {
                var val = kvp.Value;
                int count = val.Item2;
                sb.AppendLine($"[{count.ToString("D3")}] {val.Item1}");
            }

            peekText.text = sb.ToString();

            SetPeekVisible(m_PeekData.Count > 0 && !bConsoleVisible);
        }
        


        #endregion

        #region COMMANDS

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

        #endregion

        public class ConsoleData
        {
            public List<string> lines;
            public Dictionary<string, IConsoleCommand> commands;
            public Dictionary<string,string> aliases;
            public System.Action OnLogUpdated;
            public List<string> commandHistory;

            List<View> m_Views;

            public void AddView(View view)
            {
                if (!HasView(view.GetType()))
                    m_Views.Add(view);
            }

            public bool HasView(Type t)
            {
                if (m_Views == null)
                    m_Views = new List<View>();

                foreach (var view in m_Views)
                {
                    if (view.GetType() == t)
                        return true;
                }
                return false;
            }

            public List<View> views { get { return m_Views; } }

            public ConsoleData()
            {
                lines = new List<string>();
                commands = new Dictionary<string, IConsoleCommand>();
                aliases = new Dictionary<string,string>();
                commandHistory = new List<string>();
                m_Views = new List<View>();
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

    }

    #region BUILT-IN COMMANDS

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

        public string summary => "Gets a summary of all commands or shows help for a specific command.";

        public string help => @"Usage: <color=yellow>HELP</color> <i>command</i>
Shows help for specific command or list any available command.
Additional arguments are ignored";

        public IEnumerable<Console.Alias> aliases => null;
    }


    [AutoRegisterConsoleCommand]
    public class Clear : IConsoleCommand
    {
        public void Execute(string[] args)
        {
            if(args.Length >= 1)
            {
                if (args[0].ToLower() == "console")
                    Console.Clear();
                if (args[0].ToLower() == "views")
                    Console.ClearViews();
            }
            else if(args.Length == 0)
            {
                Console.Clear();
                Console.ClearViews();
            }
        }

        public string name => "clear";

        public string summary => "Clears the console output and/or views";

        public string help => @"<color=yellow>Clear</color>
    Usage: clear [console|views]
        clear           -> Clears the Console and Views
        clear console   -> Clears the Console
        clear views     -> Clears all Views";

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
    #endregion
}