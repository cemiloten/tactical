using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text currentTurn;
    public Text currentSelection;
    public List<GameObject> agentsPrefabs;

    private List<Agent> agents;
    private int nextAgentIndex = 0;

    public Agent Selection { get; private set; }
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            if (Instance != this)
            Destroy(this);
    }

    void Start()
    {
        InstantiateAgents();
        MapManager.Instance.PlaceAgents(agents);
        Selection = agents[nextAgentIndex];
    }

    void Update()
    {
        currentTurn.text = string.Format("Turn {0}", nextAgentIndex / agents.Count);
        currentSelection.text = string.Format("{0}\n {1}", Selection, Selection.Position);
        currentSelection.text = Selection.CurrentAbility.ToString();

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

        if (Selection.IsMoving
            || !MapManager.Instance.IsPositionOnMap(mousePos)
            || mousePos == Selection.Position
            || Selection.CurrentAbility >= 0)
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

    public Agent AgentAt(Cell cell)
    {
        for (int i = 0; i < agents.Count; ++i)
        {
            if (MapManager.Instance.CellAt(cell.Position) == cell
                && cell.Position == agents[i].Position)
            {
                return agents[i];
            }
        }
        return null;
    }

    private Agent NextAgent()
    {
        ++nextAgentIndex;
        return agents[nextAgentIndex % agents.Count];
    }

    public void SetCurrentAbility(int index)
    {
        if (index < 0 || index > Selection.Abilities.Length - 1)
            return;

        Selection.CurrentAbility = Selection.CurrentAbility == index ? -1 : index;
    }

    public void EndTurn()
    {
        Selection = NextAgent();
        MapManager.Instance.VisualPath = null;
    }

    private void InstantiateAgents()
    {
        agents = new List<Agent>(agentsPrefabs.Count);
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
