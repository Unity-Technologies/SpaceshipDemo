using GameplayIngredients;
using GameplayIngredients.Events;

public class ConsoleToggleEvent : EventBase
{
    public Callable[] onConsoleShow;
    public Callable[] onConsoleHide;

    private void OnEnable()
    {
        ConsoleUtility.Console.onConsoleToggle += Console_onConsoleToggle;
    }
    private void OnDisable()
    {
        ConsoleUtility.Console.onConsoleToggle -= Console_onConsoleToggle;
    }

    private void Console_onConsoleToggle(bool visible)
    {
        if (visible)
            Callable.Call(onConsoleShow);
        else
            Callable.Call(onConsoleHide);
    }
}
