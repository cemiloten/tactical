using Abilities;
using Agents;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    [SerializeField] private Button attack;
    [SerializeField] private Button move;
    [SerializeField] private Agent player;

    private void Awake() {
        move.onClick.AddListener(OnMove);
        attack.onClick.AddListener(OnAttack);
    }

    private void OnMove() {
        player.SetCurrentAbility(AbilityType.Move);
    }

    private void OnAttack() {
        player.SetCurrentAbility(AbilityType.BasicAttack);
    }
}