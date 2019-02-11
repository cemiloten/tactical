using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Ability
{
    [Tooltip("Time that it takes to move between two cells.")]
    public float movementDuration = 0.25f;

    private Agent agent;
    private int pathIndex;
    private float movementTimer;
    private bool castedFromOther;

    public List<Cell> Path { get; set; }
    public int MovementPoints { get; private set; }

    private void Awake()
    {
        Type = Ability.CastType.Move;
        Reset();
    }

    private void Update()
    {
        if (Casting)
            UpdateMove();
    }

    public override void Reset()
    {
        agent = null;
        MovementPoints = range;
        movementTimer = 0f;
        pathIndex = 0;
        Path = null;
        castedFromOther = false;
    }

    public override List<Cell> Range(Cell source)
    {
        if (source == null)
        {
            Debug.LogError("[source] is null");
            return null;
        }

        return PathMaker.ExpandToWalkables(source, MovementPoints);
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

        if (Path.Count < 1)
        {
            Debug.LogError("[Path] is empty");
            return false;
        }

        StartMoving(source);
        return true;
    }

    public void MoveFromOther(List<Cell> path)
    {
        if (path == null)
        {
            Debug.LogError("[path] is null");
            return;
        }

        if (Casting)
        {
            Debug.LogError("Already Casting");
            return;
        }

        Path = path;
        MovementPoints = int.MaxValue;
        if (Cast(path[0], path[path.Count - 1]))
        {
            castedFromOther = true;
        }
    }

    private void StartMoving(Cell source)
    {
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
    }

    private void UpdateMove()
    {
        if (Path == null)
        {
            Debug.LogFormat("{0}: [Path] is null", this);
            return;
        }

        if (pathIndex < Path.Count)
        {
            Cell current = MapManager.Instance.CellAt(agent.Position);
            if (current == null)
            {
                Debug.Log("current is null");
                return;
            }

            Cell next = Path[pathIndex];
            if (next == null)
            {
                Debug.Log("next is null");
                return;
            }

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
        else
        {
            FinishedMoving();
        }
    }

    private void FinishedMoving()
    {
        Casting = false;
        MovementPoints -= Path.Count;
        Path = null;
        pathIndex = 0;
        movementTimer = 0f;
        agent.Position = Utilities.ToMapPosition(transform.position);
        MapManager.Instance.CellAt(agent.Position).CurrentState = Cell.State.Agent;

        if (castedFromOther)
        {
            Reset();
        }
    }
}