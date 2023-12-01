using System;
using System.Collections.Generic;
using System.Linq;

public class SplayTree<T> : ICloneable
{
    private class Node
    {
        public T Value;
        public Node Left, Right;

        public Node(T value)
        {
            Value = value;
            Left = Right = null;
        }
    }

    private Node root;

    // Part A: Re-implementation

    // Inserts an item into the SplayTree and splays the last accessed node to the root.
    public void Insert(T item)
    {
        root = InsertRecursive(root, item);
        Splay(Access(item));
    }

    // Recursive method to insert a new item into the SplayTree
    private Node InsertRecursive(Node node, T item)
    {
        if (node == null)
            return new Node(item);

        int compareResult = Comparer<T>.Default.Compare(item, node.Value);

        if (compareResult < 0)
            node.Left = InsertRecursive(node.Left, item);
        else if (compareResult > 0)
            node.Right = InsertRecursive(node.Right, item);

        return node;
    }

    // Removes an item from the SplayTree and splays the maximum node in the left subtree to the root.
    public void Remove(T item)
    {
        Splay(Access(item));
        root = RemoveRecursive(root, item);
    }

    // Recursive method to remove an item from the SplayTree
    private Node RemoveRecursive(Node node, T item)
    {
        if (node == null)
            return null;

        int compareResult = Comparer<T>.Default.Compare(item, node.Value);

        if (compareResult < 0)
            node.Left = RemoveRecursive(node.Left, item);
        else if (compareResult > 0)
            node.Right = RemoveRecursive(node.Right, item);
        else
        {
            if (node.Left == null)
                return node.Right;
            else if (node.Right == null)
                return node.Left;

            node.Value = FindMax(node.Left).Value;
            node.Left = RemoveRecursive(node.Left, node.Value);
        }

        return node;
    }

    // Finds the maximum value node in a subtree
    private Node FindMax(Node node)
    {
        while (node.Right != null)
            node = node.Right;

        return node;
    }

    // Checks if the SplayTree contains a specified item and splays the last accessed node to the root.
    public bool Contains(T item)
    {
        Splay(Access(item));
        return root != null && Comparer<T>.Default.Compare(item, root.Value) == 0;
    }

    // Returns the access path in reverse order from the root to the last accessed node.
    private Stack<Node> Access(T item)
    {
        Stack<Node> path = new Stack<Node>();
        AccessRecursive(root, item, path);
        return path;
    }

    // Recursive method to build the access path
    private void AccessRecursive(Node node, T item, Stack<Node> path)
    {
        if (node == null)
            return;

        int compareResult = Comparer<T>.Default.Compare(item, node.Value);
        path.Push(node);

        if (compareResult < 0)
            AccessRecursive(node.Left, item, path);
        else if (compareResult > 0)
            AccessRecursive(node.Right, item, path);
    }

    // Splays a node to the root based on the access path
    private void Splay(Stack<Node> path)
    {
        while (path.Count > 1)
        {
            Node current = path.Pop();
            Node parent = path.Peek();
            Node grandparent = path.Count >= 2 ? path.ElementAt(path.Count - 2) : null;

            if (grandparent != null)
            {
                bool zigZig = (current == parent.Left && parent == grandparent.Left) ||
                               (current == parent.Right && parent == grandparent.Right);

                if (zigZig)
                    ZigZig(current, parent, grandparent);
                else
                    ZigZag(current, parent, grandparent);
            }
            else
            {
                Zig(current, parent);
            }
        }
    }

    // Performs the Zig operation
    private void Zig(Node current, Node parent)
    {
        if (current == parent.Left)
            RotateRight(parent);
        else
            RotateLeft(parent);
    }

    // Performs the Zig-Zig operation
    private void ZigZig(Node current, Node parent, Node grandparent)
    {
        if (current == parent.Left && parent == grandparent.Left)
        {
            RotateRight(grandparent);
            RotateRight(parent);
        }
        else if (current == parent.Right && parent == grandparent.Right)
        {
            RotateLeft(grandparent);
            RotateLeft(parent);
        }
    }

    // Performs the Zig-Zag operation
    private void ZigZag(Node current, Node parent, Node grandparent)
    {
        if (current == parent.Left && parent == grandparent.Right)
        {
            RotateRight(parent);
            RotateLeft(grandparent);
        }
        else if (current == parent.Right && parent == grandparent.Left)
        {
            RotateLeft(parent);
            RotateRight(grandparent);
        }
    }

    // Performs the right rotation
    private void RotateRight(Node pivot)
    {
        Node parent = pivot.Left;
        pivot.Left = parent.Right;
        parent.Right = pivot;
        UpdateParent(pivot, parent);
    }

    // Performs the left rotation
    private void RotateLeft(Node pivot)
    {
        Node parent = pivot.Right;
        pivot.Right = parent.Left;
        parent.Left = pivot;
        UpdateParent(pivot, parent);
    }

    // Updates the parent of a node after a rotation
    private void UpdateParent(Node original, Node replacement)
    {
        Node parent = FindParent(original);

        if (parent == null)
            root = replacement;
        else if (original == parent.Left)
            parent.Left = replacement;
        else
            parent.Right = replacement;
    }

    // Finds the parent of a node in the SplayTree
    private Node FindParent(Node node)
    {
        if (node == null || node == root)
            return null;

        Node parent = null;
        Node current = root;

        while (current != null && current != node)
        {
            parent = current;
            if (Comparer<T>.Default.Compare(node.Value, current.Value) < 0)
                current = current.Left;
            else
                current = current.Right;
        }

        return parent;
    }

    // Part B: Deep Copy

    // Creates a deep copy of the current SplayTree using a preorder traversal.
    public object Clone()
    {
        SplayTree<T> clonedTree = new SplayTree<T>();
        clonedTree.root = CloneRecursive(root);
        return clonedTree;
    }

    // Recursive method to clone a subtree
    private Node CloneRecursive(Node node)
    {
        if (node == null)
            return null;

        Node clonedNode = new Node(node.Value);
        clonedNode.Left = CloneRecursive(node.Left);
        clonedNode.Right = CloneRecursive(node.Right);

        return clonedNode;
    }

    // Checks if a tree is an exact copy of the current SplayTree.
    public override bool Equals(object other)
    {
        if (other is SplayTree<T> tree)
            return Equals(root, tree.root);

        return false;
    }

    // Recursive method to check equality of subtrees
    private bool Equals(Node node1, Node node2)
    {
        if (node1 == null && node2 == null)
            return true;
        if (node1 == null || node2 == null)
            return false;

        return EqualityComparer<T>.Default.Equals(node1.Value, node2.Value) &&
               Equals(node1.Left, node2.Left) &&
               Equals(node1.Right, node2.Right);
    }

    // Part C: Undo

    // Undoes the last successful insertion in the SplayTree.
    public SplayTree<T> Undo()
    {
        if (root == null)
            return this; // No operation needed if the tree is empty

        SplayTree<T> clonedTree = (SplayTree<T>)Clone();
        clonedTree.root = UndoRecursive(clonedTree.root);

        return clonedTree;
    }

    // Recursive method to undo the last insertion
    private Node UndoRecursive(Node node)
    {
        if (node == null)
            return null;

        Node left = UndoRecursive(node.Left);
        Node right = UndoRecursive(node.Right);

        // If the node is in the access path of the last insertion, splay it to the root
        if (left != null && left.Value.Equals(node.Value))
        {
            node.Left = left;
            Splay(UndoAccess(node.Value));
            return node.Right; // The node is moved to the root
        }
        else if (right != null && right.Value.Equals(node.Value))
        {
            node.Right = right;
            Splay(UndoAccess(node.Value));
            return node.Left; // The node is moved to the root
        }

        node.Left = left;
        node.Right = right;
        return node;
    }

    // Builds the access path in reverse order for undoing the last insertion
    private Stack<Node> UndoAccess(T item)
    {
        Stack<Node> path = new Stack<Node>();
        UndoAccessRecursive(root, item, path);
        return path;
    }

    // Recursive method to undo the last insertion
    private void UndoAccessRecursive(Node node, T item, Stack<Node> path)
    {
        if (node == null)
            return;

        int compareResult = Comparer<T>.Default.Compare(item, node.Value);
        path.Push(node);

        if (compareResult < 0)
            UndoAccessRecursive(node.Left, item, path);
        else if (compareResult > 0)
            UndoAccessRecursive(node.Right, item, path);
    }

    // Main method
    class Program
    {
        static void Main()
        {
            // Create a new SplayTree of integers
            SplayTree<int> splayTree = new SplayTree<int>();

            // Part A: Test each of the re-implemented methods of the splay tree.
            Console.WriteLine("Part A: Re-implementation Test");

            // Insert and Splay Test
            Console.WriteLine("\nInsert and Splay Test:");
            splayTree.Insert(10);
            PrintTree(splayTree);

            splayTree.Insert(5);
            PrintTree(splayTree);

            splayTree.Insert(15);
            PrintTree(splayTree);

            // Contains and Splay Test
            Console.WriteLine("\nContains and Splay Test:");
            bool contains5 = splayTree.Contains(5);
            Console.WriteLine($"Contains 5: {contains5}");
            PrintTree(splayTree);

            // Remove and Splay Test
            Console.WriteLine("\nRemove and Splay Test:");
            splayTree.Remove(10);
            PrintTree(splayTree);

            // Part C: Undo Test
            Console.WriteLine("\nPart C: Undo Test");
            splayTree.Insert(20);
            Console.WriteLine("Tree Structure after Insert:");
            PrintTree(splayTree);

            splayTree.Undo();
            Console.WriteLine("Tree Structure after Undo:");  // Print the tree structure after the undo operation
            PrintTree(splayTree);

            // Part B: Verify using the Equals method that the Clone method produces an exact copy.
            Console.WriteLine("\nPart B: Clone and Equals Test");
            SplayTree<int> clonedTree = (SplayTree<int>)splayTree.Clone();
            bool equalsOriginal = splayTree.Equals(clonedTree);
            Console.WriteLine($"Original equals Clone: {equalsOriginal}");

            // Part C: Perform a successful Insert followed by the Undo method.
            Console.WriteLine("\nPart C: Undo Test");
            splayTree.Insert(20);
            Console.WriteLine("Tree Structure after Insert:");
            PrintTree(splayTree);

            splayTree.Undo();
            Console.WriteLine("Tree Structure after Undo:");
            PrintTree(splayTree);
        }

        // Helper method to print the tree structure
        static void PrintTree(SplayTree<int> tree)
        {
            Console.WriteLine("Tree Structure:");
            PrintTreeRecursive(tree.root, 0);
            Console.WriteLine();
        }

        // Recursive method to print the tree structure
        static void PrintTreeRecursive(SplayTree<int>.Node node, int depth)
        {
            if (node != null)
            {
                PrintTreeRecursive(node.Right, depth + 1);
                Console.WriteLine($"{new string(' ', depth * 4)}{node.Value}");
                PrintTreeRecursive(node.Left, depth + 1);
            }
        }
    }
}
