using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public enum State
    {
        Empty    = 0,
        Agent    = 1,
        Obstacle = 2
    }


    public Vector2Int Position { get; private set; }
    public State CurrentState { get; set; }
    public bool Hover { get; set; }
    public bool Walkable { get { return CurrentState == State.Empty; } }

    public void Initialize(Vector2Int pos, State state = State.Empty)
    {
        Position = pos;
        CurrentState = state;
        gameObject.transform.position = MapManager.ToWorldPosition(pos);
    }


    void OnDrawGizmos()
    {
        // Gizmos.color = MapManager.Instance.HoverCell == this ? Color.blue : Color.yellow;
        Gizmos.color = Color.yellow;
        if (MapManager.Instance.path != null)
        {
            if (MapManager.Instance.path.Contains(this))
                Gizmos.color = Color.green;
        }

        Gizmos.DrawCube(transform.position, new Vector3(0.95f, 0.01f, 0.95f));
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
        Cell other = (Cell)obj; 
        return Position == other.Position && CurrentState == other.CurrentState;
    }
    
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}