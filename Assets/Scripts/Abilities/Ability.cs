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

    public abstract bool Cast(Cell source, Cell target);

    private void Awake() {
        Type = SetType();
        Agent = GetComponent<Agent>();
    }
}
}
