using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public AgentProperties properties;

    private int health = 0;
    private int movement = 0;
    private Cell cell = null;

    public void MoveTo(Cell _cell)
    {
        cell.CurrentState = Cell.State.Empty;        
        cell = _cell;
        cell.CurrentState = Cell.State.Agent;
    }
}
