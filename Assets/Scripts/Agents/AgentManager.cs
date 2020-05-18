using System.Collections.Generic;
using UnityEngine;

namespace Agents {
public class AgentManager : MonoBehaviourSingleton<AgentManager> {
    private readonly List<Agent> _agents = new List<Agent>();

    private int _currentAgentIndex;
    [SerializeField] private int agentCount = 3;
    [SerializeField] private Agent agentPrefab = default;
    private Agent CurrentAgent => _agents[_currentAgentIndex];

    protected override void OnAwake() {
        _agents.Capacity = agentCount;
        for (int i = 0; i < agentCount; i++)
            _agents.Add(Instantiate(agentPrefab));

        GameEvents.CellSelected.AddListener(OnCellSelected);
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

        _currentAgentIndex = 0;
    }

    private void NextAgent() {
        _currentAgentIndex = (_currentAgentIndex + 1) % _agents.Count;
    }

    private void OnCellSelected(Cell cell) {
        Agent curr = CurrentAgent;
        curr.CastCurrentAbility(MapManager.Instance.CellAt(curr.Position), cell);
        // NextAgent();
    }
}
}
