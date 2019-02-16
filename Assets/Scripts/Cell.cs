using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellState
{
    Empty,
    Agent,
    Obstacle,
    Hole
}

public class Cell : MonoBehaviour
{
    private Color color;
    private CellState state = CellState.Empty;
    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock propertyBlock;

    public Vector2Int Position { get; private set; }
    public bool Walkable { get { return state == CellState.Empty || state == CellState.Hole; } }

    public CellState State
    {
        get => state;
        set
        {
            state = value;
            SetColorInShader(value.ToCellColor());
        }
    }

    public Color Color
    {
        get => color;
        set
        {
            color = value;
            SetColorInShader(value);
        }
    }

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        meshRenderer = GetComponent<MeshRenderer>();
        Color = state.ToCellColor();
    }

    public void Initialize(Vector2Int pos)
    {
        Position = pos;
        state = CellState.Empty;
        color = state.ToCellColor();
        transform.position = Utilities.ToWorldPosition(pos, transform);
    }

    private void SetColorInShader(Color col)
    {
        propertyBlock.SetColor("_Color", col);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(transform.position, new Vector3(0.8f, 0.01f, 0.8f));
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
        Cell other = (Cell)obj;
        return Position == other.Position && state == other.state;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
