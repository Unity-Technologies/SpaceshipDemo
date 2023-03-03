# Console Changelog

## [2021-05-16] - 1.5.2

* Fixed Input for View Cycling / Remove.

## [2021-05-08] - 1.5.1

* Fixed Default unused Prefabs that throwed warnings during build for other input systems.

## [2021-05-03] - 1.5.0

* Added Peek Feature
* Better handling of prefabs (now you can duplicate prefabs from Package Resources into your Assets/Resources and remove the Default_ prefix to create your own variant)

## [2021-04-24] - 1.4.0

* Added Support for new Input System
* Separated Input as `ConsoleInput` abstract behaviors.
* Console now spawns from different prefabs based on the presence of input system package and usage in player settings (prioritizes to legacy if both are used)

## [2021-02-08] - 1.3.0

* Added a `onConsoleToggle` event to hook into when the console visibility changes.
* Added API & Functionality for debug views.

## [2021-01-13] - 1.2.0

Minimal version is now 2019.4

* Added a `Console.ExecuteCommand(string command)` to execute a command from script.
* Fixed namespace in `ConsoleUtility.cs` preventing to Reference `Console.Alias` directly in custom classes.

## [2019-08-24] - 1.1.0

* No more preview label for the package
* Got rid of UGUI ScrollBar, only displays one TextField
* Scrolling uses PageUp/PageDown and custom scrolling system to reduce text rendering overhead
* In order to avoid using `Console.Console` even when `using Console;`due to ambiguous calls, `Console` namespace has been renamed to `ConsoleUtility`
* Ensure command repeat history do not store contiguous duplicates.

## [2019-04-10] - 1.0.1-preview

* Make Console Log Stacktraces on Error/Exception
* Disabled EventSystem GameObject (Can be enabled if used locally)
* Fixed Time Digits

## [2019-01-15] - 1.0.0-preview

* Initial Release
