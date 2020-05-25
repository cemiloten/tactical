using Abilities;
using Agents;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    private AbilityType _currentType = AbilityType.None;
    [SerializeField] private Button attack;
    [SerializeField] private Button move;
    [SerializeField] private Player player;

    private void Awake() {
        move.onClick.AddListener(OnMove);
        attack.onClick.AddListener(OnAttack);
        GameEvents.CellSelected.AddListener(OnCellSelected);
    }

    private void OnCellSelected(Cell cell) {
        if (ReferenceEquals(AgentManager.CurrentAgent, player))
            player.Cast(_currentType, MapManager.Instance.CellAt(player.Position), cell);
        else
            Debug.LogWarning("Not your turn!");
    }

    private void OnMove() {
        _currentType = AbilityType.Move;
    }

    private void OnAttack() {
        _currentType = AbilityType.BasicAttack;
    }
}