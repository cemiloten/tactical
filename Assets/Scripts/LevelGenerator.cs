using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator
{
    private enum CellType
    {
        Empty,
        Obstacle,
        Hole,
        Heart,
        Pusher,
        Puller,
        Swapper
    }

    private Cell[] cells;
    private List<Agent> agents;

    public static void WriteLevelFile(Cell[] cells, int width, int height, string filepath)
    {
        Texture2D tex = new Texture2D(width, height);
        for (int i = 0; i < cells.Length; ++i)
        {
        }
    }

    public static Cell[] GenerateLevel(Texture2D level)
    {
        Cell[] cells = new Cell[level.width * level.height];
        for (int y = 0; y < level.height; ++y)
        {
            for (int x = 0; x < level.width; ++x)
            {

            }
        }

        return null;
    }

    private void GenerateCell(Color color)
    {

    }
}