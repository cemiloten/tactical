using Abilities;
using UnityEngine;

namespace Agents {
public class BasicAttack : Ability {
    [SerializeField] private int power = 1;

    protected override AbilityType SetType() {
        return AbilityType.BasicAttack;
    }

    public override bool Cast(Cell source, Cell target) {
        if (target.Type != CellType.Agent) {
            Debug.LogError("Target cell is not agent");
            return false;
        }

        EffectsManager.Boom(target);
        target.Agent.LoseHealth(power);
        return true;
    }
}
}
