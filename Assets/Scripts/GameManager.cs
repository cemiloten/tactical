using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> agentsPrefabs;
    private List<Agent> agents;

    private Agent _selection;
    public Agent Selection
    {
        get { return _selection; }
        private set
        {
            _selection = value ?? null;
            MapManager.Instance.CurrentAgent = _selection;
        }
    }


    void Start()
    {
        agents = new List<Agent>(agentsPrefabs.Count);
        CreateAgents();
        MapManager.Instance.PlaceAgents(agents);
        Selection = null;
    }

    void Update()
    {
        if (!Camera.main)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Selection = null;
        }

        Vector2Int mousePos;
        Utilities.MousePos(out mousePos);

        if (Input.GetMouseButtonDown(0))
        {
            if (MapManager.Instance.IsPositionOnMap(mousePos)
                && SelectionIsEmpty())
            {
                Selection = AgentAt(mousePos);
            }
        }

        if (!SelectionIsEmpty())
        {
            if (MapManager.Instance.IsPositionOnMap(mousePos)
                && mousePos != Selection.Position)
            {

                if (!Selection.IsMoving)
                    MapManager.Instance.VisualPath = AStar.FindPath(
                        MapManager.Instance.CellAt(Selection.Position),
                        MapManager.Instance.CellAt(mousePos));

                if (Input.GetMouseButtonDown(0))
                {
                    Selection.MoveTo(mousePos);
                    MapManager.Instance.VisualPath = null;
                }
            }
        }
    }

    private void CreateAgents()
    {
        for (int i = 0; i < agentsPrefabs.Count; ++i)
        {
            GameObject go = Instantiate(agentsPrefabs[i]) as GameObject;
            Agent agent = go.GetComponent<Agent>();
            if (agent == null)
            {
                agent = go.AddComponent<Agent>();
            }
            agents.Add(agent);
        }
    }

    private Agent AgentAt(Vector2Int pos)
    {
        for (int i = 0; i < agents.Count; ++i)
        {
            if (agents[i].Position == pos)
            {
                return agents[i];
            }
        }
        return null;
    }

    private bool SelectionIsEmpty()
    {
        return Selection == null;
    }
}
