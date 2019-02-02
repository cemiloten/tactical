using UnityEngine;

public static class Utilities
{
    public static int Distance(Vector2Int start, Vector2Int end)
    {
        return (Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y));
    }

    // Mouse position translated to world position as cell coordinates.
    public static bool MousePos(out Vector2Int mousePosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(
            Camera.main.ScreenPointToRay(Input.mousePosition),
            out hit,
            25f,
            LayerMask.GetMask("Map"))) // note: this layer mask here could lead to problems later
        {
            mousePosition = ToMapPosition(hit.point);
            return true; 
        }

        mousePosition = new Vector2Int(0, 0);
        return false;
    }

    public static Vector2Int ToMapPosition(Vector3 pos)
    {
       return new Vector2Int((int)pos.x, (int)pos.z);  
    }

    public static Vector3 ToWorldPosition(Vector2Int pos, Transform transform = null)
    {
        float y = transform == null ? 0f : transform.position.y;
        return new Vector3(pos.x + 0.5f, y, pos.y + 0.5f);
    }
}