using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public enum State
    {
        Empty    = 0,
        Agent    = 1,
        Obstacle = 2
    }

    public State CurrentState { get; set; }
    public Vector2Int Position { get; set; }

    public Cell(Vector2Int _position, State _state = State.Empty)
    {
       Position = _position; 
       CurrentState = _state;
    }
}