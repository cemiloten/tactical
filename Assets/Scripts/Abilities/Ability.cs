using System;
using Agents;
using UnityEngine;

namespace Abilities {

public enum AbilityType {
    None,
    Move,
    BasicAttack
}

[RequireComponent(typeof(Agent))]
public abstract class Ability : MonoBehaviour {
    protected Agent Agent;
    public int cost = 1;
    public int range = 0;

    public bool Casting { get; protected set; }
    public AbilityType Type { get; private set; }
    public bool HasDynamicCost { get; private set; }

    protected abstract AbilityType SetType();
    protected virtual bool SetHasDynamicCost() => false;

    // todo: cast API is very unclear, can be called from here but also from agent
    public abstract void Cast(Cell source, Cell target, Action onCastEnd = null,
                              bool endTurn = false);

    public bool IsInRange(Cell source, Cell target) => range >= Utilities.Distance(source, target);

    public virtual int CalculateDynamicCost(Cell source, Cell target) => 0;

    private void Awake() {
        Type = SetType();
        HasDynamicCost = SetHasDynamicCost();

        Agent = GetComponent<Agent>();
    }
}

}
