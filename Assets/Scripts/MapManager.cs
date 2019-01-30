using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public int width;
    public int height;
    public List<Cell> cells;

    private const float tileSize = 1.0f;
    private const float tileOffset = 0.5f;

    private Vector2Int selection = new Vector2Int(-1, -1);

    void Start()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr)
        {
            mr.enabled = false;
        }

        cells = new List<Cell>(width * height);
        for (int y = 0; y < height; ++y)
            for (int x = 0; x < width; ++x)
               cells.Add(new Cell(new Vector2Int(x, y)));                 
    }

    void Update()
    {
        UpdateSelection();
        DrawMap();
    }
    
    private void UpdateSelection()
    {
        if (!Camera.main)
            return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25f, LayerMask.GetMask("Map")))
        {
            selection = new Vector2Int((int)hit.point.x, (int)hit.point.z);
        }

        if (selection.x >= 0 && selection.y >= 0)
        {
            Debug.DrawLine(
                Vector3.forward * selection.y + Vector3.right * selection.x,
                Vector3.forward * (selection.y + 1) + Vector3.right * (selection.x + 1));
            Debug.DrawLine(
                Vector3.forward * (selection.y + 1) + Vector3.right * selection.x,
                Vector3.forward * selection.y + Vector3.right * (selection.x + 1));
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
