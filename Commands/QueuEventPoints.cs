using Rhino;
using Rhino.Commands;
using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using binaryTreeExample.Classes;
using Eto.Forms;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using Rhino.Input.Custom;
using Rhino.DocObjects;
using Rhino.Input;
using Rhino.UI;
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
            List<List<Point3d>> intersections = new List<List<Point3d>>();
            List<Point3d> intPts = new List<Point3d>();

            RhinoApp.WriteLine();
            RhinoApp.WriteLine("S: {0} ", S.Count.ToString());
            foreach (Curve crv in S)
            {
                RhinoApp.WriteLine(crv.ToString());
            }

            //3. initialize the queue for the event points 
            Queue<Point3d> Q;
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

                // each node contains attributes left, right, and p that point to the nodes corresponding 
                // to its leftChild, rightChild, and its parent
                // if any of the nodes is missing (child or parent) the attribute contains null

                // keys always stored in such a way sa to satisfy binary-search-tree property:
                // x = node, left subtree: y.key <= x.key; right subtree: y.key >= x.key

                // A. Insert nodes from queu, find intersection points and enqueu:
                Point3d temp = Q.Dequeue();
                double p = Math.Round(temp.X, 2);    
                foreach (Curve crv in S)
                {
                    // A. insert segments at start point, the sweepline actualizes to the event point p
                    if (Math.Round(crv.PointAtStart.Y, 2) == Math.Round(temp.Y, 2))
                    {
                        T.Insert(Math.Round(p, 2), crv); // update sweepline status + curve
                        // AA. Find intersection of lines below event point
                        // Select two curves to proof for intersection from the binary search tree: node / node.LeftChild / node.RightChild
                        //intPts = T.InOrderTreeWalk(T.Root, doc, Q);
                        BinaryKeyValueNode<double, Curve> node = T.Search(Math.Round(p, 2));
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
                                            Q.Enqueue(ccx_event.PointA);
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
                                            Q.Enqueue(ccx_event.PointA);
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
                        }
                    }
                    // B. Delete segment at end point
                    else if (Math.Round(crv.PointAtEnd.Y, 2) == Math.Round(temp.Y, 2))
                    {
                        T.Delete(temp.Y); // update sweepline status - curve
                        // BB. Find intersection of lines below event point
                        // Select two curves to proof for intersection from the binary search tree: node / node.LeftChild / node.RightChild
                        // intPts = T.InOrderTreeWalk(T.Root, doc, Q);
                    }
                }
                /*
                // C. Delete segment at end point
                foreach (Curve crv in S)
                {
                    if (Math.Round(crv.PointAtEnd.Y, 2) == Math.Round(temp.Y, 2))
                    {
                        T.Delete(temp.Y);
                    }
                }
                */
                if (intPts != null)
                {
                    intersections.Add(intPts);
                }
            }

            // D. the binary-search-tree property allows us to print out all the keys in a bst in sorted order
            // by a simple recursive algorithm - inorder tree walk:

            RhinoApp.WriteLine();
            RhinoApp.Write("Q: {0} ", Q.Count.ToString());
            foreach (Point3d pt in Q)
            {
                double qX = Math.Round(pt.X, 2);
                double qY = Math.Round(pt.Y, 2);
                RhinoApp.Write(" (" + qX.ToString() + ", " + qY.ToString() + ") ");
            }

            RhinoApp.WriteLine();
            RhinoApp.Write("intersections: {0} ", intersections.Count.ToString());
            foreach (List<Point3d> pts in intersections)
            {
                foreach (Point3d pt in pts)
                {
                    double qX = Math.Round(pt.X, 2);
                    double qY = Math.Round(pt.Y, 2);
                    RhinoApp.Write(" (" + qX.ToString() + ", " + qY.ToString() + ") ");
                }
            }

            return Result.Success;
        }
    }
}
