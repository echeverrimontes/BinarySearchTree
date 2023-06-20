using Rhino;
using Rhino.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using binaryTreeExample.Classes;
using Eto.Forms;
using Rhino.Geometry;
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
            Q = UsefulFunctions.SweepLineDomain(S, doc); // and ordered queue according to the y coordinate - regarding the sweep line

            RhinoApp.WriteLine();
            RhinoApp.Write("Q: {0} ", Q.Count.ToString());
            foreach (Point3d pt in Q)
            {
                double qX = Math.Round(pt.X, 2);
                double qY = Math.Round(pt.Y, 2);
                RhinoApp.Write(" (" + qX.ToString() + ", " + qY.ToString() + ") ");
            }

            //5. populate the binary tree T with reference to the queue Q in a dynamic way while following the sweep line

            List<Point3d> intPoints = new List<Point3d>();
            for (int j = 0; j < Q.Count; j++) //event queue - sweep line
            {
                //status structure T is an ordered sequence of segments intersecting the sweep line at each moment
                //to access the neighbours of a given segment S so that they can be tested for intersection
                //the status structure must be dynamic: inserted and deleted segments as they appear and disappear

                Point3d temp = Q.Dequeue();
                Curve tempCrv = S[j];
                T.Insert(temp.X, tempCrv);
                Point3d intPoint = HandleEventPoint.IntersectionPt(temp, T, tempCrv);
                if (intPoint != null)
                {
                    intPoints.Add(intPoint);
                }
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

            RhinoApp.Write("intPoints: {0} ", intPoints.Count.ToString());
            /*
            foreach (Point3d pt in intPoints)
            {
                double ptX = Math.Round(pt.X, 2);
                double ptY = Math.Round(pt.Y, 2);
                RhinoApp.Write("(" + ptX.ToString() + "," + ptY.ToString() + ") ");
                doc.Objects.AddPoint(pt);
            }
            */
            doc.Views.Redraw();

            return Result.Success;
        }
    }
}