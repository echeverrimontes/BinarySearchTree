using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using Rhino.Input.Custom;

namespace binaryTreeExample.Classes
{
    public class BinarySearchTreeKV<KeyType, ValueType> where KeyType : IComparable<KeyType>
    {
        // binary tree (max. 2 childs per node) with every node a key and
        // associated value. Also a BST has the property that for every node,
        // the left subtree contains only nodes with a smaller (or equal) key and
        // the right subtree contains only nodes with strictly larger keys. 
        // https://www.c-sharpcorner.com/UploadFile/ddb212/generic-binary-search-tree-with-keyed-values-using-C-Sharp/

        //1.Variables
        private Random random;

        //2.Properties
        public BinaryKeyValueNode<KeyType, ValueType> Root;

        public int Count { get; protected set; }

        //3.constructor
        public BinarySearchTreeKV()
        {
            Root = null; //empty tree declaration of the pointer for Root
            random = new Random(1);
            Count = 0;

        }

        //4.functions
        private BinaryKeyValueNode<KeyType, ValueType> InsertNode(BinaryKeyValueNode<KeyType, ValueType> node, KeyType key, ValueType value)
        {
            if (node == null)
            {
                return new BinaryKeyValueNode<KeyType, ValueType>(key, value);
            }

            int comparison = key.CompareTo(node.Key);
            if (comparison < 0)
            {
                node.LeftChild = InsertNode(node.LeftChild, key, value);
            }
            else if (comparison > 0)
            {
                node.RightChild = InsertNode(node.RightChild, key, value);
            }

            return node;
        }

        public void Insert(KeyType key, ValueType value)
        {
            Root = InsertNode(Root, key, value);
        }

        private BinaryKeyValueNode<KeyType, ValueType> SearchNode(BinaryKeyValueNode<KeyType, ValueType> node, KeyType key)
        {
            if (node == null || key.Equals(node.Key))
            {
                return node != null ? node : default; // Return default if key not found
            }

            int comparison = key.CompareTo(node.Key);
            if (comparison < 0)
            {
                return SearchNode(node.LeftChild, key);
            }

            return SearchNode(node.RightChild, key);
        }

        public BinaryKeyValueNode<KeyType, ValueType> Search(KeyType key)
        {
            return SearchNode(Root, key);
        }

        private BinaryKeyValueNode<KeyType, ValueType> DeleteNode(BinaryKeyValueNode<KeyType, ValueType> node, KeyType key)
        {
            if (node == null)
            {
                return null;
            }

            int comparison = key.CompareTo(node.Key);
            if (comparison < 0)
            {
                node.LeftChild = DeleteNode(node.LeftChild, key);
            }
            else if (comparison > 0)
            {
                node.RightChild = DeleteNode(node.RightChild, key);
            }
            else
            {
                // Node to be deleted found
                if (node.LeftChild == null)
                {
                    return node.RightChild;
                }
                else if (node.RightChild == null)
                {
                    return node.LeftChild;
                }
                else
                {
                    // Node has two children, find in-order successor (smallest key in the right subtree)
                    BinaryKeyValueNode<KeyType, ValueType> successor = FindMinimum(node.RightChild);
                    node.Key = successor.Key;
                    node.Value = successor.Value;
                    node.RightChild = DeleteNode(node.RightChild, successor.Key);
                }
            }

            return node;
        }

        public void Delete(KeyType key)
        {
            Root = DeleteNode(Root, key);
        }

        private BinaryKeyValueNode<KeyType, ValueType> FindMinimum(BinaryKeyValueNode<KeyType, ValueType> node)
        {
            while (node.LeftChild != null)
            {
                node = node.LeftChild;
            }

            return node;
        }

        private void InOrderTraversal(BinaryKeyValueNode<KeyType, ValueType> node, List<ValueType> values)
        {
            if (node != null)
            {
                InOrderTraversal(node.LeftChild, values);
                values.Add(node.Value);
                InOrderTraversal(node.RightChild, values);
            }
        }

        /// <summary>
        /// In Order traversal
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<ValueType> InOrderTraversal(BinaryKeyValueNode<KeyType, ValueType> node)
        {
            List<ValueType> values = new List<ValueType>();
            InOrderTraversal(Root, values);
            return values;
        }

        /// <summary>
        /// InOrder-Tree-Walk for printing out all keys in sorted order
        /// </summary>
        public List<Point3d> InOrderTreeWalk(BinaryKeyValueNode<double, Curve> node, RhinoDoc doc, Queue<Point3d> Q)
        {
            List<Point3d> intPts = new List<Point3d>();
            if (node != null)
            {
                if (node.LeftChild != null)
                {
                    Curve curveA = node.Value;
                    Curve curveL = node.LeftChild.Value;
                    
                    // Calculate the intersection
                    CurveIntersections events1 = Intersection.CurveCurve(curveA, curveL,
                        0.001, 0.00);

                    // Process the results
                    if (events1 != null)
                    {
                        for (int i = 0; i < events1.Count; i++)
                        {
                            IntersectionEvent ccx_event = events1[i];
                            if (intPts.Contains(ccx_event.PointA))
                            {
                                RhinoApp.WriteLine("Already in the list");
                            }
                            else
                            {
                                doc.Objects.AddPoint(ccx_event.PointA);
                                intPts.Add(ccx_event.PointA);
                                //Q.Enqueue(ccx_event.PointA);
                            }

                            if (ccx_event.PointA.DistanceTo(ccx_event.PointB) > double.Epsilon)
                            {
                                doc.Objects.AddPoint(ccx_event.PointB);
                                intPts.Add(ccx_event.PointB);
                                doc.Objects.AddLine(ccx_event.PointA, ccx_event.PointB);
                            }

                        }
                        doc.Views.Redraw();
                    }
                    
                }
                if (node.RightChild != null)
                {
                    Curve curveA = node.Value;
                    Curve curveR = node.RightChild.Value;

                    // Calculate the intersection
                    CurveIntersections events2 = Intersection.CurveCurve(curveA, curveR,
                        0.001, 0.00);
                    // Process the results
                    
                    if (events2 != null)
                    {
                        for (int i = 0; i < events2.Count; i++)
                        {
                            IntersectionEvent ccx_event = events2[i];
                            if (intPts.Contains(ccx_event.PointA))
                            {
                                RhinoApp.WriteLine("Already in the list");
                            }
                            else
                            {
                                doc.Objects.AddPoint(ccx_event.PointA);
                                intPts.Add(ccx_event.PointA);
                                //Q.Enqueue(ccx_event.PointA);
                            }

                            if (ccx_event.PointA.DistanceTo(ccx_event.PointB) > double.Epsilon)
                            {
                                doc.Objects.AddPoint(ccx_event.PointB);
                                intPts.Add(ccx_event.PointB);
                                doc.Objects.AddLine(ccx_event.PointA, ccx_event.PointB);
                            }

                        }
                        doc.Views.Redraw();
                    }
                }
                InOrderTreeWalk(node.LeftChild, doc, Q);
                RhinoApp.WriteLine(node.Key.ToString());
                InOrderTreeWalk(node.RightChild, doc, Q);
            }

            return intPts;
        }

        public Point3d IntersectionPt(BinaryKeyValueNode<double, Curve> node, RhinoDoc doc)
        {
            Point3d intPt = new Point3d();
            if (node != null)
            {
                if (node.LeftChild != null)
                {
                    Curve curveA = node.Value;
                    Curve curveL = node.LeftChild.Value;

                    // Calculate the intersection
                    CurveIntersections events1 = Intersection.CurveCurve(curveA, curveL,
                        0.001, 0.00);

                    // Process the results
                    if (events1 != null)
                    {
                        for (int i = 0; i < events1.Count; i++)
                        {
                            IntersectionEvent ccx_event = events1[i];
                            doc.Objects.AddPoint(ccx_event.PointA);
                            intPt = ccx_event.PointA;
                            if (ccx_event.PointA.DistanceTo(ccx_event.PointB) > double.Epsilon)
                            {
                                doc.Objects.AddPoint(ccx_event.PointB);
                                intPt = ccx_event.PointB;
                                doc.Objects.AddLine(ccx_event.PointA, ccx_event.PointB);
                            }
                        }
                        doc.Views.Redraw();
                    }

                }
                if (node.RightChild != null)
                {
                    Curve curveA = node.Value;
                    Curve curveR = node.RightChild.Value;

                    // Calculate the intersection
                    CurveIntersections events2 = Intersection.CurveCurve(curveA, curveR,
                        0.001, 0.00);
                    // Process the results

                    if (events2 != null)
                    {
                        for (int i = 0; i < events2.Count; i++)
                        {
                            IntersectionEvent ccx_event = events2[i];
                            doc.Objects.AddPoint(ccx_event.PointA);
                            intPt = ccx_event.PointA;
                            if (ccx_event.PointA.DistanceTo(ccx_event.PointB) > double.Epsilon)
                            {
                                doc.Objects.AddPoint(ccx_event.PointB);
                                intPt = ccx_event.PointB;
                                doc.Objects.AddLine(ccx_event.PointA, ccx_event.PointB);
                            }
                        }
                        doc.Views.Redraw();
                    }
                }
            }

            return intPt;
        }

        /*
        public void DeleteNode(BinaryKeyValueNode<KeyType, ValueType> node)
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
                    BinaryKeyValueNode<KeyType, ValueType> child = node.RightChild;
                    node.Parent.RightChild = child;
                }
                else if (node.RightChild == null)
                {
                    BinaryKeyValueNode<KeyType, ValueType> child = node.LeftChild;
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
                BinaryKeyValueNode<KeyType, ValueType> replaceBy = random.NextDouble() > .5 ? InOrderSuccesor(node) : InOrderPredecessor(node);
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
        */
    }
}
