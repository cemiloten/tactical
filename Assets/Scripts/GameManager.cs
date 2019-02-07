using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text currentTurn;
    public Text currentSelection;
    public Button moveButton;
    public Button actionButton;
    public Material red;
    public Material blue;
    public int playerCount = 2;
    public List<GameObject> agentsPrefabs;

    private int currentPlayer = 0;
    private int nextAgentIndex = 0;
    private List<Agent> agents;

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
        moveButton.onClick.AddListener(delegate { SetCurrentAbility(Ability.CastType.Move); });
        actionButton.onClick.AddListener(delegate { SetCurrentAbility(Ability.CastType.Action); });
        InstantiateAgents();
        MapManager.Instance.PlaceAgents(agents);
        Selection = agents[nextAgentIndex];
    }

    void Update()
    {
        currentTurn.text = string.Format("Turn {0}", nextAgentIndex / agents.Count);
        currentSelection.text = string.Format("{0}\n {1}", Selection, Selection.CurrentAbility?.Type);

        Vector2Int mousePos;
        Utilities.MousePos(out mousePos);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selection = NextAgent();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetCurrentAbility(Ability.CastType.Move);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetCurrentAbility(Ability.CastType.Action);
        }

        if (Input.GetMouseButtonDown(0)
            && MapManager.Instance.IsPositionOnMap(mousePos)
            && Selection != null
            && Selection.CurrentAbility != null
            && Selection.CurrentAbility.Type != Ability.CastType.None
            && MapManager.Instance.VisualPath.Contains(MapManager.Instance.CellAt(mousePos)))
        {
           Debug.Log("clicking");
            Selection.CurrentAbility.Cast(
                MapManager.Instance.CellAt(Selection.Position),
                MapManager.Instance.CellAt(mousePos));
        }
    }

    public void SetCurrentAbility(Ability.CastType type)
    {
        if (Selection != null
            && Selection.CurrentAbility != null
            && Selection.CurrentAbility.Type == type)
        {
            type = Ability.CastType.None;
        }
        Selection.SetCurrentAbility(type);
    }

    private Agent NextAgent()
    {
        nextAgentIndex = (nextAgentIndex + 1) % agentsPrefabs.Count;
        return agents[currentPlayer * agentsPrefabs.Count + nextAgentIndex];
    }

    public void EndTurn()
    {
        MapManager.Instance.VisualPath = null;
        currentPlayer = (currentPlayer + 1) % playerCount;
        nextAgentIndex = currentPlayer * agentsPrefabs.Count;
        Selection = agents[nextAgentIndex];
    }

    public Agent AgentAt(Cell cell)
    {
        if (cell == null)
        {
            Debug.LogError("[cell] is null");
            return null;
        }

        for (int i = 0; i < agents.Count; ++i)
        {
            if (MapManager.Instance.CellAt(cell.Position) != null
                && MapManager.Instance.CellAt(cell.Position) == cell
                && cell.Position == agents[i].Position)
            {
                return agents[i];
            }
        }
        // Debug.LogErrorFormat("No agent found at {0}", cell.Position);
        return null;
    }

    private void InstantiateAgents()
    {
        agents = new List<Agent>(playerCount * agentsPrefabs.Count);
        for (int i = 0; i < playerCount * agentsPrefabs.Count; ++i)
        {
            GameObject go = Instantiate(agentsPrefabs[i % agentsPrefabs.Count]) as GameObject;
            go.GetComponent<Renderer>().sharedMaterial = i < agentsPrefabs.Count ? red : blue;
            Agent agent = go.GetComponent<Agent>();
            agents.Add(agent);
        }
    }
}
