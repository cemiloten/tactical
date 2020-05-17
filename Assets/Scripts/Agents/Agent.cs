using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private Dictionary<AbilityType, Ability> _abilities;

    public Vector2Int Position { get; set; }
    public int Health { get; set; }

    public bool Casting => _abilities.Values.Any(ability => ability.Casting);

    private void Start()
    {
        _abilities = GetAbilities();
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

    public void Cast(AbilityType abilityType, Cell source, Cell target)
    {
        Ability ability = GetAbility(abilityType);
        if (ability == null)
            return;

        ability.Cast(source, target);
    }


    private Ability GetAbility(AbilityType type)
    {
        if (type == AbilityType.None)
            return null;

        _abilities.TryGetValue(type, out Ability ability);
        return ability;
    }
}
