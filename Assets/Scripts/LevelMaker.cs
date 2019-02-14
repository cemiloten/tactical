using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMaker : MonoBehaviour
{
    public GameObject colliderPrefab;
    public GameObject cellPrefab;

    public Text widthText;
    public Text heightText;
    public Slider widthSlider;
    public Slider heightSlider;

    private int width = 7;
    private int height = 7;
    private Cell[] cells;


    private void Start()
    {
        Camera.main.transform.eulerAngles = new Vector3(45f, 0f, 0f);
        GenerateGrid();
    }

    private void Update()
    {
        // DrawMap();

        Vector2Int mousePos;
        Utilities.MousePositionOnMap(out mousePos);

        if (IsPositionOnMap(mousePos))
        {
        }
    }

    public bool IsPositionOnMap(Vector2Int pos)
    {
        return (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height);
    }

    public void GenerateGrid()
    {
        InstantiateCells();
        PlaceCamera();
    }

    private void InstantiateCells()
    {
        if (cells != null)
        {
            for (int i = 0; i < cells.Length; ++i)
                Destroy(cells[i].gameObject);
        }
        cells = new Cell[width * height];
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                GameObject go = Instantiate(cellPrefab, new Vector3(x, -0.2f, y), Quaternion.identity);
                go.name = string.Format("Cell[{0}, {1}]", x, y);
                go.transform.parent = transform;
                Cell cell = go.GetComponent<Cell>();
                cell.Initialize(new Vector2Int(x, y));
                cells[x + y * width] = cell;
            }
        }
    }

    private void PlaceCamera()
    {
        float d = Mathf.Max(width, height);
        Camera.main.transform.position = new Vector3(width / 2f, d, -d / 2f);
    }

    private void SetupColliderPlane()
    {
        GameObject collider = Instantiate(
            colliderPrefab,
            new Vector3(width / 2f, 0f, height / 2f),
            Quaternion.identity);
        collider.transform.localScale = new Vector3(width / 10f, 1f, height / 10f);
        collider.transform.parent = transform;
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