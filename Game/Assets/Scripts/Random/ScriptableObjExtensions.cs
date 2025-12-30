using UnityEngine;

public static class ScriptableObjExtensions
{
    public static T GetAs<T>(this ScriptableObject obj) where T : class
    {
        return obj as T;
    }
}

