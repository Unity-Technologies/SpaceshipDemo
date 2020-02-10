# Console

A console for use in Unity Projects. The goal is to keep it as simple as possible. The console can register console commands to extend its possibilities. Console commands are easy to write and implement. There are some examples shipped with the base package.

Oh, and the console catches unity Logs.

## Install

Either Clone the repository and reference the `package.json` in the Package Manager UI, or add directly this line to the `Packages/manifest.json` under `dependencies` : 

`"net.peeweek.console": "https://github.com/peeweek/net.peeweek.console.git#1.1.0",`

## How to use

* Press the Backslash `\` key at runtime to toggle the Console (Can be customized if you use the package as local package)
* Use the input field to input commands and execute button or return key to execute.

### Navigation (Defaults)

* Backslash to Open/Close console
* Up/Down arrows to access type command history
* PageUp/PageDown to scroll console log history

### Built-in Commands

* `help` : displays a list of all registered commands with summary
* `help [command]` displays the `GetHelp()` string of given command.
* `clear` clears the console output.

### API Summary
* `using ConsoleUtility;`
* `Console.Log("Module", "Message", LogType.Warning);`for a detailed log and coloration.
* `Console.Log("Module", "Message");`shortcut with `LogType.Log`.
* `Console.Log("Message");`Simplest one, without module.
* `Console.AddCommand(command)`Registers one new command.

## Writing Console Commands

Writing console commands requires writing a Class that implements `IConsoleCommand` interface.

* To manually register a console command use `Console.AddCommand(IConsoleCommand command)`
* To set a console command to register itself automatically use the `    [AutoRegisterConsoleCommand]` class attribute

### Console Command structure

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConsoleUtility
{
    [AutoRegisterConsoleCommand]
    public class MyConsoleCommand : IConsoleCommand
    {
        public void Execute(string[] args)
        {
	     //use args array to parse command. args[] do not include the base command so in
             // command 'mycommand foo bar' args[0] is foo and args[1] is bar
        }

        public string name => "mycommand"; 		// the actual command key
        public string summary => "Does soemthing";	// summary displayed when typing 'help'
        public string help => "usage: mycommand"; 	// help displayed when typing 'help mycommand'

        public IEnumerable<Console.Alias> aliases
        {
	    get
	    {
                yield return Console.Alias.Get("myalias", "mycommand foo bar");
                // yield return any console alias you need, for ease of use purposes
	    }
        }
    }

}

```

