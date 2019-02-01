using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public AgentProperties properties;

    private int health = 0;
    private int movement = 99;
    private Cell cell = null;

    private bool isMoving = false;
    private List<Cell> path = new List<Cell>();
    private int pathIndex = 0;

    private float movementSpeed = 2f;
    private float epsilon = 0.05f;

    public Vector2Int Position { get; private set; }


    void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
       if (isMoving && path != null)
       {
           MoveToNextCell();
       }
    }

    private void MoveToNextCell()
    {
        if (pathIndex == path.Count - 1)
            return;

        Cell next = path[pathIndex + 1];
        // todo: use mathf.sin to know direction


        // if (transform.position.x < path[pathIndex + 1].Position.x + 0.5f - epsilon)
        // {
        //     transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
        // }
        // else if (transform.position.z < path[pathIndex + 1].Position.y + 0.5f - epsilon)
        // {
        //     transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
        // }
        // else if (transform.position.x > path[pathIndex + 1].Position.x + 0.5f + epsilon)
        // {
        //     transform.Translate(-Vector3.right * movementSpeed * Time.deltaTime);
        // }
        // else if (transform.position.z > path[pathIndex + 1].Position.y + 0.5f + epsilon)
        // {
        //     transform.Translate(-Vector3.forward * movementSpeed * Time.deltaTime);
        // }
        // else
        // {
        //     // finished moving, iterate
        // }

    }

    public bool CanMoveTo(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0)
            return false;

        Cell cell = MapManager.Instance.CellAt(pos);
        if (cell == null)
            return false;

        return (movement >= MapManager.Distance(Position, pos)
            && cell.Walkable);
    }

    public void MoveTo(Vector2Int pos)
    {
        if (isMoving)
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

        Cell goal = MapManager.Instance.CellAt(pos);
        path = AStar.FindPath(cell, goal);
        isMoving = true;
    }

    public void SnapTo(Vector2Int pos)
    {
        if (cell)
            cell.CurrentState = Cell.State.Empty;

        cell = MapManager.Instance.CellAt(pos);
        cell.CurrentState = Cell.State.Agent;
        Position = pos;
        transform.position = MapManager.ToWorldPosition(pos, transform);
    }
}
