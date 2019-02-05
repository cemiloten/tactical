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
    public List<GameObject> agentsPrefabs;

    private int playerCount = 2;
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
        currentSelection.text = string.Format("{0}\n {1}", Selection, Selection.Position);

        Vector2Int mousePos;
        Utilities.MousePos(out mousePos);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selection = NextAgent();
        }
    }

    public void SetCurrentAbility(Ability.CastType type)
    {
        // todo
        // if (index == Selection.CurrentAbility)
        //     index = -1;
        // Selection.SetCurrentAbility(index);
    }

    private Agent NextAgent()
    {
        nextAgentIndex = (nextAgentIndex + 1) % playerCount;
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

    private void InstantiateAgents()
    {
        agents = new List<Agent>(playerCount * agentsPrefabs.Count);
        for (int i = 0; i < playerCount * agentsPrefabs.Count; ++i)
        {
            GameObject go = Instantiate(agentsPrefabs[i % playerCount]) as GameObject;
            go.GetComponent<Renderer>().sharedMaterial = i < 2 ? red : blue;
            Agent agent = go.GetComponent<Agent>();
            agents.Add(agent);
        }
    }
}
