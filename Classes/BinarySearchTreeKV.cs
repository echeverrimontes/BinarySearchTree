using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Geometry;

namespace binaryTreeExample.Classes
{
    public class BinarySearchTreeKV<TKey, TValue> where TKey : IComparable<double>
    {
        // binary tree (max. 2 childs per node) with every node a key and
        // associated value. Also a BST has the property that for every node,
        // the left subtree contains only nodes with a smaller (or equal) key and
        // the right subtree contains only nodes with strictly larger keys. 

        //1.Variables
        private Random random;

        //2.Properties
        private BinaryKeyValueNode<double, Curve> Root { get; set; }

        public int Count { get; protected set; }

        //3.constructor
        public BinarySearchTreeKV()
        {
            Root = null;
            random = new Random(1);
            Count = 0;

        }

        //4.functions
        public void Insert(double key, Curve value)
        {
            BinaryKeyValueNode<double, Curve> newNode = new BinaryKeyValueNode<double, Curve>(key, value);
            BinaryKeyValueNode<double, Curve> tempParent = null;
            // case 1: insert first node
            if (Root == null)
            {
                Root = newNode;
            }
            // case 2: key < root.Key -> leftChild, key > root.Key -> rightChild
            else
            {
                BinaryKeyValueNode<double, Curve> current = Root;
                while (true)
                {
                    tempParent = current;
                    if (Math.Round(newNode.Key) < Math.Round(tempParent.Key))
                    {
                        current = current.LeftChild;
                        if (current == null)
                        {
                            tempParent.LeftChild = newNode;
                            newNode.Parent = tempParent;
                            return;
                        }
                    }
                    else
                    {
                        current = current.RightChild;
                        if (current == null)
                        {
                            tempParent.RightChild = newNode;
                            newNode.Parent = tempParent;
                            return;
                        }
                    }

                }
            }

            Count++;
        }

        public Curve FindFirst(double key)
        {
            return FindNode(key).Value;
        }

        public Curve FindFirstOrDefault(double key)
        {
            var node = FindNode(key, false);
            return node == null ? default(Curve) : node.Value;
        }

        public BinaryKeyValueNode<double, Curve> FindNode(double key, bool ExceptionIfKeyNotFound = true)
        {

            BinaryKeyValueNode<double, Curve> current = Root;
            
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

        public void DeleteNode(BinaryKeyValueNode<double, Curve> node)
        {
            // if the tree is empty
            if (node == null)
                throw new ArgumentNullException();

            // case 1: the node to be deleted is a leaf node - simply delete the node
            if (node.LeftChild == null && node.RightChild == null)
            {
                //DeleteNode(node);
            }
            // case 2: the node to be deleted has a single child node
            // a. replace the node with its child node
            // b. remove the child node from its original position
            else 
            {
                if (node.LeftChild == null)
                {
                    BinaryKeyValueNode<double, Curve> child = node.RightChild;
                    node.Parent.RightChild = child;
                }
                else if (node.RightChild == null)
                {
                    BinaryKeyValueNode<double, Curve> child = node.LeftChild;
                    node.Parent.LeftChild = child;
                }

                DeleteNode(node);
            }
            // case 3: the node to be deleted has two children:
            // a. get the inorder successor of that node
            // b. replace the node with the inorder successor
            // c. remove the inorder successor from its original position
            if (node.LeftChild != null && node.RightChild != null)  
            {
                BinaryKeyValueNode<double, Curve> replaceBy = random.NextDouble() > .5 ? InOrderSuccesor(node) : InOrderPredecessor(node);
                DeleteNode(replaceBy);
                node.Value = replaceBy.Value;
                node.Key = replaceBy.Key;
            }
            
            Count--;
        }

        private BinaryKeyValueNode<double, Curve> InOrderSuccesor(BinaryKeyValueNode<double, Curve> node)
        {
            BinaryKeyValueNode<double, Curve> succesor = node.RightChild;
            while (succesor.LeftChild != null)
                succesor = succesor.LeftChild;
            return succesor;
        }

        private BinaryKeyValueNode<double, Curve> InOrderPredecessor(BinaryKeyValueNode<double, Curve> node)
        {
            BinaryKeyValueNode<double, Curve> succesor = node.LeftChild;
            while (succesor.RightChild != null)
                succesor = succesor.RightChild;
            return succesor;
        }

        public IEnumerable<double> TraverseTree(DepthFirstTraversalMethod method)
        {
            return TraverseNode(Root, method);
        }

        private IEnumerable<double> TraverseNode(BinaryKeyValueNode<double, Curve> node, DepthFirstTraversalMethod method)
        {
            IEnumerable<double> TraverseLeft = node.LeftChild == null ? new double[0] : TraverseNode(node.LeftChild, method),
                TraverseRight = node.RightChild == null ? new double[0] : TraverseNode(node.RightChild, method),
                Self = new double[1] { node.Key };
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
