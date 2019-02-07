using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swap : Ability
{
    private Agent agent;

    private void Awake()
    {
        Type = Ability.CastType.Action;
    }

    private void Update()
    {
    }

    public override List<Cell> Range(Cell source)
    {
       return PathMaker.GetKnightRange(source);
    }

    public override void Cast(Cell source, Cell target)
    {
        if (source == null)
        {
            Debug.LogError("[source] is null");
            return;
        }

        if (target == null)
        {
            Debug.LogError("[target] is null");
            return;
        }

        if (Casting)
        {
            Debug.LogError("Cannot accept new Swap while casting");
            return;
        }

        if (source.CurrentState != Cell.State.Agent)
        {
            Debug.LogErrorFormat("[source] at {0}: state must be Agent, is {1} instead", source.Position, source.CurrentState);
            return;
        }

        if (target.CurrentState != Cell.State.Agent)
        {
            Debug.LogErrorFormat("[target] at {0}: state must be Agent, is {1} instead", target.Position, target.CurrentState);
            return;
        }

        Debug.LogFormat("Casting Swap() from {0} to {1}", source.Position, target.Position);

        Agent sourceAgent = GameManager.Instance.AgentAt(source);
        Agent targetAgent = GameManager.Instance.AgentAt(target);

        Vector2Int temp = source.Position;
        sourceAgent.Position = targetAgent.Position;
        sourceAgent.transform.position = Utilities.ToWorldPosition(sourceAgent.Position, transform);

        targetAgent.Position = temp;
        targetAgent.transform.position = Utilities.ToWorldPosition(targetAgent.Position, transform);
    }
}