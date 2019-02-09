using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Ability
{
    [Tooltip("Time that it takes to move between two cells.")]
    public float movementDuration = 0.25f;

    [Tooltip("Maximum distance that the agent can travel in one turn")]
    public int movementPoints = 3;

    private Agent agent;
    private int pathIndex = 0;
    private float movementTimer = 0;
    public List<Cell> Path { get; set; }

    private void Awake()
    {
        Type = Ability.CastType.Move;
    }

    public override IEnumerator Reset()
    {
        if (Casting)
            // Wait for movement to end
            yield return null;

        movementPoints = range;
        movementTimer = 0f;
        pathIndex = 0;
        Path = null;
    }

    public override List<Cell> Range(Cell source)
    {
        if (source == null)
        {
            Debug.LogError("[source] is null");
            return null;
        }

        return PathMaker.ExpandToWalkables(source, movementPoints);
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
            Debug.LogError("Cannot accept new move while moving");
            return false;
        }

        if (source.CurrentState != Cell.State.Agent)
        {
            Debug.LogErrorFormat("[source], {0}: state must be Agent, is {1} instead", source.Position, source.CurrentState);
            return false;
        }

        if (target.CurrentState != Cell.State.Empty)
        {
            Debug.LogErrorFormat("Cannot move to cell with state '{0}'", target.CurrentState);
            return false;
        }

        if (Path == null)
        {
            Debug.Log("[Path] is null, finding fastest path with AStar");
            Path = PathMaker.AStar(source, target);
        }

        StartMoving(source);
        StartCoroutine("Reset");
        return true;
    }

    private void StartMoving(Cell source)
    {
        if (Path == null)
        {
            Debug.LogError("[Path] is null, cannot start moving");
            return;
        }

        if (Path.Count < 1)
        {
            Debug.LogError("[Path] is empty");
            return;
        }

        Casting = true;
        source.CurrentState = Cell.State.Empty;
        agent = GameManager.Instance.AgentAt(source);
        if (agent == null)
        {
            Debug.LogErrorFormat("Did not find agent at {0}", source.Position);
            return;
        }
        movementTimer = 0f;
        pathIndex = 0;
        StartCoroutine("UpdateMove");
    }

    private IEnumerator UpdateMove()
    {
        while (pathIndex < Path.Count)
        {
            Cell current = MapManager.Instance.CellAt(agent.Position);
            Cell next = Path[pathIndex];
            transform.position = Vector3.Lerp(
                Utilities.ToWorldPosition(current.Position, transform),
                Utilities.ToWorldPosition(next.Position, transform),
                movementTimer / movementDuration);

            if (movementTimer < movementDuration)
                movementTimer += Time.deltaTime;
            else
            {
                movementTimer -= movementDuration;
                agent.Position = next.Position;
                ++pathIndex;
            }
            yield return null;
        }

        FinishedMoving();
    }

    private void FinishedMoving()
    {
        Casting = false;
        movementPoints -= Path.Count;
        Path = null;
        pathIndex = 0;
        movementTimer = 0f;
        agent.Position = Utilities.ToMapPosition(transform.position);
        MapManager.Instance.CellAt(agent.Position).CurrentState = Cell.State.Agent;
    }
}