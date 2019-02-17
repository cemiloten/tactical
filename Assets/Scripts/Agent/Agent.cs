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
    public int team;

    public delegate void OnAgentDeadHandler(Agent agent);
    public static event OnAgentDeadHandler OnAgentDead;

    private Dictionary<AbilityType, Ability> abilities;

    public Vector2Int Position { get; set; }
    public Ability CurrentAbility { get; private set; }
    public bool Busy
    {
        get
        {
            if (abilities == null || abilities.Count < 1)
                return false;
            foreach (Ability ability in abilities.Values)
                if (ability.Casting)
                    return true;
            return false;
        }
    }

    void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnEndTurn += OnEndTurn;
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnEndTurn -= OnEndTurn;
    }

    void Start()
    {
        abilities = GetAbilities();
        if (abilities == null)
            return;
        Ability ability = null;
        abilities.TryGetValue(AbilityType.Move, out ability);
        CurrentAbility = ability;
    }

    public void OnEndTurn()
    {
        if (abilities == null)
            return;

        foreach (Ability ability in abilities.Values)
        {
            ability.Reset();
        }
    }

    public void Die()
    {
        if (OnAgentDead == null)
        {
            Debug.LogError("[OnAgentDead] is null");
            return;
        }
        OnAgentDead(this);
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

    public void SetCurrentAbility(AbilityType type)
    {
        if (abilities == null)
        {
            Debug.LogError("Cannot update ability, [abilities] is null.");
            return;
        }
        CurrentAbility = GetFromType(type);
    }

    private Ability GetFromType(AbilityType type)
    {
        if (type == AbilityType.None)
            return null;
        Ability ability = null;
        abilities.TryGetValue(type, out ability);
        return ability;
    }

    public void SnapTo(Vector2Int pos)
    {
        Cell cell = MapManager.Instance.CellAt(Position);
        if (cell != null)
            cell.State = CellState.Empty;

        Position = pos;
        transform.position = Utilities.ToWorldPosition(pos, transform);
        MapManager.Instance.CellAt(Position).State = CellState.Agent;
    }
}
