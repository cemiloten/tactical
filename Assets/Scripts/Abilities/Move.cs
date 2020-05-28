using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Abilities {

public class Move : Ability {
    [Tooltip("Time that it takes to move between two cells.")]
    public float movementDuration = 0.25f;

    protected override AbilityType SetType() => AbilityType.Move;
    protected override bool SetHasDynamicCost() => true;

    public override int CalculateDynamicCost(Cell source, Cell target) =>
        Utilities.Distance(source, target);

    public override void Cast(Cell source, Cell target, Action onCastEnd = null,
                              bool endTurn = false) {
        if (Casting) {
            Debug.LogWarning("Cannot accept new move while moving");
            return;
        }

        if (!target.Walkable) {
            Debug.LogError("Target cell is not Walkable");
            return;
        }

        if (!PathMaker.AStar(source, target, out List<Cell> cells))
            return;

        if (cells == null || cells.Count < 1)
            return;

        Vector3[] path = MakePath(cells);
        transform.DOPath(path, path.Length * movementDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(delegate {
                source.Type = CellType.Ground;
                target.Type = CellType.Agent;
                target.Agent = Agent;
                Agent.Position = target.Position;

                onCastEnd?.Invoke();
            });
    }


    private Vector3[] MakePath(IReadOnlyList<Cell> cells) {
        var path = new Vector3[cells.Count];
        for (int i = 0; i < cells.Count; i++)
            path[i] = cells[i].Position.ToWorldPosition();

        return path;
    }
}

}
