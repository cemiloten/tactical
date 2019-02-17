using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject colliderPrefab;

    public int width;
    public int height;
    private Cell[] cells;
    private new GameObject collider;

    public Cell[] Cells { get => cells; }
    public List<Cell> VisualPath { get; set; }
    public static MapManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }

    public void SetupColliderPlane()
    {
        collider = Instantiate(
            colliderPrefab,
            new Vector3(width / 2f, 0f, height / 2f),
            Quaternion.identity);
        collider.transform.localScale = new Vector3(width / 10f, 1f, height / 10f);
        collider.transform.parent = transform;
    }

    public void InstantiateCells(Cell[] newCells)
    {
        cells = new Cell[newCells.Length];
        for (int i = 0; i < newCells.Length; ++i)
        {
            GameObject go = Instantiate(cellPrefab);
            Cell cell = go.GetComponent<Cell>();
            cell.Initialize(newCells[i].Position, newCells[i].State);
        }
    }

    public void InstantiateEmptyCells()
    {
        cells = new Cell[width * height];
        for (int y = 0; y < height; ++y)
            for (int x = 0; x < width; ++x)
            {
                GameObject go = Instantiate(cellPrefab, new Vector3(x, 0f, y), Quaternion.identity);
                Cell cell = go.GetComponent<Cell>();
                cell.Initialize(new Vector2Int(x, y));
                cells[x + y * width] = cell;
            }
    }

    public void Cleanup()
    {
        if (cells != null)
        {
            for (int i = 0; i < cells.Length; ++i)
                Destroy(cells[i].gameObject);
            cells = null;
        }
        if (collider != null)
        {
            Destroy(collider);
            collider = null;
        }
    }

    public void PlaceAgents(Agent[] agents)
    {
        for (int a = 0; a < agents.Length; ++a)
        {
            // todo: check array bounds
            PlaceAgent(agents[a], CellAt(agents[a].Position));
        }
    }

    private void PlaceAgent(Agent agent, Cell cell)
    {
        if (agent == null)
        {
            Debug.LogError("[agent] is null");
            return;
        }

        if (cell == null)
        {
            Debug.LogError("[cell] is null");
            return;
        }

        cell.State = CellState.Agent;
        agent.Position = cell.Position;
        agent.transform.position = Utilities.ToWorldPosition(cell.Position, agent.transform);
    }

    public void PlaceAgentsRandom(Agent[] agents)
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
            } while (!cell.Walkable || cell.State == CellState.Hole);

            PlaceAgent(agents[i], cell);
        }
    }

    public bool IsPositionOnMap(Vector2Int pos)
    {
        return (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height);
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

        return cells[pos.x + pos.y * width];
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

    public void UpdateVisualPath()
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
            || selection.CurrentAbility.Type == AbilityType.None
            || selection.Busy)
        {
            VisualPath = null;
        }
        else
        {
            VisualPath = selection.CurrentAbility.Range(MapManager.Instance.CellAt(selection.Position));
        }

        for (int i = 0; i < cells.Length; ++i)
        {
            if (cells[i].State == CellState.Empty)
            {
                cells[i].Color = new Color(1, 1, 1, 0);
            }
            else if (cells[i].State == CellState.Hole)
            {
                cells[i].Color = Color.black;
            }
            else if (cells[i].State == CellState.Agent)
            {
                cells[i].Color = new Color(1, 0.7f, 0);
            }
        }

        if (VisualPath != null)
        {
            for (int j = 0; j < VisualPath.Count; ++j)
            {
                Color rangeColor = selection.CurrentAbility.Type == AbilityType.Move ? Color.green : Color.magenta;
                if (VisualPath[j] != null && VisualPath[j].State != CellState.Hole)
                    VisualPath[j].Color = rangeColor;
            }
        }

        CellAt(selection.Position).Color = Color.white;
    }
}
