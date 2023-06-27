using Rhino;
using Rhino.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using binaryTreeExample.Classes;
using Eto.Forms;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using Command = Rhino.Commands.Command;

namespace binaryTreeExample.Commands
{
    public class QueuEventPoints : Command
    {
        static QueuEventPoints _instance;

        public QueuEventPoints()
        {
            _instance = this;
        }

        ///<summary>The only instance of the QueuEventPoints command.</summary>
        public static QueuEventPoints Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "QueuEventPoints"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            //1. initialize the binary tree that is going to allocate the appearance of the segments
            BinarySearchTreeKV<double, Curve> T = new BinarySearchTreeKV<double, Curve>();

            //2. set of segments for which we want to compute all intersections
            List<Curve> S = UsefulFunctions.SelectCurve();
            int numberSegments = S.Count;

            //3. initialize the queue for the event points
            //plane sweep algorithm and the line l sweep line
            //the status of the sweep line is the set of segments intersecting it
            //only at particular points is and update of the status needed: event points (in this algorithm the end points of the segments) 
            Queue<Point3d> Q = new Queue<Point3d>();
            Q = UsefulFunctions.EventPointQueue(S, doc); // and ordered queue according to the y coordinate - regarding the sweep line

            RhinoApp.WriteLine();
            RhinoApp.Write("Q: {0} ", Q.Count.ToString());
            foreach (Point3d pt in Q)
            {
                double qX = Math.Round(pt.X, 2);
                double qY = Math.Round(pt.Y, 2);
                RhinoApp.Write(" (" + qX.ToString() + ", " + qY.ToString() + ") ");
            }

            //5. populate the binary tree T with reference to the queue Q in a dynamic way while following the sweep line

            int j = 0;
            List<Point3d> intPoints = new List<Point3d>();

            //U(p) segments whose upper end point is p
            List<BinaryKeyValueNode<double, Curve>> U = new List<BinaryKeyValueNode<double, Curve>>();
            //L(p) segments whose lower end point is p
            List<BinaryKeyValueNode<double, Curve>> L = new List<BinaryKeyValueNode<double, Curve>>();
            //C(p) segments that contain p in their interior
            List<BinaryKeyValueNode<double, Curve>> C = new List<BinaryKeyValueNode<double, Curve>>();

            while (Q.Count > 0) //event queue - sweep line
            {
                //status structure T is an ordered sequence of segments intersecting the sweep line at each moment
                //to access the neighbors of a given segment S so that they can be tested for intersection
                //the status structure must be dynamic: inserted and deleted segments as they appear and disappear

                Point3d temp = Q.Dequeue();
                Point3d tempNext = Q.Peek();
                foreach (Curve crv in S)
                {
                    // insert segments at start point and test intersection with adjacent segments in T
                    if (Math.Round(crv.PointAtStart.Y, 2) == Math.Round(temp.Y, 2))
                    {
                        BinaryKeyValueNode<double, Curve> node = new BinaryKeyValueNode<double, Curve>(temp.X, crv);
                        U.Add(node);
                        T.Insert(temp.X, crv);
                        doc.Objects.AddPoint(temp);
                        // event points known beforehand / intersections points computed in the fly
                        // intersection points: test on the two segments immediately left & right
                        intPoints = HandleEventPoint.IntersectionPt(temp, T, doc);
                        if (intPoints != null)
                        {
                            foreach (Point3d pt in intPoints)
                            {
                                
                                Q.Enqueue(pt);  
                            }
                        }
                    }

                    // delete segments at end points
                    if (Math.Round(crv.PointAtEnd.Y, 2) == Math.Round(temp.Y, 2))
                    {
                        BinaryKeyValueNode<double, Curve> node = new BinaryKeyValueNode<double, Curve>(temp.X, crv);
                        L.Add(node);
                        doc.Objects.AddPoint(temp);
                        T.DeleteNode(node);
                    }
                }
                doc.Views.Redraw();
            }

            RhinoApp.WriteLine();
            int depth = T.Count;
            RhinoApp.WriteLine(depth.ToString());
            RhinoApp.WriteLine("InOrder Traversal:");
            IEnumerable inOrder = T.TraverseTree(BinarySearchTreeKV<double, Curve>.DepthFirstTraversalMethod.InOrder);
            foreach (Double i in inOrder)
            {
                RhinoApp.WriteLine("(" + Math.Round(i, 2).ToString()+", " + T.FindFirst(i) + ")"); //extract the value (Curve at this node)
            }

            RhinoApp.WriteLine();
            RhinoApp.WriteLine("PostOrder Traversal:");
            IEnumerable postOrder = T.TraverseTree(BinarySearchTreeKV<double, Curve>.DepthFirstTraversalMethod.PostOrder);
            foreach (Double i in postOrder)
            {
                RhinoApp.WriteLine("(" + Math.Round(i, 2).ToString() + ", " + T.FindFirst(i) + ")"); //extract the value (Curve at this node)
            }

            RhinoApp.WriteLine();
            RhinoApp.WriteLine("PreOrder:");
            IEnumerable preOrder = T.TraverseTree(BinarySearchTreeKV<double, Curve>.DepthFirstTraversalMethod.PreOrder);
            foreach (Double i in preOrder)
            {
                RhinoApp.WriteLine("(" + Math.Round(i, 2).ToString() + ", " + T.FindFirst(i) + ")"); //extract the value (Curve at this node)
            }

            RhinoApp.WriteLine();
            RhinoApp.Write("Q: {0} ", Q.Count.ToString());
            foreach (Point3d pt in Q)
            {
                double qX = Math.Round(pt.X, 2);
                double qY = Math.Round(pt.Y, 2);
                RhinoApp.Write(" (" + qX.ToString() + ", " + qY.ToString() + ") ");
            }
            
            RhinoApp.WriteLine();

            RhinoApp.Write("intPoints: ");

            foreach (Point3d pt in intPoints)
            {
                double ptX = Math.Round(pt.X, 2);
                double ptY = Math.Round(pt.Y, 2);
                RhinoApp.Write("(" + ptX.ToString() + "," + ptY.ToString() + ") ");

                doc.Objects.AddPoint(pt);
            }
            
            doc.Views.Redraw();

            return Result.Success;
        }
    }
}