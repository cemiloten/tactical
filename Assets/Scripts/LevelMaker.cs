using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMaker : MonoBehaviour
{
    public enum SelectionType
    {
        Empty,
        Hole,
        Pusher,
        Puller,
        Swapper,
        Heart
    }

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
    private SelectionType currentType = SelectionType.Empty;
    private int currentTeam = 0;


    private void Start()
    {
        emptyButton.onClick.AddListener   (delegate { SetCurrentType(SelectionType.Empty   ); });
        holeButton.onClick.AddListener    (delegate { SetCurrentType(SelectionType.Hole    ); });
        heartButton.onClick.AddListener   (delegate { SetCurrentType(SelectionType.Heart   ); });
        pusherButton.onClick.AddListener  (delegate { SetCurrentType(SelectionType.Pusher  ); });
        pullerButton.onClick.AddListener  (delegate { SetCurrentType(SelectionType.Puller  ); });
        swapperButton.onClick.AddListener (delegate { SetCurrentType(SelectionType.Swapper ); });

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
        MapManager.Instance.InstantiateCells();
        PlaceCamera();
    }

    private void Update()
    {
        Vector2Int mousePos;
        Utilities.MousePositionOnMap(out mousePos);

        if (Input.GetMouseButtonDown(0)
            && MapManager.Instance.IsPositionOnMap(mousePos))
        {
            Debug.Log(mousePos);
            SetCellContent(mousePos);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("debug");
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
            Debug.LogError("[cell] is null");
            return;
        }

        UpdateCell(cell, CurrentTypeToState());
    }

    private CellState CurrentTypeToState()
    {
        switch (currentType)
        {
            case SelectionType.Heart:
            case SelectionType.Pusher:
            case SelectionType.Puller:
            case SelectionType.Swapper:
                return CellState.Agent;
            case SelectionType.Hole:
                return CellState.Hole;
            case SelectionType.Empty:
                return CellState.Empty;
            default:
                Debug.LogErrorFormat(
                    "Didn't find appropriate State for type '{0}', returning Empty",
                    currentType);
                return CellState.Empty;
        }
    }

    private void UpdateCell(Cell cell, CellState newState)
    {
        cell.State = newState;

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

        GameObject prefab = SelectionTypeToPrefab(currentType);
        if (prefab == null)
        {
            Debug.LogErrorFormat("Error finding prefab from type {0}", currentType);
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
            Debug.LogErrorFormat("[position]: {0} is not valid", position);
            return null;
        }

        for (int i = 0; i < agents.Length; ++i)
        {
            if (agents[i] != null && agents[i].Position == position)
                return agents[i];
        }
        return null;
    }

    private GameObject SelectionTypeToPrefab(SelectionType type)
    {
        switch (type)
        {
            case SelectionType.Heart: return heartPrefab;
            case SelectionType.Pusher: return pusherPrefab;
            case SelectionType.Puller: return pullerPrefab;
            case SelectionType.Swapper: return swapperPrefab;
            default: return null;
        }
    }

    private void PlaceCamera()
    {
        Camera.main.transform.position = new Vector3(width / 2f,
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

    public void SetCurrentType(SelectionType type)
    {
        Debug.LogFormat("called, {0}", type);
        currentType = type;
    }

    private void DrawMap()
    {
        Vector3 widthLine = Vector3.right * width;
        Vector3 heightLine = Vector3.forward * height;

        for (int z = 0; z <= width; ++z)
        {
            Vector3 start = Vector3.forward * z;
            Debug.DrawLine(start, start + widthLine);

            for (int x = 0; x <= width; ++x)
            {
                start = Vector3.right * x;
                Debug.DrawLine(start, start + heightLine);
            }
        }
    }
}