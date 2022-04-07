using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectExtensions
{
    internal static Vector3 ToVector3(this Vector2 source)
    {
        Vector3 returnVector = new Vector3(source.x, source.y, 0f);
        return returnVector;
    }
}
