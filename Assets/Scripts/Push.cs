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

        if (source.CurrentState != Cell.State.Agent)
        {
            Debug.LogError("[source] state must be Cell.StateAgent");
            return;
        }

        if (target.CurrentState != Cell.State.Agent)
        {
            Debug.LogError("[target] state must be Cell.State.Agent");
            return;
        }

        if (!MapManager.Instance.AreNeighbours(source, target))
        {
            Debug.LogError("Can cast Push only on neighbour cell");
            return;
        }

        Debug.LogFormat("Casting Push() from {0} to {1}", source.Position, target.Position);

        Vector2Int direction = target.Position - source.Position;
        List<Cell> path = PathMaker.StraightPath(target, direction, power);
        if (path.Count <= 1)
        {
            Debug.Log("Path too short, no need to move");
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
