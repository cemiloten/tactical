using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AgentType
{
    None,
    Pusher,
    Puller,
    Swapper,
    Heart
}

public class Agent : MonoBehaviour
{
    public AgentType type;

    private Dictionary<AbilityType, Ability> _abilities;

    public Vector2Int Position { get; set; }
    public Ability CurrentAbility { get; private set; }
    public bool Busy
    {
        get
        {
            if (_abilities == null || _abilities.Count < 1)
                return false;
            foreach (Ability ability in _abilities.Values)
                if (ability.Casting)
                    return true;
            return false;
        }
    }

    private void Awake()
    {
        GameEvents.TurnEnd.AddListener(OnTurnEnd);
    }

    void Start()
    {
        _abilities = GetAbilities();
        if (_abilities == null)
            return;
        Ability ability = null;
        _abilities.TryGetValue(AbilityType.Move, out ability);
        CurrentAbility = ability;
    }

    public void Cast(AbilityType abilityType, Cell source, Cell target)
    {
        var a = GetAbility(abilityType);
        if (a == null)
            return;

        a.Cast(source, target);
    }

    public void OnTurnEnd()
    {
        if (_abilities == null)
            return;

        foreach (Ability ability in _abilities.Values)
        {
            ability.Reset();
        }
    }

    public void Die()
    {
        GameEvents.AgentDead.Invoke(this);
    }

    private Dictionary<AbilityType, Ability> GetAbilities()
    {
        Ability[] components = GetComponents<Ability>();
        if (components.Length < 1)
        {
            Debug.LogWarningFormat("No component of type [Ability] was found on {0}", this);
            return null;
        }
        var abilities = new Dictionary<AbilityType, Ability>();
        for (int i = 0; i < components.Length; ++i)
            if (!abilities.ContainsKey(components[i].Type))
                abilities[components[i].Type] = components[i];
        return abilities;
    }

    private Ability GetAbility(AbilityType type)
    {
        if (type == AbilityType.None)
            return null;
        Ability ability = null;
        _abilities.TryGetValue(type, out ability);
        return ability;
    }
}
