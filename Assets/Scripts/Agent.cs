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
    private Vector2Int movingTo = new Vector2Int(-1, -1);
    private float movementSpeed = 2f;
    private float epsilon = 0.05f;

    public Vector2Int Position { get; private set; }


    void Update()
    {
       if (isMoving)
       {
           if (transform.position.x < movingTo.x + 0.5f - epsilon)
           {
               transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
           }
           else if (transform.position.z < movingTo.y + 0.5f - epsilon)
           {
               transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
           }
           else if (transform.position.x > movingTo.x + 0.5f + epsilon)
           {
               transform.Translate(-Vector3.right * movementSpeed * Time.deltaTime);
           }
           else if (transform.position.z > movingTo.y + 0.5f + epsilon)
           {
               transform.Translate(-Vector3.forward * movementSpeed * Time.deltaTime);
           }
           else
           {
               Position = movingTo;
               isMoving = false;
           }
       }
    }

    public bool CanMoveTo(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0)
            return false;

        return movement >= MapManager.Distance(Position, pos);
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
        {
            cell.CurrentState = Cell.State.Empty;
        }

        cell = MapManager.Instance.CellAt(pos);
        cell.CurrentState = Cell.State.Agent;
        isMoving = true;
        movingTo = pos;
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
