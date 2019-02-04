using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pull : Ability
{
    public int power;

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

        if (!Utilities.IsStraightLine(source.Position, target.Position))
        {
            Debug.LogError("Can cast Pull() only in a straight line");
            return;
        }

        Debug.LogFormat("Casting Pull() from {0} to {1}", source.Position, target.Position);

        List<Cell> path = AStar.FindPathToNearestNeighbour(target, source);
        Agent agent = GameManager.Instance.AgentAt(target);
        agent.Move(path);
    }
}
