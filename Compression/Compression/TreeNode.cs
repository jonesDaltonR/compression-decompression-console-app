using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compression
{
    class TreeNode<T> : IComparable
    {
        private T el;
        private TreeNode<T> left;
        private TreeNode<T> right;

        public TreeNode(T element)
        {
            el = element;
            left = null;
            right = null;
        }
        public TreeNode()
        {
        }

        public T Element
        {
            get
            {
                return el;
            }
            set
            {
                el = value;
            }
        }

        public TreeNode<T> Left
        {
            get
            {
                return left;
            }
            set
            {
                left = value;
            }
        }

        public TreeNode<T> Right
        {
            get
            {
                return right;
            }
            set
            {
                right = value;
            }
        }

        public Boolean isLeaf
        {
            get
            {
                return left == null && right == null;
            }
        }

        public int CompareTo(object obj)
        {
            return (el as CharacterFrequency).CompareTo((obj as TreeNode<CharacterFrequency>).Element);
        }
    }
}
