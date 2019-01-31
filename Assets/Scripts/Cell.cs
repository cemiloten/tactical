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

    private Vector2Int position;

    public State CurrentState { get; set; }
    public bool Hover { get; set; }

    public void Initialize(Vector2Int pos, State state = State.Empty)
    {
        position = pos;
        CurrentState = state;
        gameObject.transform.position = MapManager.ToWorldPosition(pos);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = MapManager.Instance.HoverCell == this ? Color.blue : Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}