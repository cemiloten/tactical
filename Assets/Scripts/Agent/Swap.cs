using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swap : Ability
{
    private Agent agent;

    private void Awake()
    {
        Type = AbilityType.Action;
    }

    public override List<Cell> Range(Cell source)
    {
       return PathMaker.GetKnightRange(source);
    }

    public override bool Cast(Cell source, Cell target)
    {
        if (source == null)
        {
            Debug.LogError("[source] is null");
            return false;
        }

        if (target == null)
        {
            Debug.LogError("[target] is null");
            return false;
        }

        if (Casting)
        {
            Debug.LogError("Cannot accept new Swap while casting");
            return false;
        }

        if (source.Type != CellType.Agent)
        {
            Debug.LogErrorFormat("[source] at {0}: state must be Agent, is {1} instead", source.Position, source.Type);
            return false;
        }

        if (target.Type != CellType.Agent)
        {
            Debug.LogErrorFormat("[target] at {0}: state must be Agent, is {1} instead", target.Position, target.Type);
            return false;
        }

        Debug.LogFormat("Casting Swap() from {0} to {1}", source.Position, target.Position);

        Agent sourceAgent = MapManager.Instance.AgentAt(source);
        Agent targetAgent = MapManager.Instance.AgentAt(target);

        Vector2Int temp = source.Position;
        sourceAgent.Position = targetAgent.Position;
        sourceAgent.transform.position = Utilities.ToWorldPosition(sourceAgent.Position, transform);

        targetAgent.Position = temp;
        targetAgent.transform.position = Utilities.ToWorldPosition(targetAgent.Position, transform);

        return true;
    }
}