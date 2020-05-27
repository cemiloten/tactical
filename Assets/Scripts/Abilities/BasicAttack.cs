using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Abilities {

public class BasicAttack : Ability {
    [SerializeField] private float maxDuration = 2f;
    [SerializeField] private float maxJump = 5f;
    [SerializeField] private float minDuration = 0.5f;
    [SerializeField] private float minJump = 1f;
    [SerializeField] private int power = 1;

    protected override AbilityType SetType() => AbilityType.BasicAttack;

    public override void Cast(Cell source, Cell target, Action onCastEnd = null) {
        Transform attackFx = EffectsManager.Attack(source);
        attackFx.DOJump(target.Position.ToWorldPosition(),
                        Random.Range(minJump, maxJump), 1,
                        Random.Range(minDuration, maxDuration))
            .OnComplete(delegate {
                Destroy(attackFx.gameObject);
                EffectsManager.Boom(target);
                if (target.Type == CellType.Agent && target.Agent != null)
                    target.Agent.LoseHealth(power);

                onCastEnd?.Invoke();
            });
    }
}

}