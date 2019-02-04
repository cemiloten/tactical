using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public AgentProperties properties;

    private int pathIndex = 0;

    private float movementDuration = 0.25f;
    private float movementTimer = 0f;

    public List<Cell> Path { get; set; }
    public bool IsMoving { get; set; }
    public int MovementPoints { get; private set; }
    public Vector2Int Position { get; private set; }
    public Ability[] Abilities { get; private set; }
    public int CurrentAbility { get; set; }

    void Start()
    {
        Abilities = GetComponents<Ability>();
        CurrentAbility = -1;
        IsMoving = false;
        MovementPoints = 9;
    }

    void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
       if (IsMoving && Path != null)
       {
           MoveToNextCell();
       }
    }

    private void MoveToNextCell()
    {
        if (pathIndex >= Path.Count)
        {
            FinishedMoving();
            return;
        }

        Cell current = MapManager.Instance.CellAt(Position);
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
            Position = next.Position;
            ++pathIndex;
        }
    }

    public void Move(List<Cell> path)
    {
        if (IsMoving)
        {
            Debug.LogError("Cannot accept new move while already moving");
            return;
        }
        Path = path;
        StartMoving();
    }

    private void StartMoving()
    {
        IsMoving = true;
        pathIndex = 0;
        movementTimer = 0f;
        MapManager.Instance.CellAt(Position).CurrentState = Cell.State.Empty;
    }

    private void FinishedMoving()
    {
        IsMoving = false;
        Path = null;
        pathIndex = 0;
        movementTimer = 0f;
        Position = Utilities.ToMapPosition(transform.position);
        MapManager.Instance.CellAt(Position).CurrentState = Cell.State.Agent;
    }

    public void SnapTo(Vector2Int pos)
    {
        MapManager.Instance.CellAt(Position).CurrentState = Cell.State.Empty;
        Position = pos;
        transform.position = Utilities.ToWorldPosition(pos, transform);
        MapManager.Instance.CellAt(Position).CurrentState = Cell.State.Agent;
    }
}
