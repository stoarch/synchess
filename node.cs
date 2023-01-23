namespace SyncChess
{
    public class Node
    {
        public int X { get; set; } // x coordinate of the node
        public int Y { get; set; } // y coordinate of the node
        public float G { get; set; } // cost to move from the start node to this node
        public float H { get; set; } // estimated cost to move from this node to the goal node
        public float F { get; set; } // total cost of this node (F = G + H)
        public Node Parent { get; set; } // parent node of this node in the final path

        public Node(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Node node = (Node)obj;
            return X == node.X && Y == node.Y;
        }
    }
}
