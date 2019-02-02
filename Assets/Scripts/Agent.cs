using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private Cell cell = null;
    private int movementPoints = 99;

    private int pathIndex = 0;
    private float movementSpeed = 2f;
    private float epsilon = 0.05f;

    public List<Cell> Path { get; set; }
    public Vector2Int Position { get; private set; }
    public bool IsMoving { get; set; } 


    void Start()
    {
        IsMoving = false;
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
        if (pathIndex == Path.Count - 1)
        {
            FinishedMoving();
            return;
        }

        Cell next = Path[pathIndex + 1];
        // todo: use mathf.sin to know direction

        if (transform.position.x < next.Position.x + 0.5f - epsilon)
        {
            transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
        }
        else if (transform.position.z < next.Position.y + 0.5f - epsilon)
        {
            transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
        }
        else if (transform.position.x > next.Position.x + 0.5f + epsilon)
        {
            transform.Translate(-Vector3.right * movementSpeed * Time.deltaTime);
        }
        else if (transform.position.z > next.Position.y + 0.5f + epsilon)
        {
            transform.Translate(-Vector3.forward * movementSpeed * Time.deltaTime);
        }
        else
        {
            ++pathIndex;
        }

    }

    public bool CanMoveTo(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0)
            return false;

        Cell cell = MapManager.Instance.CellAt(pos);
        if (cell == null)
            return false;

        return (cell.Walkable
            && movementPoints >= Utilities.Distance(Position, pos));
    }

    public void MoveTo(Vector2Int pos)
    {
        if (IsMoving)
        {
            Debug.LogError("Cannot accept new move while already moving");
            return;
        }

        if (!CanMoveTo(pos))
        {
            Debug.LogErrorFormat("Cannot move from {0} to {1}", Position, pos);
            return;
        }

        if (cell)
            cell.CurrentState = Cell.State.Empty;

        if (Path == null)
        {
            Cell goal = MapManager.Instance.CellAt(pos);
            Path = AStar.FindPath(cell, goal);
        }

        StartMoving();
    }

    private void StartMoving()
    {
        IsMoving = true;
        pathIndex = 0;
    }

    private void FinishedMoving()
    {
        IsMoving = false;
        pathIndex = 0;
        Position = Utilities.ToMapPosition(transform.position);
    }

    public void SnapTo(Vector2Int pos)
    {
        if (cell)
            cell.CurrentState = Cell.State.Empty;

        cell = MapManager.Instance.CellAt(pos);
        cell.CurrentState = Cell.State.Agent;
        Position = pos;
        transform.position = Utilities.ToWorldPosition(pos, transform);
    }
}
