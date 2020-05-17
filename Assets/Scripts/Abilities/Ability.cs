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
    public AbilityType Type { get; private set; }

    protected Agent Agent;

    protected abstract AbilityType SetType();

    public abstract bool Cast(Cell source, Cell target);

    private void Awake()
    {
        Type = SetType();
        Agent = GetComponent<Agent>();
    }
}
