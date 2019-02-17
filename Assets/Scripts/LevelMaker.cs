using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMaker : MonoBehaviour
{
    public GameObject colliderPrefab;
    public GameObject cellPrefab;
    public GameObject heartPrefab;
    public GameObject pusherPrefab;
    public GameObject pullerPrefab;
    public GameObject swapperPrefab;
    public Text widthText;
    public Text heightText;
    public Text teamText;
    public Slider widthSlider;
    public Slider heightSlider;
    public Slider teamSlider;
    public Button emptyButton;
    public Button holeButton;
    public Button heartButton;
    public Button pusherButton;
    public Button pullerButton;
    public Button swapperButton;

    private int width;
    private int height;
    private Agent[] agents = new Agent[Globals.maxAgentCount];
    private List<Vector2Int> holes = new List<Vector2Int>();
    private CellState currentSelection = CellState.Empty;
    private AgentType currentAgentType = AgentType.None;
    private int currentTeam = 0;


    private void Start()
    {
        emptyButton.onClick.AddListener   (delegate { SetCurrentType(CellState.Empty, AgentType.None    ); });
        holeButton.onClick.AddListener    (delegate { SetCurrentType(CellState.Hole,  AgentType.None    ); });
        heartButton.onClick.AddListener   (delegate { SetCurrentType(CellState.Agent, AgentType.Heart   ); });
        pusherButton.onClick.AddListener  (delegate { SetCurrentType(CellState.Agent, AgentType.Pusher  ); });
        pullerButton.onClick.AddListener  (delegate { SetCurrentType(CellState.Agent, AgentType.Puller  ); });
        swapperButton.onClick.AddListener (delegate { SetCurrentType(CellState.Agent, AgentType.Swapper ); });

        SetWidthFromSlider();
        SetHeightFromSlider();
        SetCurrentTeam();

        Camera.main.transform.eulerAngles = new Vector3(90f, 0f, 0f);
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        MapManager.Instance.width = width;
        MapManager.Instance.height = height;
        MapManager.Instance.Cleanup();
        MapManager.Instance.SetupColliderPlane();
        MapManager.Instance.InstantiateEmptyCells();
        PlaceCamera();
    }

    public void WriteToDisk()
    {
        Level level = new Level();
        level.width = width;
        level.height = height;
        level.cells = MapManager.Instance.Cells;
        level.agents = agents;
    }

    private void Update()
    {
        Vector2Int mousePos;
        Utilities.MousePositionOnMap(out mousePos);

        if (Input.GetMouseButtonDown(0)
            && MapManager.Instance.IsPositionOnMap(mousePos))
        {
            SetCellContent(mousePos);
        }
    }

    private void SetCellContent(Vector2Int position)
    {
        if (!MapManager.Instance.IsPositionOnMap(position))
        {
            Debug.LogErrorFormat("[position]: {0} is not valid", position);
            return;
        }

        Cell cell = MapManager.Instance.CellAt(position);
        if (cell == null)
        {
            Debug.LogError("Cannot update null cell");
            return;
        }

        UpdateCell(cell, SelectionTypeToState());
    }

    private void UpdateCell(Cell cell, CellState newState)
    {
        // cell.State = newState;
        if (cell.State == CellState.Hole && newState != CellState.Hole)
        {
            // remove hole from list
        }

        Agent agent = AgentAt(cell.Position);
        if (agent != null)
        {
            Destroy(agent.gameObject);
            int index = Array.IndexOf(agents, agent);
            if (index < 0)
            {
                Debug.LogErrorFormat("Didn't find agent '{0}' in array", agent);
                return;
            }
            agents[index] = null;
        }

        if (newState == CellState.Empty || newState == CellState.Hole)
        {
            return;
        }

        GameObject prefab = SelectionTypeToPrefab(currentSelection);
        if (prefab == null)
        {
            Debug.LogErrorFormat("Error finding prefab from type {0}", currentSelection);
            return;
        }

        GameObject go = Instantiate(
            prefab,
            Utilities.ToWorldPosition(cell.Position, prefab.transform),
            Quaternion.identity);
        agent = go.GetComponent<Agent>();
        agent.Position = cell.Position;
        AddAgentToArray(agent);
    }

    private void AddAgentToArray(Agent agent)
    {
        for (int i = 0; i < agents.Length / 2; ++i)
        {
            int index = i + currentTeam * agents.Length / 2;
            if (agents[index] == null)
            {
                agents[index] = agent;
                return;
            }
        }
        Debug.LogError("Couldn't add agent to array, no free slot left");
    }

    private Agent AgentAt(Vector2Int position)
    {
        if (!MapManager.Instance.IsPositionOnMap(position))
        {
            return null;
        }
        return Array.Find(agents, a => a.Position == position);
    }

    private GameObject AgentTypeToPrefab(AgentType type)
    {
        switch (type)
        {
            case AgentType.Heart: return heartPrefab;
            case AgentType.Pusher: return pusherPrefab;
            case AgentType.Puller: return pullerPrefab;
            case AgentType.Swapper: return swapperPrefab;
            default: return null;
        }
    }

    private void PlaceCamera()
    {
        Camera.main.transform.position = new Vector3(
            width / 2f,
            Mathf.Max(width, height),
            height / 2f);
    }

    public void SetCurrentTeam()
    {
        currentTeam = (int)teamSlider.value;
        teamText.text = currentTeam.ToString();
    }

    public void SetWidthFromSlider()
    {
        width = (int)widthSlider.value;
        widthText.text = string.Format("Width: {0}", width);
    }

    public void SetHeightFromSlider()
    {
        height = (int)heightSlider.value;
        heightText.text = string.Format("Height: {0}", height);
    }

    public void SetCurrentType(CellState state, AgentType type)
    {
        currentSelection = state;
        currentAgentType = type;
    }
}