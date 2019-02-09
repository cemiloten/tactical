using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public enum CastType
    {
        None,
        Move,
        Action
    }

    public int range = 0;

    public bool Casting { get; set; }
    public CastType Type { get; protected set; }

    // Execute ability's effect
    public abstract bool Cast(Cell source, Cell target);

    // Cells that can be targeted with this ability, from {source} Cell.
    public abstract List<Cell> Range(Cell source);

    // Set ability's default state
    public abstract IEnumerator Reset();

    private void Awake()
    {
        Casting = false;
    }
}
