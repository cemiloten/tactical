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

        if (source.CurrentState != Cell.State.Agent)
        {
            Debug.LogWarningFormat("Cannot cast Pull with {0} as [source], must be Agent", source.CurrentState);
            return false;
        }

        if (target.CurrentState != Cell.State.Agent)
        {
            Debug.LogWarningFormat("Cannot cast Pull with {0} as [target], must be Agent", target.CurrentState);
            return false;
        }

        if (!Utilities.IsStraightLine(source.Position, target.Position))
        {
            Debug.LogWarning("Can cast Pull only in a straight line");
            return false;
        }

        Vector2Int direction = (source.Position - target.Position).Normalized();
        List<Cell> path = PathMaker.StraightPath(target, direction, power);
        if (path.Count <= 1)
        {
            Debug.Log("Path too short, cannot pull");
            return false;
        }

        Agent targetAgent = GameManager.Instance.AgentAt(target);
        Ability ability = targetAgent.CurrentAbility;
        if (ability != null && ability.Type != Ability.CastType.Move)
            targetAgent.SetCurrentAbility(Ability.CastType.Move);

        Move move = targetAgent.CurrentAbility as Move;
        move.MoveFromOther(path);

        targetAgent.SetCurrentAbility(ability.Type);
        return true;
    }
}
