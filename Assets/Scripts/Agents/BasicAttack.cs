using System;
using Abilities;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Agents {

public class BasicAttack : Ability {
    [SerializeField] private float maxDuration = 2f;
    [SerializeField] private float maxJump = 5f;
    [SerializeField] private float minDuration = 0.5f;
    [SerializeField] private float minJump = 1f;
    [SerializeField] private int power = 1;

    protected override AbilityType SetType() {
        return AbilityType.BasicAttack;
    }

    public override void Cast(Cell source, Cell target, Action onCastEnd = null) {
        Transform a = EffectsManager.Attack(source);
        a.DOJump(target.Position.ToWorldPosition(), Random.Range(minJump, maxJump), 1,
                 Random.Range(minDuration, maxDuration)).OnComplete(
            delegate {
                Destroy(a.gameObject);
                EffectsManager.Boom(target);
                onCastEnd?.Invoke();
            });

        if (target.Type == CellType.Agent && target.Agent != null)
            target.Agent.LoseHealth(power);
    }
}

}