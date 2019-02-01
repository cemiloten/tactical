using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
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

    public static List<Cell> FindPath(Cell _start, Cell _goal)
    {
        if (!_start.Walkable)
        {
            Debug.LogError("Cannot start path from non walkable cell");
            return null;
        }
        if (!_goal.Walkable)
        {
            Debug.LogError("Cannot find path to non walkable cell");
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
                // handle case where same f but we still want the node
                if (open[i].f < current.f) // || open[i] == goal)
                {
                    current = open[i];
                    current_index = i;
                }
            }
            open.RemoveAt(current_index);
            closed.Add(current); 

            if (current.Equals(goal))
                return PathFrom(current);

            List<Node> neighbours = new List<Node>()
            {
                new Node(manager.CellAt(current.cell.Position.x + 1, current.cell.Position.y), current), // right
                new Node(manager.CellAt(current.cell.Position.x, current.cell.Position.y + 1), current), // top
                new Node(manager.CellAt(current.cell.Position.x - 1, current.cell.Position.y), current), // left
                new Node(manager.CellAt(current.cell.Position.x, current.cell.Position.y - 1), current)  // bottom
            };

            for (int n = 0; n < neighbours.Count; ++n)
            {
                Node neighbour = neighbours[n];
                if (neighbour.cell == null || closed.Contains(neighbour))
                    continue;
                if (!neighbour.cell.Walkable)                
                    continue;
                
                neighbour.g = current.g + 1;
                neighbour.h = CalculateHeuristic(neighbour, goal);
                neighbour.f = neighbour.g + neighbour.h;

                // todo: refactor
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

    private static List<Cell> PathFrom(Node node)
    {
        if (node == null)
        {
            Debug.LogError("{node} null reference");
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