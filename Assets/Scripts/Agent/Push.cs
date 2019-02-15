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

    public override List<Cell> Range(Cell source)
    {
        return PathMaker.GetNeighbours(source);
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
            Debug.LogWarning("[source] cell of Push must contain an Agent");
            return false;
        }

        if (target.CurrentState != Cell.State.Agent)
        {
            Debug.LogWarning("[target] cell of Push state must contain an Agent");
            return false;
        }

        if (!MapManager.Instance.AreNeighbours(source, target))
        {
            Debug.LogError("Can cast Push only on neighbour cell");
            return false;
        }

        Vector2Int direction = target.Position - source.Position;
        List<Cell> path = PathMaker.StraightPath(target, direction, power);
        if (path == null)
        {
            Debug.Log("Result from StraightPath is null");
            return false;
        }

        if (path.Count <= 1)
        {
            Debug.Log("Result from StraightPath is too short, cannot push");
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
