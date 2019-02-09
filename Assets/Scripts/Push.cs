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

    public override IEnumerator Reset() { return null; }

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
            Debug.LogError("[source] state must be Cell.StateAgent");
            return false;
        }

        if (target.CurrentState != Cell.State.Agent)
        {
            Debug.LogError("[target] state must be Cell.State.Agent");
            return false;
        }

        if (!MapManager.Instance.AreNeighbours(source, target))
        {
            Debug.LogError("Can cast Push only on neighbour cell");
            return false;
        }

        Debug.LogFormat("Casting Push() from {0} to {1}", source.Position, target.Position);

        Vector2Int direction = target.Position - source.Position;
        List<Cell> path = PathMaker.StraightPath(target, direction, power);
        if (path.Count <= 1)
        {
            Debug.Log("Path too short, cannot move");
            return false;
        }

        // Agent targetAgent = GameManager.Instance.AgentAt(target);
        // Ability ability = targetAgent.CurrentAbility;
        // if (ability != null && ability.Type != Ability.CastType.Move)
        //     targetAgent.SetCurrentAbility(Ability.CastType.Move);
        // Move move = ability as Move;
        // move.movementPoints = 99;
        // move.Path = path;
        // move.Cast(path[0], path[path.Count - 1]);
        // move.Reset();
        // targetAgent.SetCurrentAbility(ability.Type);
        Debug.Log("not implemented");
        return true;
    }
}
