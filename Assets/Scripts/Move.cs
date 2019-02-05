using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Ability
{
    [Tooltip("Time that it takes to move between two cells.")]
    public float movementDuration = 0.25f;

    private List<Cell> path;
    private Cell current;
    private int pathIndex;
    private float movementTimer;

    private void Start()
    {
        Type = Ability.CastType.Move;
    }

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

        if (target.CurrentState != Cell.State.Empty)
        {
            Debug.LogErrorFormat("Cannot move to cell with state '{0}'", target.CurrentState);
            return;
        }

        if (!MapManager.Instance.AreNeighbours(source, target))
        {
            Debug.LogError("Can cast Push only on neighbour cell");
            return;
        }

        // Debug.LogFormat("Casting Move() from {0} to {1}", source.Position, target.Position);

        Casting = true;
        path = AStar.FindPath(source, target);
        Agent sourceAgent = GameManager.Instance.AgentAt(source);
        StartMoving(sourceAgent);
    }

    private void StartMoving(Agent agent)
    {
        if (Casting)
        {
            Debug.LogError("Cannot accept new move while already moving");
            return;
        }

        if (agent == null)
        {
            Debug.LogError("{agent} is null");
            return;
        }

        if (path == null)
        {
            Debug.LogError("{path} is null");
            return;
        }

        if (path.Count < 1)
        {
            Debug.LogError("{path} must contain at least one element");
            return;
        }

        Casting = true;
        movementTimer = 0f;
        pathIndex = 0;
        current = path[pathIndex];
        current.CurrentState = Cell.State.Empty;
        GameManager.Instance.AgentAt(current).Position = path[path.Count - 1].Position;

        while (current != path[path.Count - 1])
        {
            Cell next = path[pathIndex + 1];

            transform.position = Vector3.Lerp(
                Utilities.ToWorldPosition(current.Position, transform),
                Utilities.ToWorldPosition(next.Position, transform),
                movementTimer / movementDuration);

            if (movementTimer < movementDuration)
                movementTimer += Time.deltaTime;
            else
            {
                movementTimer -= movementDuration;
                current = path[++pathIndex];
            }
        }

        Casting = false;
        path = null;
        pathIndex = 0;
        movementTimer = 0f;
        agent.Position = Utilities.ToMapPosition(transform.position);
        MapManager.Instance.CellAt(agent.Position).CurrentState = Cell.State.Agent;
    }
}