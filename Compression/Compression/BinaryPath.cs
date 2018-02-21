using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression
{
    class BinaryPath<T>
    {
        private T val;

        public T Element
        {
            get
            {
                return val;
            }
            set
            {
                val = value;
            }
        }

        public string Path { get; set; }

        public BinaryPath(T element,string path)
        {
            Element = element;
            Path = path;
        }
    }
}
