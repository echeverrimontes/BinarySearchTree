using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace binaryTreeExample.Classes
{
    public class BinaryKeyValueNode<Tkey, Tvalue> where Tkey : IComparable<Tkey>
    {
        public double Key { get; set; }
        public Curve Value { get; set; }
        public BinaryKeyValueNode<double, Curve> Parent { get; set; }
        public BinaryKeyValueNode<double, Curve> LeftChild { get; set; }
        public BinaryKeyValueNode<double, Curve> RightChild { get; set; }
        public BinaryKeyValueNode(double key, Curve value)
        {
            Value = value;
            Key = key;
        }


    }
}
