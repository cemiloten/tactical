using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text currentTurn;
    public Text currentSelection;
    public Button endTurn;
    public GameObject playerPrefab;
    public GameObject currentAgentCircle;
    public List<GameObject> agentsPrefabs;

    private Agent player;
    private List<Agent> agents;
    private int nextAgentIndex = 0;

    public Agent Selection { get; private set; }


    void Start()
    {
        endTurn.onClick.AddListener(EndTurn);

        InstantiateAgents();
        MapManager.Instance.PlaceAgents(agents);
        Selection = player;

        // Player always starts first
        for (int i = 0; i < agents.Count; ++i)
        {
            if (agents[i] == player)
            {
                nextAgentIndex = i;
                break;
            }
        }
    }

    void Update()
    {
        currentTurn.text = string.Format("Turn {0}", nextAgentIndex / agents.Count);
        currentSelection.text = string.Format("{0}\n {1}", Selection, Selection.Position);

        if (Selection != player)
        {
            // Simple AI that goes towards player
            if (!Selection.IsMoving)
            {
                List<Cell> path = AStar.FindPathToNearestNeighbour(
                    MapManager.Instance.CellAt(Selection.Position),
                    MapManager.Instance.CellAt(player.Position));
                
                if (path != null)
                    Selection.Move(path);
                
                // todo: make sure ai agent ends its turn
                // EndTurn();
            }
            return;
        }

        Vector2Int mousePos;
        if (!Utilities.MousePos(out mousePos))
            return;

        if (Input.GetMouseButtonDown(0) && MapManager.Instance.IsPositionOnMap(mousePos))
        {
            if (MapManager.Instance.VisualPath != null
                && mousePos == MapManager.Instance.VisualPath[MapManager.Instance.VisualPath.Count - 1].Position)
            {
                Selection.Move(MapManager.Instance.VisualPath);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Pressed tab, ending turn");
            EndTurn();
        }

        if (Selection.IsMoving
            || !MapManager.Instance.IsPositionOnMap(mousePos)
            || mousePos == Selection.Position)
        {
            MapManager.Instance.VisualPath = null;
        }
        else
        {
            List<Cell> path = AStar.FindPath(
                MapManager.Instance.CellAt(Selection.Position),
                MapManager.Instance.CellAt(mousePos));
            if (path != null && path.Count <= Selection.MovementPoints)
                MapManager.Instance.VisualPath = path;
        }
    }

    private Agent NextAgent()
    {
        ++nextAgentIndex;
        return agents[nextAgentIndex % agents.Count];
    }

    private void EndTurn()
    {
        Selection = NextAgent();
        MapManager.Instance.VisualPath = null;
    }

    private Agent InstantiatePlayer()
    {
        GameObject go = Instantiate(playerPrefab) as GameObject;
        Agent player = go.GetComponent<Agent>();
        if (player == null)
            player = go.AddComponent<Agent>();
        return player;
    }

    private void InstantiateAgents()
    {
        agents = new List<Agent>(agentsPrefabs.Count);
        player = InstantiatePlayer();
        agents.Add(player);

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
}
