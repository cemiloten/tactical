using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    None,
    Move,
    Action
}

public abstract class Ability : MonoBehaviour
{
    public int range = 0;

    public bool Casting { get; protected set; }
    public AbilityType Type { get; protected set; }

    public abstract bool Cast(Cell source, Cell target);

    // Cells that can be targeted with this ability, from {source} Cell.
    public abstract List<Cell> Range(Cell source);

    // Set ability's default state
    public virtual void Reset()
    {
        Debug.LogFormat("Reset {0}", this);
    }

    private void Awake()
    {
        Casting = false;
    }
}
