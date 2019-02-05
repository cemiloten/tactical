using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push : Ability
{
    public int power = 3;

    private void Awake()
    {
        Type = Ability.CastType.Action;
    }

    public override void Cast(Cell source, Cell target)
    {
        if (source == null)
        {
            Debug.LogError("{source} is null");
            return;
        }

        if (target == null)
        {
            Debug.LogError("{target} is null");
            return;
        }

        if (source.CurrentState != Cell.State.Agent)
        {
            Debug.LogErrorFormat("{source} state must be Agent, is {0} instead", source.CurrentState);
            return;
        }

        if (target.CurrentState != Cell.State.Agent)
        {
            Debug.LogErrorFormat("{target} state must be Agent, is {0} instead", target.CurrentState);
            return;
        }

        if (!MapManager.Instance.AreNeighbours(source, target))
        {
            Debug.LogError("Can cast Push only on neighbour cell");
            return;
        }

        Debug.LogFormat("Casting Push() from {0} to {1}", source.Position, target.Position);

        Agent targetAgent = GameManager.Instance.AgentAt(target);
        Vector2Int direction = target.Position - source.Position;
        List<Cell> path = new List<Cell>();
        for (int i = 1; i <= power; ++i)
        {
            Cell cell = MapManager.Instance.CellAt(
                target.Position + new Vector2Int(i * direction.x, i * direction.y));
            if (cell == null || !cell.Walkable)
                break;
            path.Add(cell);
        }
        // targetAgent.Move(path);
    }
}
