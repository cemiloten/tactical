using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public enum State
    {
        Empty,
        Agent,
        Obstacle,
        Hole
    }

    public Color Color { get; set; }
    public State CurrentState { get; set; }
    public Vector2Int Position { get; private set; }
    public bool Walkable { get { return CurrentState == State.Empty || CurrentState == State.Hole; } }

    public void Initialize(Vector2Int pos)
    {
        Position = pos;
        CurrentState = State.Empty;
        transform.position = Utilities.ToWorldPosition(pos, transform);
        Color = new Color(0, 0, 0, 0);
    }

    void OnDrawGizmos()
    {
        Color withAlpha = new Color(this.Color.r, this.Color.g, this.Color.b, 0.55f);
        Gizmos.color = withAlpha;
        Gizmos.DrawCube(transform.position, new Vector3(0.8f, 0.01f, 0.8f));
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
