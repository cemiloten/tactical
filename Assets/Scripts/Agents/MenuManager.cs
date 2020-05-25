using Agents;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviourSingleton<MenuManager> {
    [SerializeField] private Text currentAgentText;

    protected override void Awake() {
        base.Awake();
        GameEvents.OnAgentStartTurn.AddListener(OnAgentStartTurn);
    }

    private void OnAgentStartTurn(Agent agent) {
        currentAgentText.text = $"Current agent: {agent.gameObject.name}";
    }
}