using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathMaker
{
    private enum Direction
    {
        Right,
        Up,
        Left,
        Down
    }

    private static System.Collections.IEnumerable Directions()
    {
        yield return Direction.Right;
        yield return Direction.Up;
        yield return Direction.Left;
        yield return Direction.Down;
    }

    private static Cell DirectionToCell(Cell start, Direction direction, int range)
    {
        if (start == null)
        {
            Debug.LogError("[start] is null.");
            return null;
        }

        if (range < 1)
        {
            Debug.LogError("[range] must be at least 1");
            return null;
        }

        switch (direction)
        {
            case Direction.Right:
                return MapManager.Instance.CellAt(start.Position.x + range, start.Position.y);
            case Direction.Up:
                return MapManager.Instance.CellAt(start.Position.x, start.Position.y + range);
            case Direction.Left:
                return MapManager.Instance.CellAt(start.Position.x - range, start.Position.y);
            case Direction.Down:
                return MapManager.Instance.CellAt(start.Position.x, start.Position.y - range);
            default:
                Debug.LogError("[direction] is not known");
                return null;
        }
    }

    // Get cells that are in a straight line at a maximum distance of [range]
    public static List<Cell> GetStraightLine(Cell start, int range)
    {
        if (start == null)
        {
            Debug.LogError("[start] is null");
            return null;
        }

        if (range < 1)
        {
            Debug.LogError("[range] cannot be inferior to 1");
            return null;
        }

        List<Cell> result = new List<Cell>();
        foreach (Direction dir in Directions())
        {
            for (int r = 1; r <= range; ++r)
            {
                // todo: refactor
                Cell cell = DirectionToCell(start, dir, r);
                if (cell == null)
                    break;
                if (!cell.Walkable)
                {
                    // Can cast on agent, but ends there.
                    if (cell.CurrentState == Cell.State.Agent)
                        result.Add(cell);
                    break;
                }
                result.Add(cell);
            }
        }
        return result;
    }

    public static List<Cell> GetNeighbours(Cell cell)
    {
        return new List<Cell>()
        {
            DirectionToCell(cell, Direction.Right, 1),
            DirectionToCell(cell, Direction.Up,    1),
            DirectionToCell(cell, Direction.Left,  1),
            DirectionToCell(cell, Direction.Down,  1)
        };
    }

    public static List<Cell> StraightPath(Cell start, Vector2Int direction, int distance)
    {
        List<Cell> path = new List<Cell>() { start };
        for (int i = 1; i <= distance; ++i)
        {
            Cell cell = MapManager.Instance.CellAt(
                start.Position + new Vector2Int(i * direction.x, i * direction.y));
            if (cell == null || !cell.Walkable)
                break;
            path.Add(cell);
        }
        return path;
    }

    public static List<Cell> GetKnightRange(Cell start)
    {
        return new List<Cell>()
        {
            MapManager.Instance.CellAt(start.Position.x + 2, start.Position.y + 1),
            MapManager.Instance.CellAt(start.Position.x + 1, start.Position.y + 2),
            MapManager.Instance.CellAt(start.Position.x - 1, start.Position.y + 2),
            MapManager.Instance.CellAt(start.Position.x - 2, start.Position.y + 1),
            MapManager.Instance.CellAt(start.Position.x - 2, start.Position.y - 1),
            MapManager.Instance.CellAt(start.Position.x - 1, start.Position.y - 2),
            MapManager.Instance.CellAt(start.Position.x + 1, start.Position.y - 2),
            MapManager.Instance.CellAt(start.Position.x + 2, start.Position.y - 1)
        };
    }

    public static List<Cell> ExpandToWalkables(Cell start, int range)
    {
        if (start == null)
        {
            Debug.LogError("[start] is null");
            return null;
        }

        if (range == 0)
        {
            return new List<Cell>() { start };
        }

        List<Cell> result = new List<Cell>();
        Expand(start, result, range, 0);
        return result;
    }

    private static void Expand(Cell cell, List<Cell> result, int range, int distance)
    {
        if (distance >= range)
            return;

        List<Cell> neighbours = GetNeighbours(cell);
        for (int i = 0; i < neighbours.Count; ++i)
        {
            if (neighbours[i] == null || !neighbours[i].Walkable)
            {
                continue;
            }

            if (!result.Contains(neighbours[i]))
            {
                result.Add(neighbours[i]);
            }

            Expand(neighbours[i], result, range, distance + 1);
        }
    }


    private class Node
    {
        public Cell cell;
        public Node parent;

        // Distance between node and start node.
        public int g;

        // Heuristic - Estimated distance from the current node to the end node.
        public int h;

        // Total cost of the node.
        public int f;

        public Node(Cell _cell = null, Node _parent = null)
        {
            cell = _cell;
            parent = _parent;
            g = 0;
            h = 0;
            f = 0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            return cell.Position == ((Node)obj).cell.Position;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static List<Cell> AStarToNearestNeighbour(Cell _start, Cell _goal)
    {
        if (_start == null)
        {
            Debug.LogError("[_start] is null");
            return null;
        }
        if (_goal == null)
        {
            Debug.LogError("[_goal] is null");
            return null;
        }

        Node start = new Node(_start);
        Node goal = new Node(_goal);
        List<Node> neighbours = GetNeighbours(goal);

        Node nearest = null;
        for (int n = 0; n < neighbours.Count; ++n)
        {
            if (neighbours[n] != null && neighbours[n].cell != null)
            {
                nearest = neighbours[n];
                break;
            }
        }
        if (nearest == null)
            // Every neighbour of target is null
            return null;

        // Compare distance to other non null neighbours
        for (int n = 0; n < neighbours.Count; ++n)
        {
            if (neighbours[n] != null
                && neighbours[n].cell != null
                && neighbours[n].cell.Walkable
                && CalculateHeuristic(start, neighbours[n]) < CalculateHeuristic(start, nearest))
            {
                nearest = neighbours[n];
            }
        }

        if (nearest == null)
            return null;
        return AStar(_start, nearest.cell);
    }

    public static List<Cell> AStar(Cell _start, Cell _goal)
    {
        if (_start == null)
        {
            Debug.LogError("[_start] is null");
            return null;
        }

        if (_goal == null)
        {
            Debug.LogError("[_goal] is null");
            return null;
        }

        if (!_goal.Walkable)
        {
            Debug.LogError("[_goal] is not walkable");
            return null;
        }

        MapManager manager = MapManager.Instance;
        Node start = new Node(_start);
        Node goal = new Node(_goal);
        Node current = new Node();
        List<Node> open = new List<Node>() { start };
        List<Node> closed = new List<Node>();

        while (open.Count > 0)
        {
            current = open[0];
            int current_index = 0;

            for (int i = 0; i < open.Count; ++i)
            {
                if (open[i].f < current.f)
                {
                    current = open[i];
                    current_index = i;
                }
            }
            open.RemoveAt(current_index);
            closed.Add(current);

            if (current.Equals(goal))
                return InvertedPathFrom(current);

            List<Node> neighbours = GetNeighbours(current);
            for (int n = 0; n < neighbours.Count; ++n)
            {
                Node neighbour = neighbours[n];
                if (neighbour == null || neighbour.cell == null || closed.Contains(neighbour))
                    continue;
                if (!neighbour.cell.Walkable)
                    continue;

                neighbour.g = current.g + 1;
                neighbour.h = CalculateHeuristic(neighbour, goal);
                neighbour.f = neighbour.g + neighbour.h;

                bool _continue = false;
                for (int i = 0; i < open.Count; ++i)
                {
                    // We already added this node in a previous iteration (previous g)
                    if (open[i].Equals(neighbour) && neighbour.g > open[i].g)
                    {
                        _continue = true;
                        break;
                    }
                }
                if (_continue)
                    continue;

                if (!open.Contains(neighbour))
                    open.Add(neighbour);
            }
        }

        Debug.LogError("Failed: nothing to return");
        return null;
    }

    private static List<Node> GetNeighbours(Node node)
    {
        return new List<Node>()
        {
            new Node(MapManager.Instance.CellAt(node.cell.Position.x + 1, node.cell.Position.y), node), // right
            new Node(MapManager.Instance.CellAt(node.cell.Position.x, node.cell.Position.y + 1), node), // top
            new Node(MapManager.Instance.CellAt(node.cell.Position.x - 1, node.cell.Position.y), node), // left
            new Node(MapManager.Instance.CellAt(node.cell.Position.x, node.cell.Position.y - 1), node)  // bottom
        };
    }


    private static List<Cell> InvertedPathFrom(Node node)
    {
        if (node == null)
        {
            Debug.LogError("[node] is null");
            return null;
        }

        if (node.cell == null)
        {
            Debug.LogError("[node.cell] is null");
            return null;
        }

        List<Cell> path = new List<Cell>();
        while (node.cell != null && node.parent != null)
        {
            path.Add(node.cell);
            node = node.parent;
        }
        path.Reverse();
        return path;
    }

    private static int CalculateHeuristic(Node node, Node goal)
    {
        int x = goal.cell.Position.x - node.cell.Position.x;
        int y = goal.cell.Position.y - node.cell.Position.y;
        return (x * x + y * y);
    }
}