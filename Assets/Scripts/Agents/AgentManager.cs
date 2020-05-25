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

        GameEvents.OnAgentEndTurn.AddListener(OnAgentEndTurn);
        GameEvents.OnAgentDead.AddListener(OnAgentDead);

        _agents.Capacity = agentCount;
        _agents.Add(player);
        for (int i = 0; i < agentCount; i++) {
            Agent agent = Instantiate(agentPrefab);
            agent.name = $"NPC {i}";

            _agents.Add(agent);
        }
    }

    private void OnAgentDead(Agent agent) {
        _agents.Remove(agent);
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
        _currentAgentIndex = (_currentAgentIndex + 1) % _agents.Count;
        CurrentAgent.StartTurn();
        turnIndicator.SetAgent(CurrentAgent.transform);
    }
}

}