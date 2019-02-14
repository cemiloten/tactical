using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
    public MapLayout mapLayout;
    public GameObject colliderPrefab;
    public GameObject cellPrefab;

    private Cell[] cells;

    public static MapManager Instance { get; private set; }
    public List<Cell> VisualPath { get; set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
        SetupColliderPlane();
        InstantiateCells();
    }

    void Update()
    {
        // DrawMap();
        UpdateVisualPath();
    }

    private void SetupColliderPlane()
    {
        GameObject collider = Instantiate(
            colliderPrefab,
            new Vector3(mapLayout.width / 2f, 0f, mapLayout.height / 2f),
            Quaternion.identity);
        collider.transform.localScale = new Vector3(mapLayout.width / 10f, 1f, mapLayout.height / 10f);
        collider.transform.parent = transform;
    }

    private void PlaceAgent(Agent agent, Cell cell)
    {
        cell.CurrentState = Cell.State.Agent;
        agent.Position = cell.Position;
        agent.transform.position = Utilities.ToWorldPosition(cell.Position, agent.transform);
    }

    public void PlaceAgentsFromLayout(MapLayout mapLayout, Agent[] agents)
    {
        if (mapLayout == null)
        {
            Debug.LogError("[layout] is null");
            return;
        }

        for (int a = 0; a < mapLayout.agents.Length; ++a)
        {
            // todo: check array bounds
            PlaceAgent(agents[a], CellAt(mapLayout.agents[a].position));
        }
    }

    public void PlaceAgents(Agent[] agents)
    {
        for (int i = 0; i < agents.Length; ++i)
        {
            int index;
            Cell cell;
            // note: Very bad in case of no walkable cells in map
            do
            {
                index = Random.Range(0, cells.Length);
                cell = cells[index];
            } while (!cell.Walkable || cell.CurrentState == Cell.State.Hole);

            PlaceAgent(agents[i], cell);
        }
    }

    public bool IsPositionOnMap(Vector2Int pos)
    {
        return (pos.x >= 0 && pos.x < mapLayout.width && pos.y >= 0 && pos.y < mapLayout.height);
    }

    public bool AreNeighbours(Cell c1, Cell c2)
    {
        if (c1 == null || c2 == null)
        {
            Debug.LogError("[c1] or [c2] is null");
            return false;
        }

        if (!IsPositionOnMap(c1.Position) || !IsPositionOnMap(c2.Position))
        {
            Debug.LogError("[c1] or [c2] is not a position on the map");
            return false;
        }

        List<Cell> neighbours = GetNeighbours(c1);
        for (int i = 0; i < neighbours.Count; ++i)
        {
            if (neighbours[i] != null && neighbours[i] == c2)
                return true;
        }
        return false;
    }

    public Cell CellAt(Vector2Int pos)
    {
        if (cells == null)
            return null;

        if (!IsPositionOnMap(pos))
            return null;

        return cells[pos.x + pos.y * mapLayout.width];
    }

    public Cell CellAt(int x, int y)
    {
        return CellAt(new Vector2Int(x, y));
    }

    public List<Cell> GetNeighbours(Cell cell)
    {
        return new List<Cell>()
        {
            CellAt(cell.Position.x + 1, cell.Position.y), // right
            CellAt(cell.Position.x, cell.Position.y + 1), // top
            CellAt(cell.Position.x - 1, cell.Position.y), // left
            CellAt(cell.Position.x, cell.Position.y - 1)  // bottom
        };
    }

    public List<Cell> GetCastRange(Cell source, int range = 1, bool withSource = false)
    {
        if (source == null)
        {
            Debug.LogError("[source] is null");
            return null;
        }

        if (range < 0)
        {
            Debug.LogError("[range] cannot be inferior to 0");
            return null;
        }

        if (range == 0)
            return new List<Cell>() { source };

        List<Cell> cellsInRange = new List<Cell>();
        for (int i = -range; i <= range; ++i)
        {
            Vector2Int pos = new Vector2Int(source.Position.x, source.Position.y + i);
            int amount = range - Mathf.Abs(i);
            for (int j = -amount; j <= amount; ++j)
            {
                Cell cell = MapManager.Instance.CellAt(new Vector2Int(pos.x + j, pos.y));
                if (cell != null && cell.Walkable)
                {
                    if (cell == source && !withSource)
                        continue;
                    cellsInRange.Add(cell);
                }
            }
        }
        return cellsInRange;
    }

    private void InstantiateCells()
    {
        cells = new Cell[mapLayout.width * mapLayout.height];
        for (int y = 0; y < mapLayout.height; ++y)
            for (int x = 0; x < mapLayout.width; ++x)
            {
                GameObject go = Instantiate(cellPrefab, new Vector3(x, 0f, y), Quaternion.identity);
                Cell cell = go.GetComponent<Cell>();
                cell.Initialize(new Vector2Int(x, y));
                if (Array.Exists(
                    GameManager.Instance.mapLayout.winningPositions,
                    element => element == new Vector2Int(x, y)))
                {
                    cell.CurrentState = Cell.State.Hole;
                }
                cells[x + y * mapLayout.width] = cell;
            }
    }

    private void DrawMap()
    {
        Vector3 widthLine = Vector3.right * mapLayout.width;
        Vector3 heightLine = Vector3.forward * mapLayout.height;

        for (int z = 0; z <= mapLayout.width; ++z)
        {
            Vector3 start = Vector3.forward * z;
            Debug.DrawLine(start, start + widthLine);

            for (int x = 0; x <= mapLayout.width; ++x)
            {
                start = Vector3.right * x;
                Debug.DrawLine(start, start + heightLine);
            }
        }
    }

    private void UpdateVisualPath()
    {
        if (GameManager.Instance.HasBusyAgent)
            return;

        if (cells == null)
        {
            Debug.LogError("[cells] is null");
            return;
        }

        Agent selection = GameManager.Instance.Selection;
        if (selection == null)
            return;

        if (selection.CurrentAbility == null
            || selection.CurrentAbility.Type == Ability.CastType.None
            || selection.Busy)
        {
            VisualPath = null;
        }
        else
        {
            VisualPath = selection.CurrentAbility.Range(
                MapManager.Instance.CellAt(selection.Position));
        }

        for (int i = 0; i < cells.Length; ++i)
        {
            if (cells[i].CurrentState == Cell.State.Empty)
            {
                cells[i].Color = Color.gray;
            }
            else if (cells[i].CurrentState == Cell.State.Hole)
            {
                cells[i].Color = Color.black;
            }
            else if (cells[i].CurrentState == Cell.State.Agent)
            {
                cells[i].Color = new Color(1, 0.7f, 0);
            }
        }

        if (VisualPath != null)
        {
            for (int j = 0; j < VisualPath.Count; ++j)
            {
                Color rangeColor = selection.CurrentAbility.Type == Ability.CastType.Move ? Color.green : Color.magenta;
                if (VisualPath[j] != null && VisualPath[j].CurrentState != Cell.State.Hole)
                    VisualPath[j].Color = rangeColor;
            }
        }

        CellAt(selection.Position).Color = Color.white;
    }
}
