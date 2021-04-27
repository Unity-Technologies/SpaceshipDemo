using UnityEngine;
using GameplayIngredients.Actions;

public class OpenURLAction : ActionBase
{
    public string URL = "https://unity3d.com";
    public override void Execute(GameObject instigator = null)
    {
        Application.OpenURL(URL);
    }
}
