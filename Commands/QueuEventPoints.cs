﻿using Rhino;
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
            List<Curve> U = new List<Curve>();
            List<Curve> L = new List<Curve>();
            List<Curve> C = new List<Curve>();

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
                    Point3d ptA = new Point3d();
                    List<Point3d> intPt = new List<Point3d>();

                    // insert segments at start point, the sweepline actualizes to the event point p
                    if (Math.Round(crv.PointAtStart.Y, 2) == Math.Round(temp.Y, 2))
                    {
                        doc.Objects.AddPoint(temp);
                        U.Add(crv);
                        T.Insert(Math.Round(p, 2), crv);

                        // Find all segments stored in T that contain p
                        // if p = lower end point 
                        if (Math.Round(crv.PointAtEnd.Y, 2) == Math.Round(temp.Y, 2))
                        {
                            L.Add(crv);
                        }
                        // if p = crv that contain p
                        /*
                        if ()
                        {
                            C.Add();

                        }
                        */
                        // FindNewEvent(sl, sr, p):
                        BinaryKeyValueNode<double, Curve> root = T.Root;
                        Curve sl = root.LeftChild.Value;
                        Curve sr = root.RightChild.Value;
                        Point3d ptInt = UsefulFunctions.IntersectionPoint(sl, sr);
                        if (ptInt != null && !Q.Contains(ptInt))
                        {
                            Q.Enqueue(ptInt);
                        }


                        /* BinaryKeyValueNode<double, Curve> node = T.Search(Math.Round(p, 2));
                        // Curve crvP = node.Value;

                        if (node.LeftChild != null)
                        {
                            Curve left = node.LeftChild.Value;
                            // intersect segment on the left?
                            CurveIntersections intLeft = Intersection.CurveCurve(crvP, left, 0.001, 0.001);

                            if (intLeft.Count >= 1)
                            {
                                RhinoApp.WriteLine(intLeft.Count.ToString());
                                ptA = intLeft[0].PointA;
                                intPt.Add(ptA);
                                doc.Objects.AddPoint(ptA);
                                Q.Enqueue(ptA);
                            }
                            else
                            {
                                RhinoApp.WriteLine("there are no curve intersections with left segment");
                            }
                        }
                        if (node.RightChild != null)
                        {
                            Curve right = node.RightChild.Value;
                            // intersect segment on the right?
                            CurveIntersections intRight = Intersection.CurveCurve(crvP, right, 0.001, 0.001);

                            if (intRight.Count >= 1)
                            {
                                RhinoApp.WriteLine(intRight.Count.ToString());
                                ptA = intRight[0].PointA;
                                intPt.Add(ptA);
                                doc.Objects.AddPoint(ptA);
                                Q.Enqueue(ptA);
                            }
                            else
                            {
                                RhinoApp.WriteLine("there are no curve intersections with right segment");
                            }
                        }
                    }
                    
                    // delete segment at end point
                    if (Math.Round(crv.PointAtEnd.Y, 2) == Math.Round(temp.Y, 2))
                    {
                        T.Delete(temp.Y);
                        if (U.Contains(crv))
                        {
                            L.Add(crv);
                        }

                    }*/
                    }
                }
                // B. the binary-search-tree property allows us to print out all the keys in a bst in sorted order
                // by a simple recursive algorithm - inorder tree walk:

                RhinoApp.WriteLine();
                T.InOrderTreeWalk(T.Root);
                doc.Views.Redraw();
                
            }
            return Result.Success;
        }
    }
}
