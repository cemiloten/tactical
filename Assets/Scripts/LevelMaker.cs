using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    private List<Agent> agents = new List<Agent>();
    private List<Vector2Int> holes = new List<Vector2Int>();
    private CellState currentState = CellState.Empty;
    private AgentType currentAgentType = AgentType.None;
    private int currentTeam = 0;


    private void Start()
    {
        emptyButton.onClick.AddListener(delegate { SetCurrentType(CellState.Empty, AgentType.None); });
        holeButton.onClick.AddListener(delegate { SetCurrentType(CellState.Hole, AgentType.None); });
        heartButton.onClick.AddListener(delegate { SetCurrentType(CellState.Agent, AgentType.Heart); });
        pusherButton.onClick.AddListener(delegate { SetCurrentType(CellState.Agent, AgentType.Pusher); });
        pullerButton.onClick.AddListener(delegate { SetCurrentType(CellState.Agent, AgentType.Puller); });
        swapperButton.onClick.AddListener(delegate { SetCurrentType(CellState.Agent, AgentType.Swapper); });

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
        Level level = ScriptableObject.CreateInstance(typeof(Level)) as Level;
        level.width = width;
        level.height = height;
        level.cells = MapManager.Instance.Cells;
        level.agents = agents;
        level.holes = holes;
        AssetDatabase.CreateAsset(level, "Assets/Levels/Level.asset");
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

        if(Input.GetKeyDown(KeyCode.Space))
            WriteToDisk();
    }

    private void SetCellContent(Vector2Int position)
    {
        Cell cell = MapManager.Instance.CellAt(position);
        if (cell == null)
        {
            Debug.LogError("Cannot update null cell");
            return;
        }

        UpdateCell(cell);
    }

    private void UpdateCell(Cell cell)
    {
        if (currentState == CellState.Obstacle)
            throw new NotImplementedException();

        if (cell.State == CellState.Empty)
        {
            if (currentState == CellState.Empty)
                return;
            if (currentState == CellState.Hole)
            {
                holes.Add(cell.Position);
            }
            else if (currentState == CellState.Agent)
            {
                AddAgent(cell);
            }
        }
        else if (cell.State == CellState.Hole)
        {
            if (currentState == CellState.Hole)
                return;

            holes.RemoveAll(vec2 => vec2 == cell.Position);
            if (currentState == CellState.Agent)
            {
                AddAgent(cell);
            }
        }
        else if (cell.State == CellState.Agent)
        {
            Agent agent = AgentAt(cell.Position);
            if (agent == null)
            {
                Debug.LogErrorFormat("Didn't find agent at {0}", cell.Position);
                return;
            }

            if (currentState == CellState.Agent && currentAgentType == agent.type)
            {
                return;
            }

            Destroy(agent.gameObject);
            agents.Remove(agent);
            if (currentState == CellState.Agent)
            {
                AddAgent(cell);
            }
        }

        cell.State = currentState;
    }

private void AddAgent(Cell cell)
{
    GameObject prefab = CurrentAgentTypeToPrefab();
    if (prefab == null)
    {
        Debug.LogErrorFormat("Error finding prefab from type {0}", currentState);
        return;
    }
    GameObject go = Instantiate(
        prefab,
        Utilities.ToWorldPosition(cell.Position, prefab.transform),
        Quaternion.identity);
    Agent agent = go.GetComponent<Agent>();
    agent.team = currentTeam;
    agent.type = currentAgentType;
    agent.Position = cell.Position;
    agents.Add(agent);
}


private GameObject CurrentAgentTypeToPrefab()
{
    switch (currentAgentType)
    {
        case AgentType.Heart   : return heartPrefab;
        case AgentType.Pusher  : return pusherPrefab;
        case AgentType.Puller  : return pullerPrefab;
        case AgentType.Swapper : return swapperPrefab;
        default: return null;
    }
}

private Agent AgentAt(Vector2Int position)
{
    return agents.Find(a => a.Position == position);
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
    currentState = state;
    currentAgentType = type;
}
}