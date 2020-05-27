using System;
using Agents;
using UnityEngine;

namespace Abilities {

public enum AbilityType {
    None,
    Move,
    BasicAttack
}

public abstract class Ability : MonoBehaviour {
    protected Agent Agent;
    public int range = 0;

    public bool Casting { get; protected set; }
    public AbilityType Type { get; private set; }

    protected abstract AbilityType SetType();

    public abstract void Cast(Cell source, Cell target, Action onCastEnd = null);

    public bool IsInRange(Cell source, Cell target) =>
        range >= Utilities.Distance(source.Position, target.Position);

    private void Awake() {
        Type = SetType();
        Agent = GetComponent<Agent>();
    }
}

}