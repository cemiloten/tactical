using System.Collections.Generic;
using UnityEngine;

namespace Agents
{
public class AgentManager : MonoBehaviourSingleton<AgentManager>
{
    [SerializeField] private Agent agentPrefab = default;
    [SerializeField] private int agentCount = 3;

    private readonly List<Agent> _agents = new List<Agent>();

    protected override void OnAwake()
    {
        _agents.Capacity = agentCount;
        for (int i = 0; i < agentCount; i++)
        {
            _agents.Add(Instantiate(agentPrefab));
        }

        GameEvents.CellSelected.AddListener(OnCellSelected);
    }

    private void Start()
    {
        foreach (Agent agent in _agents)
        {
            // result: (hasCell, position)
            (bool, Vector2Int) result =
                MapManager.Instance.ReserveRandomCell(CellType.Agent, agent);
            if (!result.Item1)
                continue;

            agent.Position = result.Item2;
            agent.transform.position = agent.Position.ToWorldPosition();
        }
    }

    private void OnCellSelected(Cell cell)
    {
        Agent agent = _agents[0];
        agent.Cast(AbilityType.Move, MapManager.Instance.CellAt(agent.Position), cell);
    }
}
}
