using UnityEngine;

public static class Utilities {
    public static int Distance(Vector2Int start, Vector2Int end) =>
        Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y);

    public static bool IsStraightLine(Vector2Int start, Vector2Int end) =>
        end.x - start.x == 0 || end.y - start.y == 0;

    // Mouse position translated to world position as cell coordinates.
    public static bool MousePositionOnMap(out Vector2Int mousePosition) {
        mousePosition = new Vector2Int(-1, -1);
        if (!Camera.main)
            return false;

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

        return false;
    }

    public static Vector2Int ToMapPosition(Vector3 pos) => new Vector2Int((int)pos.x, (int)pos.z);

    public static Vector3 ToWorldPosition(this Vector2Int pos) =>
        new Vector3(pos.x + 0.5f, 0f, pos.y + 0.5f);
}