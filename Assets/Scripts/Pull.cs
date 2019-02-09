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

    public override IEnumerator Reset() { return null; }

    public override List<Cell> Range(Cell source)
    {
       return PathMaker.GetStraightLine(source, range);
    }

    public override bool Cast(Cell source, Cell target)
    {
        Debug.Log("calling");
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
            Debug.LogErrorFormat("[source] state must be Agent, is {0} instead", source.CurrentState);
            return false;
        }

        if (target.CurrentState != Cell.State.Agent)
        {
            Debug.LogErrorFormat("[target] state must be Agent, is {0} instead", target.CurrentState);
            return false;
        }

        if (!Utilities.IsStraightLine(source.Position, target.Position))
        {
            Debug.LogError("Can cast Pull() only in a straight line");
            return false;
        }

        Debug.LogFormat("Casting Pull() from {0} to {1}", source.Position, target.Position);

        Vector2Int direction = source.Position - target.Position;
        direction = direction.Normalized();

        List<Cell> path = PathMaker.StraightPath(target, direction, power);
        if (path.Count <= 1)
        {
            Debug.Log("Path too short, cannot pull");
            return false;
        }

        Agent targetAgent = GameManager.Instance.AgentAt(target);
        if (targetAgent == null)
        {
            Debug.LogError("[targetAgent] is null");
            return false;
        }

        Ability ability = targetAgent.CurrentAbility;
        if (ability == null)
        {
            Debug.LogError("[ability] is null");
            return false;
        }
        if (ability.Type != Ability.CastType.Move)
            targetAgent.SetCurrentAbility(Ability.CastType.Move);

        Move move = ability as Move;
        if (move == null)
        {
            Debug.LogError("[move] is null");
            return false;
        }

        move.Path = path;
        if (move.Path == null)
        {
            Debug.LogError("[move.Path] is null");
        }

        move.movementPoints = 99;
        move.Cast(path[0], path[path.Count - 1]);
        targetAgent.SetCurrentAbility(ability.Type);
        return true;
    }
}
