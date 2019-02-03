using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public AgentProperties properties;

    private Cell cell = null;
    private int pathIndex = 0;
    private float movementSpeed = 3f;
    private float epsilon = 0.1f;

    public List<Cell> Path { get; set; }
    public bool IsMoving { get; set; } 
    public Vector2Int Position { get; private set; }
    public int MovementPoints { get; private set; }


    void Start()
    {
        IsMoving = false;
        MovementPoints = 5;
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

        // todo: use mathf.sin to know direction
        Cell next = Path[pathIndex];

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
        if (cell)
            cell.CurrentState = Cell.State.Empty;
        IsMoving = true;
        pathIndex = 0;
    }

    private void FinishedMoving()
    {
        IsMoving = false;
        Path = null;
        pathIndex = 0;
        Position = Utilities.ToMapPosition(transform.position);
        transform.position = Utilities.ToWorldPosition(Position, transform);
        cell = MapManager.Instance.CellAt(Position);
        cell.CurrentState = Cell.State.Agent;
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
