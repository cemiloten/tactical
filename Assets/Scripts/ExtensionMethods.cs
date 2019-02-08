using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    ///<summary>
    ///Returns the normalized vector
    ///</summary>
    public static Vector2Int Normalized(this Vector2Int v)
    {
        // todo: refactor
        int x;
        if (v.x > 1)
        {
            x = v.x / v.x;
        }
        else if (v.x < 0)
        {
            x = -(v.x / v.x);
        }
        else
        {
            x = v.x;
        }

        int y;
        if (v.y > 1)
        {
            y = v.y / v.y;
        }
        else if (v.y < 0)
        {
            y = -(v.y / v.y);
        }
        else
        {
            y = v.y;
        }

        return new Vector2Int(x, y);
    }
}
