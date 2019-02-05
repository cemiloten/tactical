using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Ability
{
    [Tooltip("Time that it takes to move between two cells.")]
    public float movementDuration = 0.25f;

    private List<Cell> path;
    private Agent agent;
    private int pathIndex;
    private float movementTimer;

    private void Awake()
    {
        Type = Ability.CastType.Move;
    }

    private void Update()
    {
        if (Casting && path != null)
            MoveToNextCell();
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

        if (Casting)
        {
            Debug.LogError("Cannot accept new move while moving");
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

        Debug.LogFormat("Casting Move() from {0} to {1}", source.Position, target.Position);

        path = AStar.FindPath(source, target);
        StartMoving(source);
    }

    private void StartMoving(Cell source)
    {
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
        agent = GameManager.Instance.AgentAt(source);
        source.CurrentState = Cell.State.Empty;
        movementTimer = 0f;
        pathIndex = 0;
    }

    private void MoveToNextCell()
    {
        if (pathIndex >= path.Count)
        {
            FinishedMoving();
            return;
        }
        Cell current = MapManager.Instance.CellAt(agent.Position);
        Cell next = path[pathIndex];

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
    }

    private void FinishedMoving()
    {
        Casting = false;
        path = null;
        pathIndex = 0;
        movementTimer = 0f;
        agent.Position = Utilities.ToMapPosition(transform.position);
        MapManager.Instance.CellAt(agent.Position).CurrentState = Cell.State.Agent;
    }
}