using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pull : Ability
{
    public int power;

    private void Awake()
    {
        Type = Ability.CastType.Action;
    }

    public override List<Cell> Range(Cell source)
    {
       return PathMaker.GetStraightLine(source, range);
    }

    public override void Cast(Cell source, Cell target)
    {
        Debug.Log("calling");
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

        if (source.CurrentState != Cell.State.Agent)
        {
            Debug.LogErrorFormat("[source] state must be Agent, is {0} instead", source.CurrentState);
            return;
        }

        if (target.CurrentState != Cell.State.Agent)
        {
            Debug.LogErrorFormat("[target] state must be Agent, is {0} instead", target.CurrentState);
            return;
        }

        if (!Utilities.IsStraightLine(source.Position, target.Position))
        {
            Debug.LogError("Can cast Pull() only in a straight line");
            return;
        }

        Debug.LogFormat("Casting Pull() from {0} to {1}", source.Position, target.Position);

        Vector2Int direction = source.Position - target.Position;
        direction = direction.Normalized();

        List<Cell> path = PathMaker.StraightPath(target, direction, power);
        if (path.Count <= 1)
        {
            Debug.Log("Path too short, cannot pull");
            return;
        }

        Agent targetAgent = GameManager.Instance.AgentAt(target);
        Ability ability = targetAgent.CurrentAbility;
        if (ability.Type != Ability.CastType.Move)
            targetAgent.SetCurrentAbility(Ability.CastType.Move);
        Move move = ability as Move;
        move.Path = path;
        move.Cast(path[0], path[path.Count - 1]);
        targetAgent.SetCurrentAbility(ability.Type);
    }
}
