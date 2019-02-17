using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Tactical/Level")]
public class Level : ScriptableObject
{
    public int width;
    public int height;
    public Cell[] cells;
    public List<Agent> agents;
    public List<Vector2Int> holes;
}