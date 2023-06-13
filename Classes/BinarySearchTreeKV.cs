using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;

namespace binaryTreeExample.Classes
{
    public class BinarySearchTreeKV<TKey, TValue> where TKey : IComparable<TKey>
    {
        // binary tree (max. 2 childs per node) with every node a key and
        // associated value. Also a BST has the property that for every node,
        // the left subtree contains only nodes with a smaller (or equal) key and
        // the right subtree contains only nodes with strictly larger keys. 

        //1.Variables
        private Random random;

        //2.Properties
        private BinaryKeyValueNode<TKey, TValue> Root { get; set; }

        public int Count { get; protected set; }

        //3.constructor
        public BinarySearchTreeKV()
        {
            Root = null;
            random = new Random(1);
            Count = 0;

        }

        //4.functions
        public void Insert(TKey key, TValue value)
        {
            BinaryKeyValueNode<TKey, TValue> parent = null;
            BinaryKeyValueNode<TKey, TValue> current = Root;
            int compare = 0;
            while (current != null)
            {
                parent = current;
                compare = current.Key.CompareTo(key);
                current = compare < 0 ? current.RightChild : current.LeftChild;
            }
            BinaryKeyValueNode<TKey, TValue> newNode = new BinaryKeyValueNode<TKey, TValue>(key, value);
            if (parent != null)
                if (compare < 0)
                    parent.RightChild = newNode;
                else
                    parent.LeftChild = newNode;
            else
                Root = newNode;
            newNode.Parent = parent;
            Count++;
        }

        public TValue FindFirst(TKey key)
        {
            return FindNode(key).Value;
        }

        public TValue FindFirstOrDefault(TKey key)
        {
            var node = FindNode(key, false);
            return node == null ? default(TValue) : node.Value;
        }

        private BinaryKeyValueNode<TKey, TValue> FindNode(TKey key, bool ExceptionIfKeyNotFound = true)
        {
            BinaryKeyValueNode<TKey, TValue> current = Root;
            while (current != null)
            {
                int compare = current.Key.CompareTo(key);
                if (compare == 0)
                    return current;
                if (compare < 0)
                    current = current.RightChild;
                else
                    current = current.LeftChild;
            }
            if (ExceptionIfKeyNotFound)
                throw new KeyNotFoundException();
            else
                return null;
        }

        private void DeleteNode(BinaryKeyValueNode<TKey, TValue> node)
        {
            if (node == null)
                throw new ArgumentNullException();
            if (node.LeftChild != null && node.RightChild != null) //2 childs  
            {
                BinaryKeyValueNode<TKey, TValue> replaceBy = random.NextDouble() > .5 ? InOrderSuccesor(node) : InOrderPredecessor(node);
                DeleteNode(replaceBy);
                node.Value = replaceBy.Value;
                node.Key = replaceBy.Key;
            }
            else //1 or less childs  
            {
                var child = node.LeftChild == null ? node.RightChild : node.LeftChild;
                if (node.Parent.RightChild == node)
                    node.Parent.RightChild = child;
                else
                    node.Parent.LeftChild = child;
            }
            Count--;
        }

        private BinaryKeyValueNode<TKey, TValue> InOrderSuccesor(BinaryKeyValueNode<TKey, TValue> node)
        {
            BinaryKeyValueNode<TKey, TValue> succesor = node.RightChild;
            while (succesor.LeftChild != null)
                succesor = succesor.LeftChild;
            return succesor;
        }

        private BinaryKeyValueNode<TKey, TValue> InOrderPredecessor(BinaryKeyValueNode<TKey, TValue> node)
        {
            BinaryKeyValueNode<TKey, TValue> succesor = node.LeftChild;
            while (succesor.RightChild != null)
                succesor = succesor.RightChild;
            return succesor;
        }

        public IEnumerable<TKey> TraverseTree(DepthFirstTraversalMethod method)
        {
            return TraverseNode(Root, method);
        }

        private IEnumerable<TKey> TraverseNode(BinaryKeyValueNode<TKey, TValue> node, DepthFirstTraversalMethod method)
        {
            IEnumerable<TKey> TraverseLeft = node.LeftChild == null ? new TKey[0] : TraverseNode(node.LeftChild, method),
                TraverseRight = node.RightChild == null ? new TKey[0] : TraverseNode(node.RightChild, method),
                Self = new TKey[1] { node.Key };
            switch (method)
            {
                case DepthFirstTraversalMethod.PreOrder:
                    //string i = Self.Concat(TraverseLeft).Concat(TraverseRight).ToString();
                    //RhinoApp.WriteLine(Self.ToString());
                    return Self.Concat(TraverseLeft).Concat(TraverseRight);
                    //return Self.Concat(TraverseLeft).Concat(TraverseRight);
                case DepthFirstTraversalMethod.InOrder:
                    return TraverseLeft.Concat(Self).Concat(TraverseRight);
                case DepthFirstTraversalMethod.PostOrder:
                    return TraverseLeft.Concat(TraverseRight).Concat(Self);
                default:
                    throw new ArgumentException();
            }
        }

        public enum DepthFirstTraversalMethod
        {
            PreOrder,
            InOrder,
            PostOrder
        }

    }
}
