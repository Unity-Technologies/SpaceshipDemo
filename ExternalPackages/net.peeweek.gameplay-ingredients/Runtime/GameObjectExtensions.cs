using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    public static IEnumerable<Transform> GetAllChildren(this Transform transform)
    {
        var stack = new Stack<Transform>();
        stack.Push(transform);
        while(stack.Count != 0)
        {
            var t = stack.Pop();
            yield return t;

            for (int i = 0; i < t.childCount; i++)
            {
                stack.Push(t.GetChild(i));
            }
        }
    }

    public static IEnumerable<GameObject> GetAllChildren(this GameObject gameObject)
    {
        var all = gameObject.transform.GetAllChildren();
        foreach(Transform t in all)
        {
            yield return t.gameObject;
        }
    }
}
