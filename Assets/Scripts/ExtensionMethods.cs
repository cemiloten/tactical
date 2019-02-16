using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    ///<summary>
    ///Return the CellState's corresponding predefined color.
    ///</summary>
    public static Color ToCellColor(this CellState state)
    {
        switch (state)
        {
            case CellState.Empty:    return new Color(1f, 1f, 1f);
            case CellState.Agent:    return new Color(1f, 0f, 0f);
            case CellState.Obstacle: return new Color(0f, 1f, 0f);
            case CellState.Hole:     return new Color(0f, 0f, 1f);
            default: return new Color(0f, 0f, 0f);
        }
    }

    ///<summary>
    ///Returns the normalized vector.
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
