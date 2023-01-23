using System;
using System.Collections.Generic;

namespace SyncChess
{
public class AStar
{
    // AStar pathfinding algorithm
    // http://www.policyalmanac.org/games/aStarTutorial.htm

    private List<AStarNode> openList;
    private List<AStarNode> closedList;
    private List<AStarNode> pathList;

    public AStar()
    {
        openList = new List<AStarNode>();
        closedList = new List<AStarNode>();
        pathList = new List<AStarNode>();
    }

    public List<AStarNode> GetPath(int[,] map, int startX, int startY, int endX, int endY)
    {
        // Clear the lists
        openList.Clear();
        closedList.Clear();
        pathList.Clear();

        // Create the start node
        AStarNode startNode = new AStarNode(startX, startY);
        startNode.G = 0;
        startNode.H = GetH(startNode, endX, endY);
        startNode.F = startNode.G + startNode.H;

        //G - distance from start to current nodes
        //H - distance from current node to end nodes
        //F - G + H - total cost of the path

        startNode.Parent = null;

        // Add the start node to the open list
        openList.Add(startNode);

        // Loop until the open list is empty
        while (openList.Count > 0)
        {
            // Get the node with the lowest F value
            AStarNode currentNode = openList[0];

            // Loop through the open list to find the node with the lowest F value
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].F < currentNode.F)
                {
                    currentNode = openList[i];
                }
            }

            // Add the current node to the closed list
            closedList.Add(currentNode);

            // Remove the current node from the open list
            openList.Remove(currentNode);

            // If the current node is the end node, then we have found the path
            if (currentNode.X == endX && currentNode.Y == endY)
            {
                // Get the path
                AStarNode node = currentNode;

                while (node.Parent != null)
                {
                    pathList.Add(node);
                    node = node.Parent;
                }

                // Reverse the path
                pathList.Reverse();

                // Return the path
                return pathList;
            }

            // Get the adjacent nodes
            List<AStarNode> adjacentNodes = GetAdjacentNodes(map, currentNode, endX, endY);

            // Loop through the adjacent nodes
            for (int i = 0; i < adjacentNodes.Count; i++)
            {
                AStarNode adjacentNode = adjacentNodes[i];

                // If the adjacent node is in the closed list, ignore it
                if (closedList.Contains(adjacentNode))
                {
                    continue;
                }

                // Set the G, H, and F values -
                // G = distance from start to current node
                // H = distance from current node to end node
                // F = G + H - total cost of the path
                //
                // If the adjacent node is in the open list, check to see if this path to the node is better, using G cost as the measure
                // If it is a better path, change the parent of the node and recalculate the G and F scores
                // If the node is not in the open list, calculate the G and F scores and set the parent then add it to the open lists
                if (openList.Contains(adjacentNode))
                {
                    if (adjacentNode.G > currentNode.G + 1)
                    {
                        adjacentNode.Parent = currentNode;
                        adjacentNode.G = currentNode.G + 1;
                        adjacentNode.F = adjacentNode.G + adjacentNode.H;
                    }
                }
                else
                {
                    adjacentNode.Parent = currentNode;
                    adjacentNode.G = currentNode.G + 1;
                    adjacentNode.H = GetH(adjacentNode, endX, endY);
                    adjacentNode.F = adjacentNode.G + adjacentNode.H;
                    openList.Add(adjacentNode);
                }
            }
        }

        // No path found
        return null;
    }

    // This code ignores blockers because it is only checking the adjacent nodes to the current node.
    // If the current node is a blocker, then the adjacent nodes will be checked and the path will be found around the blocker.
    // If the current node is not a blocker, then the adjacent nodes will be checked and the path will be found through the blocker.
    // If the current node is a blocker and there are no adjacent nodes, then the path will not be found.
    // If the current node is not a blocker and there are no adjacent nodes, then the path will not be found.
    // This code will not ignore blockers if the current node is a blocker and there are no adjacent nodes.
    // This code will not ignore blockers if the current node is not a blocker and there are no adjacent nodes.

    private List<AStarNode> GetAdjacentNodes(
        int[,] map,
        AStarNode currentNode,
        int endX,
        int endY
    )
    {
        List<AStarNode> adjacentNodes = new List<AStarNode>();

        // Check the node above the current node
        if (currentNode.Y - 1 >= 0)
        {
            if (map[currentNode.X, currentNode.Y - 1] == 0)
            {
                adjacentNodes.Add(new AStarNode(currentNode.X, currentNode.Y - 1));
            }
        }

        // Check the node below the current node
        if (currentNode.Y + 1 < map.GetLength(1))
        {
            if (map[currentNode.X, currentNode.Y + 1] == 0)
            {
                adjacentNodes.Add(new AStarNode(currentNode.X, currentNode.Y + 1));
            }
        }

        // Check the node to the left of the current node
        if (currentNode.X - 1 >= 0)
        {
            if (map[currentNode.X - 1, currentNode.Y] == 0)
            {
                adjacentNodes.Add(new AStarNode(currentNode.X - 1, currentNode.Y));
            }
        }

        // Check the node to the right of the current node
        if (currentNode.X + 1 < map.GetLength(0))
        {
            if (map[currentNode.X + 1, currentNode.Y] == 0)
            {
                adjacentNodes.Add(new AStarNode(currentNode.X + 1, currentNode.Y));
            }
        }


        return adjacentNodes;
    }

    private int GetH(AStarNode node, int endX, int endY)
    {
        // Use the Manhattan method for calculating H - the distance between the current node and the end node
        return Math.Abs(node.X - endX) + Math.Abs(node.Y - endY);
    }
}

public class AStarNode
{
    public int X;
    public int Y;
    public int G; // distance from start to current nodes
    public int H; // distance from current node to end nodes
    public int F; // G + H - total cost of the path
    public AStarNode Parent;

    public AStarNode(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        AStarNode node = obj as AStarNode;

        if (node == null)
        {
            return false;
        }

        return X == node.X && Y == node.Y;
    }

    public override int GetHashCode()
    {
        return X ^ Y;
    }
}
}
