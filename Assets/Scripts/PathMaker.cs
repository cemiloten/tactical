using System.Collections.Generic;
using UnityEngine;

public static class PathMaker {
    private static readonly Direction[] directions = new Direction[4] {
        Direction.Right,
        Direction.Up,
        Direction.Left,
        Direction.Down
    };

    private static Cell DirectionToCell(Cell start, Direction direction, int distance) {
        if (start == null) {
            Debug.LogError("[start] is null.");
            return null;
        }

        if (distance < 1) {
            Debug.LogError("[distance] must be at least 1");
            return null;
        }

        switch (direction) {
            case Direction.Right:
                return MapManager.Instance.CellAt(start.Position.x + distance, start.Position.y);
            case Direction.Up:
                return MapManager.Instance.CellAt(start.Position.x, start.Position.y + distance);
            case Direction.Left:
                return MapManager.Instance.CellAt(start.Position.x - distance, start.Position.y);
            case Direction.Down:
                return MapManager.Instance.CellAt(start.Position.x, start.Position.y - distance);
            default:
                Debug.LogError("[direction] is not known");
                return null;
        }
    }

    // Get cells that are in a straight line at a maximum distance of [range]
    public static List<Cell> GetStraightLine(Cell start, int range) {
        if (start == null) {
            Debug.LogError("[start] is null");
            return null;
        }

        if (range < 1) {
            Debug.LogError("[range] cannot be inferior to 1");
            return null;
        }

        var result = new List<Cell>();
        for (int d = 0; d < directions.Length; ++d)
        for (int r = 1; r <= range; ++r) {
            Cell cell = DirectionToCell(start, directions[d], r);
            if (cell == null)
                break;
            if (!cell.Walkable) {
                // Accept agent, but path ends there.
                if (cell.Type == CellType.Agent)
                    result.Add(cell);
                break;
            }
            result.Add(cell);
        }
        return result;
    }

    public static List<Cell> GetNeighbours(Cell cell) =>
        new List<Cell> {
            MapManager.Instance.CellAt(cell.Position.x + 1, cell.Position.y),
            MapManager.Instance.CellAt(cell.Position.x, cell.Position.y + 1),
            MapManager.Instance.CellAt(cell.Position.x - 1, cell.Position.y),
            MapManager.Instance.CellAt(cell.Position.x, cell.Position.y - 1)
        };

    public static List<Cell> StraightPath(Cell start, Vector2Int direction, int distance) {
        if (start == null) {
            Debug.LogError("[start] is null");
            return null;
        }

        if (distance < 1) {
            Debug.LogError("[distance] cannot be inferior to 1");
            return null;
        }

        var path = new List<Cell> { start };
        for (int i = 1; i <= distance; ++i) {
            Cell cell = MapManager.Instance.CellAt(
                start.Position + new Vector2Int(i * direction.x, i * direction.y));
            if (cell == null || !cell.Walkable)
                break;
            path.Add(cell);
        }
        return path;
    }

    public static List<Cell> GetKnightRange(Cell start) =>
        new List<Cell> {
            MapManager.Instance.CellAt(start.Position.x + 2, start.Position.y + 1),
            MapManager.Instance.CellAt(start.Position.x + 1, start.Position.y + 2),
            MapManager.Instance.CellAt(start.Position.x - 1, start.Position.y + 2),
            MapManager.Instance.CellAt(start.Position.x - 2, start.Position.y + 1),
            MapManager.Instance.CellAt(start.Position.x - 2, start.Position.y - 1),
            MapManager.Instance.CellAt(start.Position.x - 1, start.Position.y - 2),
            MapManager.Instance.CellAt(start.Position.x + 1, start.Position.y - 2),
            MapManager.Instance.CellAt(start.Position.x + 2, start.Position.y - 1)
        };

    public static List<Cell> ExpandToWalkables(Cell start, int range) {
        if (start == null) {
            Debug.LogError("[start] is null");
            return null;
        }

        if (range == 0)
            return null;

        var result = new List<Cell>();
        Expand(start, result, range, 0);
        return result;
    }

    private static void Expand(Cell cell, List<Cell> result, int range, int distance) {
        if (distance >= range)
            return;

        List<Cell> neighbours = GetNeighbours(cell);
        for (int i = 0; i < neighbours.Count; ++i) {
            if (neighbours[i] == null || !neighbours[i].Walkable)
                continue;

            if (!result.Contains(neighbours[i]))
                result.Add(neighbours[i]);

            Expand(neighbours[i], result, range, distance + 1);
        }
    }

    // public static List<Cell> AStarToNearestNeighbour(Cell _start, Cell _goal)
    // {
    //     if (_start == null)
    //     {
    //         Debug.LogError("[_start] is null");
    //         return null;
    //     }
    //     if (_goal == null)
    //     {
    //         Debug.LogError("[_goal] is null");
    //         return null;
    //     }
    //
    //     Node start = new Node(_start);
    //     Node goal = new Node(_goal);
    //     List<Node> neighbours = GetNeighbours(goal);
    //
    //     Node nearest = null;
    //     for (int n = 0; n < neighbours.Count; ++n)
    //     {
    //         if (neighbours[n] != null && neighbours[n].cell != null)
    //         {
    //             nearest = neighbours[n];
    //             break;
    //         }
    //     }
    //     if (nearest == null)
    //         // Every neighbour of target is null
    //         return null;
    //
    //     // Compare distance to other non null neighbours
    //     for (int n = 0; n < neighbours.Count; ++n)
    //     {
    //         if (neighbours[n] != null
    //             && neighbours[n].cell != null
    //             && neighbours[n].cell.Walkable
    //             && CalculateHeuristic(start, neighbours[n]) < CalculateHeuristic(start, nearest))
    //         {
    //             nearest = neighbours[n];
    //         }
    //     }
    //
    //     if (nearest == null)
    //         return null;
    //     return AStar(_start, nearest.cell);
    // }

    public static bool AStar(Cell _start, Cell _goal, out List<Cell> path) {
        path = null;

        if (_start == null) {
            Debug.LogError("[_start] is null");
            return false;
        }

        if (_goal == null) {
            Debug.LogError("[_goal] is null");
            return false;
        }

        var start = new Node(_start);
        var goal = new Node(_goal);
        var open = new List<Node> { start };
        var closed = new List<Node>();

        while (open.Count > 0) {
            Node current = open[0];
            int currentIndex = 0;

            for (int i = 0; i < open.Count; ++i)
                if (open[i].f < current.f) {
                    current = open[i];
                    currentIndex = i;
                }
            open.RemoveAt(currentIndex);
            closed.Add(current);

            if (current.Equals(goal)) {
                path = InvertedPathFrom(current);
                return true;
            }

            List<Node> neighbours = GetNeighbours(current);
            for (int n = 0; n < neighbours.Count; ++n) {
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
                    // We already added this node in a previous iteration (previous g)
                    if (open[i].Equals(neighbour) && neighbour.g > open[i].g) {
                        _continue = true;
                        break;
                    }
                if (_continue)
                    continue;

                if (!open.Contains(neighbour))
                    open.Add(neighbour);
            }
        }

        Debug.LogError("Failed: nothing to return");
        return false;
    }

    private static List<Node> GetNeighbours(Node node) =>
        new List<Node> {
            new Node(MapManager.Instance.CellAt(node.cell.Position.x + 1, node.cell.Position.y),
                     node), // right
            new Node(MapManager.Instance.CellAt(node.cell.Position.x, node.cell.Position.y + 1),
                     node), // top
            new Node(MapManager.Instance.CellAt(node.cell.Position.x - 1, node.cell.Position.y),
                     node), // left
            new Node(MapManager.Instance.CellAt(node.cell.Position.x, node.cell.Position.y - 1),
                     node) // bottom
        };


    private static List<Cell> InvertedPathFrom(Node node) {
        if (node == null) {
            Debug.LogError("[node] is null");
            return null;
        }

        if (node.cell == null) {
            Debug.LogError("[node.cell] is null");
            return null;
        }

        var path = new List<Cell>();
        while (node.cell != null && node.parent != null) {
            path.Add(node.cell);
            node = node.parent;
        }
        path.Reverse();
        return path;
    }

    private static int CalculateHeuristic(Node node, Node goal) {
        int x = goal.cell.Position.x - node.cell.Position.x;
        int y = goal.cell.Position.y - node.cell.Position.y;
        return x * x + y * y;
    }

    private enum Direction {
        Right,
        Up,
        Left,
        Down
    }


    private class Node {
        public readonly Cell cell;
        public readonly Node parent;

        // Total cost of the node.
        public int f;

        // Distance between node and start node.
        public int g;

        // Heuristic - Estimated distance from the current node to the end node.
        public int h;

        public Node(Cell _cell = null, Node _parent = null) {
            cell = _cell;
            parent = _parent;
            g = 0;
            h = 0;
            f = 0;
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;
            return cell.Position == ((Node)obj).cell.Position;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}