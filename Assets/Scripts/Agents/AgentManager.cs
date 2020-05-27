using System.Collections.Generic;
using UnityEngine;

namespace Agents {

public class AgentManager : MonoBehaviourSingleton<AgentManager> {
    private readonly List<Agent> _agents = new List<Agent>();
    private int _currentAgentIndex;

    [SerializeField] private int agentCount = 3;
    [SerializeField] private Agent agentPrefab = default;
    [SerializeField] private Player player = default;
    [SerializeField] private TurnIndicator turnIndicator = default;

    public static Agent CurrentAgent => Instance._agents[Instance._currentAgentIndex];
    public static Vector2Int PlayerPosition => Instance.player.Position;

    protected override void Awake() {
        base.Awake();

        _agents.Capacity = agentCount;
        _agents.Add(player);
        for (int i = 0; i < agentCount; i++) {
            Agent agent = Instantiate(agentPrefab);
            agent.name = $"NPC {i}";

            _agents.Add(agent);
        }
    }

    private void OnEnable() {
        GameEvents.AgentEndTurn += OnAgentEndTurn;
        GameEvents.AgentDead += OnAgentDead;
    }

    private void OnDisable() {
        GameEvents.AgentEndTurn -= OnAgentEndTurn;
        GameEvents.AgentDead -= OnAgentDead;
    }

    private void OnAgentDead(Agent agent) {
        if (_agents == null || _agents.Count < 1)
            return;

        if (agent.CompareTag("Player")) {
            // Lose
            DestroyAgents();
            GameEvents.OnDefeat();
            return;
        }

        _agents.Remove(agent);
        Destroy(agent.gameObject);

        if (_agents.Count > 1)
            return;

        // Win
        GameEvents.OnVictory();
    }

    private void DestroyAgents() {
        int c = _agents.Count;
        for (int i = c - 1; i >= 0; i--) {
            Agent a = _agents[i];
            Destroy(a.gameObject);
            _agents.Remove(a);
        }
    }

    private void OnAgentEndTurn(Agent agent) {
        NextAgent();
    }

    private void Start() {
        foreach (Agent agent in _agents) {
            // result: (hasCell, position)
            (bool, Vector2Int) result =
                MapManager.Instance.ReserveRandomCell(CellType.Agent, agent);
            if (!result.Item1)
                continue;

            agent.Position = result.Item2;
            agent.transform.position = agent.Position.ToWorldPosition();
        }

        player.transform.position = Vector2Int.zero.ToWorldPosition();
        player.Position = Vector2Int.zero;

        _currentAgentIndex = 0;
        CurrentAgent.StartTurn();
        turnIndicator.SetAgent(CurrentAgent.transform);
    }

    private void NextAgent() {
        if (_agents == null || _agents.Count == 0)
            return;

        _currentAgentIndex = (_currentAgentIndex + 1) % _agents.Count;
        CurrentAgent.StartTurn();
        turnIndicator.SetAgent(CurrentAgent.transform);
    }
}

}
