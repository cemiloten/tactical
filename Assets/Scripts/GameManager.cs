using System;
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
    public Button nextAgentButton;
    public Button endTurnButton;
    public Material red;
    public Material blue;
    public int playerCount = 2;
    public MapLayout mapLayout;

    [Serializable]
    public struct AgentWithType
    {
        public Agent.Type type;
        public GameObject prefab;
    }
    public AgentWithType[] agentsWithTypes;

    public delegate void OnEndTurnHandler();
    public event OnEndTurnHandler OnEndTurn;

    private int currentPlayer = 0;
    private int nextAgentIndex = 0;
    private Agent[] agents;
    private Agent[] deadAgents;

    public Agent Selection { get; private set; }
    public static GameManager Instance { get; private set; }
    public bool HasBusyAgent
    {
        get
        {
            if (agents == null)
                throw new NullReferenceException();
            for (int a = 0; a < agents.Length; ++a)
                if (agents[a] != null && agents[a].Busy)
                    return true;
            return false;
        }
    }

    void OnEnable()
    {
        Agent.OnAgentDead += OnAgentDead;
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);

        moveButton.onClick.AddListener(delegate { SetCurrentAbility(Ability.CastType.Move); });
        actionButton.onClick.AddListener(delegate { SetCurrentAbility(Ability.CastType.Action); });
        endTurnButton.onClick.AddListener(ButtonToEndTurn);
        nextAgentButton.onClick.AddListener(SelectNextAgent);

        deadAgents = new Agent[mapLayout.agents.Length];
        InstantiateAgentsFromLayout();
    }

    void Start()
    {
        MapManager.Instance.Initialize(mapLayout);
        MapManager.Instance.PlaceAgentsFromLayout(mapLayout, agents);

SelectNextAgent();
    }

    void Update()
    {
        currentTurn.text = string.Format("Turn {0}", 0);
        if (Selection != null && Selection.CurrentAbility != null)
            currentSelection.text = string.Format("{0}\n {1}", Selection, Selection.CurrentAbility.Type);
        if (Selection.CurrentAbility == null)
        {
            moveButton.GetComponent<Image>().color = Color.white;
            actionButton.GetComponent<Image>().color = Color.white;
        }
        else
        {
            if (Selection.CurrentAbility.Type == Ability.CastType.Move)
            {
                moveButton.GetComponent<Image>().color = Color.green;
                actionButton.GetComponent<Image>().color = Color.white;
            }
            else if (Selection.CurrentAbility.Type == Ability.CastType.Action)
            {
                moveButton.GetComponent<Image>().color = Color.white;
                actionButton.GetComponent<Image>().color = Color.magenta;
            }
        }

        Vector2Int mousePos;
        Utilities.MousePos(out mousePos);

        if (Input.GetKeyDown(KeyCode.Tab))
            SelectNextAgent();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetCurrentAbility(Ability.CastType.Move);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SetCurrentAbility(Ability.CastType.Action);

        if (Input.GetMouseButtonDown(0)
            && MapManager.Instance.IsPositionOnMap(mousePos)
            && Selection != null
            && Selection.CurrentAbility != null
            && Selection.CurrentAbility.Type != Ability.CastType.None)
        {
            if (MapManager.Instance.VisualPath == null)
                Debug.Log("visual path null");
            else if (MapManager.Instance.VisualPath.Contains(MapManager.Instance.CellAt(mousePos)))
            {
                bool cast = Selection.CurrentAbility.Cast(
                    MapManager.Instance.CellAt(Selection.Position),
                    MapManager.Instance.CellAt(mousePos));
                if (Selection.CurrentAbility.Type == Ability.CastType.Action && cast)
                {
                    StartCoroutine(EndTurn());
                }
            }
        }
    }

    void OnAgentDead(Agent agent)
    {
        Debug.LogFormat("Receiving agent dead callback from {0}", agent);
        MapManager.Instance.CellAt(agent.Position).CurrentState = Cell.State.Hole;
        agent.gameObject.SetActive(false);
        int index = 0;
        for (int i = 0; i < agents.Length; ++i)
        {
            if (agents[i] != null && agents[i] == agent)
            {
                index = i;
                agents[i] = null;
                break;
            }
        }
        deadAgents[index] = agent;
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

    public void SelectNextAgent()
    {
        // todo: refactor
        // todo: if only heart is left, infinite loop
        int nextPlayerIndex = currentPlayer * (agents.Length / 2);
        nextAgentIndex = (nextAgentIndex + 1) % (agents.Length / 2);
        while (agents[nextPlayerIndex + nextAgentIndex] == null
            || agents[nextPlayerIndex + nextAgentIndex].type == Agent.Type.Heart)
        {
            nextAgentIndex = (nextAgentIndex + 1) % (agents.Length / 2);
        }
        Selection = agents[nextPlayerIndex + nextAgentIndex];
    }

    private void ButtonToEndTurn()
    {
        StartCoroutine(EndTurn());
    }

    private IEnumerator EndTurn()
    {
        // Don't execute cleanup code if agents are casting/moving
        while (HasBusyAgent)
            yield return null;

        MapManager.Instance.VisualPath = null;
        currentPlayer = (currentPlayer + 1) % playerCount;
        SelectNextAgent();
        Selection.SetCurrentAbility(Ability.CastType.Move);

        OnEndTurn?.Invoke();
    }

    public Agent AgentAt(Cell cell)
    {
        if (cell == null)
        {
            Debug.LogError("[cell] is null");
            return null;
        }

        for (int i = 0; i < agents.Length; ++i)
        {
            if (agents[i] == null)
                continue;

            if (MapManager.Instance.CellAt(cell.Position) != null
                && MapManager.Instance.CellAt(cell.Position) == cell
                && cell.Position == agents[i].Position)
            {
                return agents[i];
            }
        }
        Debug.LogErrorFormat("No agent found at {0}", cell.Position);
        return null;
    }

    private GameObject TypeToPrefab(Agent.Type type)
    {
        for (int i = 0; i < agentsWithTypes.Length; ++i)
        {
            if (agentsWithTypes[i].type == type)
            {
                return agentsWithTypes[i].prefab;
            }
        }
        Debug.LogErrorFormat("Didn't find prefab of type {0}", type);
        return null;
    }

    private void InstantiateAgentsFromLayout()
    {
        agents = new Agent[mapLayout.agents.Length];
        for (int i = 0; i < agents.Length; ++i)
        {
            GameObject prefab = TypeToPrefab(mapLayout.agents[i].type);
            if (prefab == null)
            {
                Debug.LogErrorFormat("Didn't find prefab of type {0}", mapLayout.agents[i].type);
                return;
            }

            GameObject go = Instantiate(prefab);
            go.GetComponent<Renderer>().sharedMaterial = i < agents.Length / 2 ? blue : red;
            agents[i] = go.GetComponent<Agent>();
        }
    }
}