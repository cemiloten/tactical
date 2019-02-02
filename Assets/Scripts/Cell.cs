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
    public bool Walkable { get { return CurrentState == State.Empty; } }
    public Color Color { get; set; }

    public void Initialize(Vector2Int pos, State state = State.Empty)
    {
        Position = pos;
        CurrentState = state;
        gameObject.transform.position = Utilities.ToWorldPosition(pos);
        Color = Color.yellow;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color;
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