using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Abilities {

public class Move : Ability {
    [Tooltip("Time that it takes to move between two cells.")]
    public float movementDuration = 0.25f;

    protected override AbilityType SetType() => AbilityType.Move;

    public override void Cast(Cell source, Cell target, Action onCastEnd = null) {
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

        Vector3[] path = MakePath(cells);
        if (path == null || path.Length < 2)
            return;

        source.Type = CellType.Ground;

        target.Type = CellType.Agent;
        target.Agent = Agent;
        Agent.Position = target.Position;

        transform.DOPath(path, path.Length * movementDuration)
            .OnComplete(() => onCastEnd?.Invoke());
    }

    private Vector3[] MakePath(IReadOnlyList<Cell> cells) {
        var path = new Vector3[cells.Count];
        for (int i = 0; i < cells.Count; i++)
            path[i] = cells[i].Position.ToWorldPosition();

        return path;
    }

    private IEnumerator _Move(Vector3[] path) {
        float movementTimer = 0f;
        int pathIndex = 0;

        while (pathIndex < path.Length - 1) {
            Vector3 curr = path[pathIndex];
            Vector3 next = path[pathIndex + 1];

            while (movementTimer < movementDuration) {
                transform.position = Vector3.Lerp(curr, next, movementTimer / movementDuration);

                movementTimer += Time.deltaTime;
                yield return null;
            }

            movementTimer -= movementDuration;
            ++pathIndex;
            yield return null;
        }

        transform.position = path[path.Length - 1];
        Casting = false;
    }
}

}