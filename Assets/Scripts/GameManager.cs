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
    public Button endTurnButton;
    public Material red;
    public Material blue;
    public int playerCount = 2;
    public GameObject[] agentsPrefabs;

    private int currentPlayer = 0;
    private int nextAgentIndex = 0;
    private Agent[] agents;

    public Agent Selection { get; private set; }
    public static GameManager Instance { get; private set; }
    public bool HasBusyAgent
    {
        get
        {
            if (agents == null)
                throw new NullReferenceException();
            for (int a = 0; a < agents.Length; ++a)
                if (agents[a].Busy)
                    return true;
            return false;
        }
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            if (Instance != this)
            Destroy(this);

        moveButton.onClick.AddListener(delegate { SetCurrentAbility(Ability.CastType.Move); });
        actionButton.onClick.AddListener(delegate { SetCurrentAbility(Ability.CastType.Action); });
        endTurnButton.onClick.AddListener(ButtonToEndTurn);
        InstantiateAgents();
    }

    void Start()
    {
        MapManager.Instance.PlaceAgents(agents);
        Selection = agents[nextAgentIndex];
    }

    void Update()
    {
        currentTurn.text = string.Format("Turn {0}", nextAgentIndex / agents.Length);
        if (Selection != null && Selection.CurrentAbility != null)
            currentSelection.text = string.Format("{0}\n {1}", Selection, Selection.CurrentAbility.Type);

        Vector2Int mousePos;
        Utilities.MousePos(out mousePos);

        if (Input.GetKeyDown(KeyCode.Tab))
            Selection = NextAgent();

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
        nextAgentIndex = (nextAgentIndex + 1) % agentsPrefabs.Length;
        return agents[currentPlayer * agentsPrefabs.Length + nextAgentIndex];
    }


    private void ButtonToEndTurn()
    {
        StartCoroutine(EndTurn());
    }

    private IEnumerator EndTurn()
    {
        if (HasBusyAgent)
            yield return null;

        Selection.OnEndTurn();

        MapManager.Instance.VisualPath = null;
        currentPlayer = (currentPlayer + 1) % playerCount;
        nextAgentIndex = currentPlayer * agentsPrefabs.Length;

        Selection = agents[nextAgentIndex];
        Selection.SetCurrentAbility(Ability.CastType.Move);
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

    private void InstantiateAgents()
    {
        agents = new Agent[playerCount * agentsPrefabs.Length];
        for (int i = 0; i < playerCount * agentsPrefabs.Length; ++i)
        {
            int index = i % agentsPrefabs.Length;
            GameObject go = Instantiate(agentsPrefabs[index]) as GameObject;
            string name;
            Material m;
            if (i < agentsPrefabs.Length)
            {
                m = red;
                name = "Red";
            }
            else
            {
                m = blue;
                name = "Blue";
            }
            go.GetComponent<Renderer>().sharedMaterial = m;
            go.name = string.Format("{0} - {1}", agentsPrefabs[index].name, name);
            agents[i] = go.GetComponent<Agent>();
        }
    }
}
