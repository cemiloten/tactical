using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public enum CastType
    {
        Move,
        Action
    }

    public int range = 0;

    public bool Casting { get; set; }
    public CastType Type { get; protected set; }

    public abstract void Cast(Cell source, Cell target);

    private void Start()
    {
        Casting = false;
    }
}
