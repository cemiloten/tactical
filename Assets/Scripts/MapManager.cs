using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public int width;
    public int height;
    public float tileSize = 1.0f;
    public float tileOffset = 0.5f;

    private List<Cell> cells;

    public static MapManager Instance { get; private set; }
    public Cell HoverCell { get; private set; }

private Cell start;
private Cell goal;
public List<Cell> path;

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
        cells = new List<Cell>(width * height);
        for (int y = 0; y < height; ++y)
            for (int x = 0; x < width; ++x)
            {
               GameObject go = new GameObject(string.Format("Cell {0} - {1}", x, y));
               Cell cell = go.AddComponent<Cell>();
               cell.Initialize(new Vector2Int(x, y));
               cells.Add(cell);
            }
        start = null;
        goal = null;
        path = null;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(
                Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25f, LayerMask.GetMask("Map")))
            {
                Vector2Int pos = new Vector2Int((int)hit.point.x, (int)hit.point.z);
                if (IsPositionOnMap(pos))
                {
                    start = CellAt(pos);
                }
            }
        }

        if (start != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(
                Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25f, LayerMask.GetMask("Map")))
            {
                Vector2Int pos = new Vector2Int((int)hit.point.x, (int)hit.point.z);
                if (IsPositionOnMap(pos))
                {
                    goal = CellAt(pos);
                }
            }
            path = AStar.FindPath(start, goal);
        }


        UpdateHoverCell();
        DrawMap();
    }

    public static Vector3 ToWorldPosition(Vector2Int pos, Transform transform = null)
    {
        float y = transform == null ? 0f : transform.position.y;
        return new Vector3(pos.x + 0.5f, y, pos.y + 0.5f);
    }

    public static int Distance(Vector2Int start, Vector2Int end)
    {
        return (Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y));
    }

    public void PlaceAgents(List<Agent> agents)
    {
       for (int i = 0; i < agents.Count; ++i)
       {
           int x = Random.Range(0, width);
           int y = Random.Range(0, height);
           Cell cell = CellAt(x, y);
           if (cell != null && cell.Walkable)
                agents[i].SnapTo(new Vector2Int(x, y));
       }
    }

    public bool IsPositionOnMap(Vector2Int pos)
    {
        return (pos.x >= 0 && pos.x < width
            && pos.y >= 0 && pos.y < height);
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

    private void UpdateHoverCell()
    {
        if (!Camera.main)
            return;

        RaycastHit hit;
        if (Physics.Raycast(
            Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25f, LayerMask.GetMask("Map")))
        {
            Vector2Int pos = new Vector2Int((int)hit.point.x, (int)hit.point.z);
            if (IsPositionOnMap(pos))
            {
                HoverCell = CellAt(pos);
                // todo: hover must be true only for cell currently under mouse
            }
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
}
