using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace binaryTreeExample.Classes
{
    class BinaryKeyValueNode<Tkey, Tvalue> where Tkey : IComparable<Tkey>
    {
        public Tkey Key { get; set; }
        public Tvalue Value { get; set; }
        public BinaryKeyValueNode<Tkey, Tvalue> Parent { get; set; }
        public BinaryKeyValueNode<Tkey, Tvalue> LeftChild { get; set; }
        public BinaryKeyValueNode<Tkey, Tvalue> RightChild { get; set; }
        public BinaryKeyValueNode(Tkey key, Tvalue value)
        {
            Value = value;
            Key = key;
        }


    }
}
