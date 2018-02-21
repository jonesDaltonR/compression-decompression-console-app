using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compression
{

    public enum Relative
    {
        LeftChild, RightChild, Parent, Root
    }

    class BinaryTree<T>
    {


        private TreeNode<T> root;
        private TreeNode<T> current;
        private LinkedList<BinaryPath<T>> binaryTable = new LinkedList<BinaryPath<T>>();
        private string binarystring;

        public BinaryTree()
        {
            root = null;
            current = null;
            binarystring = null;
        }

        public TreeNode<T> Current
        {
            get
            {
                return current;
            }
        }

        public TreeNode<T> Root
        {
            get
            {
                return root;
            }
        }

        public Boolean isEmpty
        {
            get
            {
                return root == null;
            }
        }

        public Boolean moveTo(Relative relative)
        {
            Boolean found = false;


            switch (relative)
            {
                case Relative.LeftChild:
                    if (current.Left != null)
                    {
                        current = current.Left;
                        found = true;
                    }

                    break;

                case Relative.RightChild:
                    if (current.Right != null)
                    {
                        current = current.Right;
                        found = true;
                    }
                    break;

                case Relative.Root:
                    if (root != null)
                    {
                        current = root;
                        found = true;
                    }
                    break;

                case Relative.Parent:
                    if (current != root)
                    {
                        current = findParent(current);
                        found = true;
                    }
                    break;
            }

            return found;
        }

        public TreeNode<T> findParent(TreeNode<T> node)
        {
            Stack<TreeNode<T>> s = new Stack<TreeNode<T>>();
            TreeNode<T> n = root;

            while (n.Left != node && n.Right != node)
            {
                if (n.Right != null)
                    s.Push(n.Right);

                if (n.Left != null)
                    n = n.Left;
                else
                    n = s.Pop();
            }
            s.Clear();
            return n;
        }

        public Boolean Insert(T value, Relative relative)
        {
            Boolean inserted = true;
            TreeNode<T> newNode = new TreeNode<T>(value);

            if ((relative == Relative.LeftChild && current.Left != null) ||
                  relative == Relative.RightChild && current.Right != null)
            {
                inserted = false;
            }
            else
            {
                if (relative == Relative.LeftChild)
                {
                    current.Left = newNode;
                }
                else if (relative == Relative.RightChild)
                {
                    current.Right = newNode;
                }
                else if (relative == Relative.Root)
                {
                    if (root == null)
                    {
                        root = newNode;
                    }
                    current = root;
                    inserted = false;
                }
            }

            return inserted;
        }

        public Boolean Insert(TreeNode<T> treeNode, Relative relative)
        {
            Boolean inserted = true;
            TreeNode<T> newNode = treeNode;

            if ((relative == Relative.LeftChild && current.Left != null) ||
                  relative == Relative.RightChild && current.Right != null)
            {
                inserted = false;
            }
            else
            {
                if (relative == Relative.LeftChild)
                {
                    current.Left = newNode;
                }
                else if (relative == Relative.RightChild)
                {
                    current.Right = newNode;
                }
                else if (relative == Relative.Root)
                {
                    if (root == null)
                    {
                        root = newNode;
                    }
                    current = root;
                    inserted = false;
                }
            }

            return inserted;
        }

        public OrderedLinkedList<TreeNode<CharacterFrequency>> TransferListToTree(OrderedLinkedList<TreeNode<CharacterFrequency>> orderedNodeList)
        {
            while (orderedNodeList.Count != 1)
            {
                TreeNode<CharacterFrequency> least = orderedNodeList.GetFirst();
                orderedNodeList.RemoveFirst();
                TreeNode<CharacterFrequency> nextLeast = orderedNodeList.GetFirst();
                orderedNodeList.RemoveFirst();

                TreeNode<CharacterFrequency> combinedLeast = new TreeNode<CharacterFrequency>(new CharacterFrequency('\0', (least.Element.Frequency + nextLeast.Element.Frequency)));
                combinedLeast.Left = least;
                combinedLeast.Right = nextLeast;

                orderedNodeList.Add(combinedLeast);
            }
            return orderedNodeList;
        }

        public LinkedList<BinaryPath<T>> BinaryTable(TreeNode<T> node)
        {
            FindPath(node);
            return binaryTable;
        }

        private void FindPath(TreeNode<T> node)
        {
            if (node != null)
            {
                binarystring += '1';
                FindPath(node.Left);
                binarystring = binarystring.Substring(0, binarystring.Length - 1);
                binaryTable.AddLast(new BinaryPath<T>(node.Element, binarystring));
                binarystring += '0';
                FindPath(node.Right);
                binarystring = binarystring.Substring(0, binarystring.Length - 1);
            }
        }
    }
}
