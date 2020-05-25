using System.Collections;
using Abilities;
using UnityEngine;

namespace Agents {

public class NPC : Agent {
    // todo: Code AI behaviour
    // if player is in range of attack, attack
    // else move towards player

    protected override void OnTurnStart() {
        if (!GetAbility(AbilityType.BasicAttack, out Ability ability)) {
            Debug.LogError("Not found BasicAttack ability");
            StartCoroutine(_EndTurn(1f));
            return;
        }

        Cell source = MapManager.Instance.CellAt(Position);
        Cell target = MapManager.Instance.CellAt(AgentManager.PlayerPosition);
        if (!ability.IsInRange(source, target)) {
            StartCoroutine(_EndTurn(1f));
            return;
        }

        ability.Cast(source, target, EndTurn);
    }

    private IEnumerator _EndTurn(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        EndTurn();
    }
}

}