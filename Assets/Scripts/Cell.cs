using Agents;
using Lean.Touch;
using UnityEngine;

public enum CellType {
    Ground,
    Agent,
}

public class Cell : MonoBehaviour {
    private LeanSelectable _leanSelectable;

    public Vector2Int Position { get; private set; }
    public CellType Type { get; set; }
    public Agent Agent { get; set; }

    public bool Walkable => Type == CellType.Ground;

    private void Awake() {
        _leanSelectable = GetComponent<LeanSelectable>();
        _leanSelectable.OnSelect.AddListener(OnSelect);
    }

    private void OnSelect(LeanFinger finger) {
        GameEvents.CellSelected.Invoke(this);
    }

    public void Initialize(Vector2Int pos, CellType type = CellType.Ground) {
        Position = pos;
        Type = type;
        transform.position = pos.ToWorldPosition();
    }
}
