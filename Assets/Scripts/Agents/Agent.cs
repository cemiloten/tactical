using System.Collections.Generic;
using System.Linq;
using Abilities;
using UnityEngine;

namespace Agents {

public abstract class Agent : MonoBehaviour {
    private Dictionary<AbilityType, Ability> Abilities;
    [SerializeField] private int actionPoints = 5;
    [SerializeField] private int health = 3;

    [SerializeField] private int startActionPoints = 5;

    public Vector2Int Position { get; set; }

    public bool Casting => Abilities.Values.Any(ability => ability.Casting);

    public int ActionPoints => actionPoints;

    private void Start() {
        Abilities = GetAbilities();
    }

    public void StartTurn() {
        Debug.Log($"My turn: {this}");

        actionPoints = startActionPoints;
        OnTurnStart();
        GameEvents.OnAgentStartTurn(this);
    }

    protected abstract void OnTurnStart();

    private Dictionary<AbilityType, Ability> GetAbilities() {
        Ability[] components = GetComponents<Ability>();

        if (components.Length < 1) {
            Debug.LogWarningFormat("No component of type [Ability] was found on {0}", this);
            return null;
        }

        var abilities = new Dictionary<AbilityType, Ability>();
        for (int i = 0; i < components.Length; ++i)
            if (!abilities.ContainsKey(components[i].Type))
                abilities[components[i].Type] = components[i];
        return abilities;
    }

    public void Cast(AbilityType abilityType, Cell source, Cell target, bool endTurn = false) {
        if (!GetAbility(abilityType, out Ability ability))
            return;

        int cost = ability.HasDynamicCost
            ? ability.CalculateDynamicCost(source, target)
            : ability.cost;

        if (actionPoints < cost) {
            Debug.LogError($"Cannot afford to cast {abilityType}.");
            return;
        }

        actionPoints -= cost;

        if (endTurn)
            ability.Cast(source, target, EndTurn);
        else
            ability.Cast(source, target);
    }

    public void EndTurn() {
        GameEvents.OnAgentEndTurn(this);
    }

    protected bool GetAbility(AbilityType type, out Ability ability) {
        ability = null;
        if (type == AbilityType.None)
            return false;

        return Abilities.TryGetValue(type, out ability);
    }

    public void LoseHealth(int amount) {
        Debug.Log($"Lost {amount} health");
        health -= amount;
        if (health <= 0)
            Die();
    }

    private void Die() {
        GameEvents.OnAgentDead(this);
    }
}

}