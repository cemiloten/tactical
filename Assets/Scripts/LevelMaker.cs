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
    public Slider widthSlider;
    public Slider heightSlider;
    public Button emptyButton;
    public Button holeButton;
    public Button heartButton;
    public Button pusherButton;
    public Button pullerButton;
    public Button swapperButton;

    private int width = 7;
    private int height = 7;
    private List<Agent> agents = new List<Agent>();
    private SelectionType currentType = SelectionType.Empty;


    private void Start()
    {
        emptyButton.onClick.AddListener   (delegate { SetCurrentType(SelectionType.Empty   ); });
        holeButton.onClick.AddListener    (delegate { SetCurrentType(SelectionType.Hole    ); });
        heartButton.onClick.AddListener   (delegate { SetCurrentType(SelectionType.Heart   ); });
        pusherButton.onClick.AddListener  (delegate { SetCurrentType(SelectionType.Pusher  ); });
        pullerButton.onClick.AddListener  (delegate { SetCurrentType(SelectionType.Puller  ); });
        swapperButton.onClick.AddListener (delegate { SetCurrentType(SelectionType.Swapper ); });

        Camera.main.transform.eulerAngles = new Vector3(45f, 0f, 0f);
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

    private Cell.State CurrentTypeToState()
    {
        switch (currentType)
        {
            case SelectionType.Heart:
            case SelectionType.Pusher:
            case SelectionType.Puller:
            case SelectionType.Swapper:
                return Cell.State.Agent;
            case SelectionType.Hole:
                return Cell.State.Hole;
            case SelectionType.Empty:
                return Cell.State.Empty;
            default:
                Debug.LogErrorFormat(
                    "Didn't find appropriate State for type '{0}', returning Empty",
                    currentType);
                return Cell.State.Empty;
        }
    }

    private void UpdateCell(Cell cell, Cell.State newState)
    {
        cell.CurrentState = newState;
        // todo: fix this (handle list correctly)

        Agent agent = AgentAt(cell.Position);
        bool wasNull = true;
        if (agent != null)
        {
            Destroy(agent.gameObject);
            wasNull = false;
        }

        if (newState != Cell.State.Agent)
        // No need to keep entry in list
        {
            agents.Remove(agent);
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
        if (wasNull)
            agents.Add(agent);
    }

    private Agent AgentAt(Vector2Int position)
    {
        Debug.LogFormat("AgentAt: arg position: {0}", position);
        for (int i = 0; i < agents.Count; ++i)
        {
            Debug.LogFormat("agent[{0}] position: {1}", agents[i], i);
            if (agents[i].Position == position)
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
        float max = Mathf.Max(width, height);
        Camera.main.transform.position = new Vector3(width / 2f, max, -max / 2f);
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