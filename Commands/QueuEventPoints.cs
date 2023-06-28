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
            // SweepLine algorithm: the status of the sweep line is the set of segments intersecting it
            // only at particular points is an update of the status required 
            // at event points (in this algorithm: endpoints of the segments)

            //1. initialize the binary tree that is going to allocate the appearance of the segments
            BinarySearchTreeKV<double, Curve> T = new BinarySearchTreeKV<double, Curve>();

            //2. set of segments for which we want to compute all intersections
            List<Curve> S = UsefulFunctions.SelectCurve();
            RhinoApp.WriteLine();
            RhinoApp.WriteLine("S: {0} ", S.Count.ToString());
            foreach (Curve crv in S)
            {
                RhinoApp.WriteLine(crv.ToString());
            }

            //3. initialize the queue for the event points 
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

            //4. populate the binary tree T with reference to the queue Q in a dynamic way while following the sweep line
            while (Q.Count > 0) //event queue - sweep line
            {
                // status structure T is an ordered sequence of segments intersecting the sweep line at each moment
                // to access the neighbors of a given segment S so that they can be tested for intersection
                // the status structure must be dynamic: inserted and deleted segments as they appear and disappear
                
                List<Point3d> intPoints = new List<Point3d>();
                Point3d temp = Q.Dequeue();
                //Point3d tempNext = Q.Peek();
                foreach (Curve crv in S)
                {
                    // insert segments at start point 
                    if (Math.Round(crv.PointAtStart.Y, 2) == Math.Round(temp.Y, 2))
                    {
                        doc.Objects.AddPoint(temp);
                        // BinaryKeyValueNode<double, Curve> node = new BinaryKeyValueNode<double, Curve>(temp.X, crv);
                        T.Insert(temp.X, crv);
                        // intersection points: test on the two segments immediately left & right
                        intPoints = HandleEventPoint.IntersectionPt(temp, T, doc);
                    }
                    else if (Math.Round(crv.PointAtEnd.Y, 2) == Math.Round(temp.Y, 2))
                    {
                        doc.Objects.AddPoint(temp);
                        BinaryKeyValueNode<double, Curve> node = new BinaryKeyValueNode<double, Curve>(temp.X, crv);
                        T.DeleteNode(node);
                    }
                }
            }
            
            RhinoApp.WriteLine();
            RhinoApp.Write("Q: {0} ", Q.Count.ToString());
            foreach (Point3d pt in Q)
            {
                double qX = Math.Round(pt.X, 2);
                double qY = Math.Round(pt.Y, 2);
                RhinoApp.Write(" (" + qX.ToString() + ", " + qY.ToString() + ") ");
            }

            doc.Views.Redraw();
            return Result.Success;
        }
    }
}