using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public AgentProperties properties;

    private int pathIndex = 0;
    private Dictionary<Ability.CastType, Ability> abilities;

    public Vector2Int Position { get; set; }
    public Ability CurrentAbility { get; private set; }
    public bool Busy
    {
        get
        {
            if (abilities == null || abilities.Count < 1)
                return false;
            var values = System.Enum.GetValues(typeof(Ability.CastType)) as Ability.CastType[];
            for (int i = 0; i < values.Length; ++i)
            {
                Ability a = null;
                if (abilities.TryGetValue(values[i], out a))
                {
                    if (a.Casting)
                        return true;
                }
            }
            return false;
        }
    }

    void Start()
    {
        abilities = GetAbilities();
        Ability ability = null;
        abilities.TryGetValue(Ability.CastType.Move, out ability);
        CurrentAbility = ability;
    }

    void Update()
    {
        // todo
    }

    private Dictionary<Ability.CastType, Ability> GetAbilities()
    {
        Ability[] components = GetComponents<Ability>();
        if (components.Length < 1)
        {
            Debug.LogError("No component of type {Ability} was found.");
            return null;
        }
        var abilities = new Dictionary<Ability.CastType, Ability>();
        for (int i = 0; i < components.Length; ++i)
            if (!abilities.ContainsKey(components[i].Type))
                abilities[components[i].Type] = components[i];
        return abilities;
    }

    public void SetCurrentAbility(Ability.CastType type)
    {
        if (abilities == null)
        {
            Debug.LogError("Cannot update ability, {abilities} is null.");
            return;
        }
        CurrentAbility = GetFromType(type);
    }

    private Ability GetFromType(Ability.CastType type)
    {
        if (type == Ability.CastType.None)
            return null;
        Ability ability = null;
        abilities.TryGetValue(type, out ability);
        return ability;
    }

    public void SnapTo(Vector2Int pos)
    {
        MapManager.Instance.CellAt(Position).CurrentState = Cell.State.Empty;
        Position = pos;
        transform.position = Utilities.ToWorldPosition(pos, transform);
        MapManager.Instance.CellAt(Position).CurrentState = Cell.State.Agent;
    }
}
