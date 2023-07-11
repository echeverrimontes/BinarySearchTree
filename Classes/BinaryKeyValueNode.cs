using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace binaryTreeExample.Classes
{
    public class BinaryKeyValueNode<KeyType, ValueType> where KeyType : IComparable<KeyType>
    {
        public KeyType Key { get; set; }
        public ValueType Value { get; set; }
        public BinaryKeyValueNode<KeyType, ValueType> Parent { get; set; }
        public BinaryKeyValueNode<KeyType, ValueType> LeftChild { get; set; }
        public BinaryKeyValueNode<KeyType, ValueType> RightChild { get; set; }
        public BinaryKeyValueNode(KeyType key, ValueType value)
        {
            Value = value;
            Key = key;
            LeftChild = null;
            RightChild = null;
        }


    }
}
