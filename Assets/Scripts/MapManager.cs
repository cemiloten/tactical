using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public int width;
    public int height;
    public float tileSize = 1.0f;
    public float tileOffset = 0.5f;

    private Cell[] cells;

    public static MapManager Instance { get; private set; }
    public List<Cell> VisualPath { get; set; }

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
        VisualPath = new List<Cell>();
        InstantiateCells();
    }

    void Update()
    {
        DrawMap();
        UpdateCells();
    }

    public void PlaceAgents(List<Agent> agents)
    {
        for (int i = 0; i < agents.Count; ++i)
        {
            bool found = false;
            for (int j = 0; j < cells.Length; ++j)
            {
                Vector2Int pos = new Vector2Int(
                    Random.Range(0, width),
                    Random.Range(0, height));

                Cell cell = CellAt(pos);
                if (cell != null && cell.Walkable)
                {
                    found = true;
                    agents[i].SnapTo(pos);
                    break;
                }
            }
            if (!found)
            {
                Debug.LogError("No free cell to place agent to");
                return;
            }
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
            Debug.LogError("{c1} or {c2} is null");
            return false;
        }
        if (!IsPositionOnMap(c1.Position) || !IsPositionOnMap(c2.Position))
        {
            Debug.LogError("{c1} or {c2} is not a position on the map");
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

    private List<Cell> GetNeighbours(Cell cell)
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
            Debug.LogError("{source} is null");
            return null;
        }

        if (range < 0)
        {
            Debug.LogError("{range} cannot be inferior to 0");
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
                if (cell != null)// && (!withSource && cell != source))
                    cellsInRange.Add(cell);
            }
        }
        return cellsInRange;
    }

    private void InstantiateCells()
    {
        cells = new Cell[width * height];
        for (int y = 0; y < height; ++y)
            for (int x = 0; x < width; ++x)
            {
                GameObject go = new GameObject(string.Format("Cell ({0}, {1})", x, y));
                Cell cell = go.AddComponent<Cell>();
                cell.Initialize(new Vector2Int(x, y));
                cells[x + y * width] = cell;
            }
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

    private void UpdateCells()
    {
        Agent agent = GameManager.Instance.Selection;
        if (agent == null)
            return;
        // if (agent.CurrentAbility < 0)
        //     VisualPath = null;
        // else
        // {
        //     VisualPath = GetCastRange(
        //         MapManager.Instance.CellAt(agent.Position),
        //         agent.CurrentAbility.range);
        // }

        for (int i = 0; i < cells.Length; ++i)
        {
            if (VisualPath != null && VisualPath.Contains(cells[i]))
                cells[i].Color = Color.green;
            else
                cells[i].Color = Color.yellow;
        }
    }
}
