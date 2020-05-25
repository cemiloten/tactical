using System.Collections.Generic;
using System.Linq;
using Abilities;
using UnityEngine;

namespace Agents {

public abstract class Agent : MonoBehaviour {
    protected Dictionary<AbilityType, Ability> Abilities;
    [SerializeField] private int health = 3;

    public Vector2Int Position { get; set; }

    public bool Casting => Abilities.Values.Any(ability => ability.Casting);

    private void Start() {
        Abilities = GetAbilities();
    }

    public void StartTurn() {
        Debug.Log($"My turn: {this}");
        OnTurnStart();
        GameEvents.OnAgentStartTurn.Invoke(this);
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

    public void Cast(AbilityType abilityType, Cell source, Cell target) {
        if (!GetAbility(abilityType, out Ability ability))
            return;

        ability.Cast(source, target, EndTurn);
    }

    protected void EndTurn() {
        GameEvents.OnAgentEndTurn.Invoke(this);
    }

    protected bool GetAbility(AbilityType type, out Ability ability) {
        ability = null;
        if (type == AbilityType.None)
            return false;

        return Abilities.TryGetValue(type, out ability);
    }

    public void LoseHealth(int power) {
        Debug.Log($"Lost {power} health");
        health -= power;
        if (health <= 0)
            Die();
    }

    private void Die() {
        GameEvents.OnAgentDead.Invoke(this);
        Destroy(gameObject);
    }
}

}