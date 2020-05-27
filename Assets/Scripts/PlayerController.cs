using Abilities;
using Agents;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    private AbilityType _currentType = AbilityType.None;
    [SerializeField] private Button attack = default;
    [SerializeField] private Button move = default;
    [SerializeField] private Player player = default;

    private void Awake() {
        move.onClick.AddListener(OnMove);
        attack.onClick.AddListener(OnAttack);
    }

    private void OnEnable() {
        GameEvents.CellSelected += CellSelected;
    }

    private void OnDisable() {
        GameEvents.CellSelected -= CellSelected;
    }

    private void CellSelected(Cell cell) {
        if (!ReferenceEquals(AgentManager.CurrentAgent, player)) {
            Debug.LogWarning("Not your turn!");
            return;
        }

        player.Cast(_currentType, MapManager.Instance.CellAt(player.Position), cell);
    }

    private void OnMove() {
        _currentType = AbilityType.Move;
    }

    private void OnAttack() {
        _currentType = AbilityType.BasicAttack;
    }
}
