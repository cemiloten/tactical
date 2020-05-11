using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class MapManager : MonoBehaviourSingleton<MapManager>
{
    [SerializeField] public Cell cellPrefab;
    [SerializeField] public int width;
    [SerializeField] public int height;

    private Cell[] _cells;
    private List<Cell> _groundCells;

    public List<Cell> VisualPath { get; set; }

    protected override void OnAwake()
    {
        InstantiateCells();
        _groundCells = _cells.ToList();
    }

    private Cell GetRandomCell()
    {
        if (_groundCells.Count == 0)
            return null;

        return _groundCells[Random.Range(0, _groundCells.Count)];
    }

    public void InstantiateCells()
    {
        _cells = new Cell[width * height];
        int index = 0;
        for (int y = 0; y < height; ++y)
            for (int x = 0; x < width; ++x)
            {
                Cell cell = Instantiate(cellPrefab);
                cell.Initialize(new Vector2Int(x, y));
                cell.transform.parent = transform;

                _cells[index++] = cell;
            }
    }

    public void Cleanup()
    {
        if (_cells == null)
            return;

        for (int i = 0; i < _cells.Length; ++i)
            Destroy(_cells[i].gameObject);
        _cells = null;
    }

    public (bool, Vector2Int) ReserveRandomCell(CellType cellType, Agent agent)
    {
        if (agent == null)
            return (false, Vector2Int.zero);

        Cell c = GetRandomCell();
        if (c == null)
            return (false, Vector2Int.zero);

        _groundCells.Remove(c);
        c.Type = cellType;
        c.Agent = agent;
        return (true, c.Position);
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

        cell.Type = CellType.Agent;
        agent.Position = cell.Position;
        agent.transform.position = Utilities.ToWorldPosition(cell.Position, agent.transform);
    }

    public bool IsPositionOnMap(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width
            && pos.y >= 0 && pos.y < height;
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

        // todo: that's horrible
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
        if (_cells == null)
            return null;

        if (!IsPositionOnMap(pos))
            return null;

        return _cells[pos.x + pos.y * width];
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

    public Agent AgentAt(Cell cell)
    {
        return cell.Agent ?? null;
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
