using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public AgentProperties properties;

    private int health = 0;
    private int movement = 0;
    private Cell cell = null;

    public Vector2Int Position { get; private set; }

    public void Initialize(Vector2Int pos)
    {
    }

    public void MoveTo(Vector2Int pos)
    {
        if (cell)
        {
            cell.CurrentState = Cell.State.Empty;
        }
        cell = MapManager.Instance.CellAt(pos);
        cell.CurrentState = Cell.State.Agent;

        Position = pos;
        transform.position = MapManager.ToWorldPosition(pos, transform);
    }
}
