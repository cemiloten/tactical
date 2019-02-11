using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapLayout layout;
    public GameObject colliderPrefab;

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

        SetupColliderPlane();
        InstantiateCells();
    }

    void Update()
    {
        DrawMap();
        UpdateVisualPath();
    }

    private void SetupColliderPlane()
    {
        GameObject collider = Instantiate(
            colliderPrefab,
            new Vector3(layout.width / 2f, 0f, layout.height / 2f),
            Quaternion.identity);
        collider.transform.localScale = new Vector3(layout.width / 10f, 1f, layout.height / 10f);
        collider.transform.parent = transform;
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
            } while (!cell.Walkable || cell.CurrentState == Cell.State.Win);

            PlaceAgent(agents[i], cell);
        }
    }

    private void PlaceAgent(Agent agent, Cell cell)
    {
        cell.CurrentState = Cell.State.Agent;
        agent.Position = cell.Position;
        agent.transform.position = Utilities.ToWorldPosition(cell.Position, agent.transform);
    }

    public bool IsPositionOnMap(Vector2Int pos)
    {
        return (pos.x >= 0 && pos.x < layout.width && pos.y >= 0 && pos.y < layout.height);
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

        return cells[pos.x + pos.y * layout.width];
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
        cells = new Cell[layout.width * layout.height];
        for (int y = 0; y < layout.height; ++y)
            for (int x = 0; x < layout.width; ++x)
            {
                GameObject go = new GameObject(string.Format("[{0}, {1}]", x, y));
                Cell cell = go.AddComponent<Cell>();
                cell.Initialize(new Vector2Int(x, y));
                if ((x == 0 && y == 0) || (x == layout.width - 1 && y == layout.height - 1))
                {
                    cell.CurrentState = Cell.State.Win;
                }
                cells[x + y * layout.width] = cell;
            }
    }

    private void DrawMap()
    {
        Vector3 widthLine = Vector3.right * layout.width;
        Vector3 heightLine = Vector3.forward * layout.height;

        for (int z = 0; z <= layout.width; ++z)
        {
            Vector3 start = Vector3.forward * z;
            Debug.DrawLine(start, start + widthLine);

            for (int x = 0; x <= layout.width; ++x)
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

        Agent agent = GameManager.Instance.Selection;
        if (agent == null)
            return;

        if (agent.CurrentAbility == null
            || agent.CurrentAbility.Type == Ability.CastType.None
            || agent.Busy)
        {
            VisualPath = null;
        }
        else
        {
            VisualPath = agent.CurrentAbility.Range(
                MapManager.Instance.CellAt(agent.Position));
        }

        for (int i = 0; i < cells.Length; ++i)
        {
            if (cells[i].Walkable)
            {
                cells[i].Color = Color.yellow;
            }
            else if (cells[i].CurrentState == Cell.State.Win)
            {
                if (cells[i].Position == new Vector2Int(0, 0))
                {
                    cells[i].Color = new Color(1, 0, 0.2f);
                }
                else
                {
                    cells[i].Color = new Color(0, 0.5f, 1);
                }
            }
            else
            {
                cells[i].Color = new Color(1, 0.7f, 0);
            }
        }

        if (VisualPath != null)
        {
            for (int j = 0; j < VisualPath.Count; ++j)
            {
                if (VisualPath[j] != null)
                    VisualPath[j].Color = Color.green;
            }
        }

        Agent selection = GameManager.Instance.Selection;
        if (selection != null)
        {
            CellAt(selection.Position).Color = Color.black;
        }
    }
}
