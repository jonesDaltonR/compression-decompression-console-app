using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression
{
    public class OrderedLinkedList<T> : IEnumerable<T> where T : IComparable
    {
        //ordered least to greatest

        private LinkedList<T> list = new LinkedList<T>();


        public void Add(T element)
        {
            if (list.Count == 0)
            {
                list.AddFirst(element);
            }
            else if (list.First.Value.CompareTo(element) >= 0)
            {
                list.AddFirst(element);

            }
            else if (list.Last.Value.CompareTo(element) <= 0)
            {
                list.AddLast(element);
            }
            else if (list.Last.Value.CompareTo(element) > 0)
            {
                LinkedListNode<T> node = list.First;

                while (node.Value.CompareTo(element) < 0)
                {
                    node = node.Next;
                }
                list.AddBefore(node, element);

            }
        }

        public void RemoveFirst()
        {
            list.RemoveFirst();
        }

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public T[] GetArray()
        {
            return list.ToArray();
        }

        public T GetFirst()
        {
            return list.First();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
