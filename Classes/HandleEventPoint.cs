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

namespace binaryTreeExample.Classes 
{
    class HandleEventPoint
    {

        //variables



        //properties



        //constructor
        public HandleEventPoint()
        {


            throw new NotImplementedException();
        }

        //methods

        //test for intersection against the ones already intersecting the sweep line
        //order the segments form left to right as they intersect the sweep line
        //test only segments that are adjacent in the horizontal ordering
        //that is test a new segment only with the ones immediately left and right 
        //if (balanced)
        //{

        public static List<Point3d> IntersectionPt(Point3d p, BinarySearchTreeKV<double, Curve> T)
        {
            // initialize intersection point
            CurveIntersections intPointl = null;
            CurveIntersections intPointr = null;
            List<Point3d> pts = null;

            //U(p) segments whose upper end point is p
            List<BinaryKeyValueNode<double, Curve>> U = new List<BinaryKeyValueNode<double, Curve>>();
            //L(p) segments whose lower end point is p
            List<BinaryKeyValueNode<double, Curve>> L = new List<BinaryKeyValueNode<double, Curve>>();
            //C(p) segments that contain p in their interior
            List<BinaryKeyValueNode<double, Curve>> C = new List<BinaryKeyValueNode<double, Curve>>();

            // line function to determine if pt lays in the segment - y = mx + b, where m is equal to the slope and b i the intersection with y axis
            // Point3d start = crv.PointAtStart;
            // Point3d end = crv.PointAtEnd;
            // double m = (end.Y - start.Y) / (end.X - start.X);
            // double b = start.Y - m * start.X;

            if (T != null)
            {
                // from p to the T node to identify the curves adjacent in T
                BinaryKeyValueNode<double, Curve> node = T.FindNode(p.X);
                BinaryKeyValueNode<double, Curve> leftChild = node.LeftChild;
                BinaryKeyValueNode<double, Curve> rightChild = node.RightChild;

                if (node.Value.PointAtStart.Y == p.Y)
                {
                    U.Add(node);
                }
                else if (node.Value.PointAtEnd.Y == p.Y)
                {
                    L.Add(node);
                }

                if (leftChild != null)
                {
                    double ml = (Math.Round(leftChild.Value.PointAtEnd.Y, 2) - Math.Round(leftChild.Value.PointAtStart.Y, 2)) /
                                (Math.Round(leftChild.Value.PointAtEnd.X, 2) - Math.Round(leftChild.Value.PointAtStart.X, 2));
                    double bl = Math.Round(leftChild.Value.PointAtStart.Y, 2) - Math.Round(ml * leftChild.Value.PointAtStart.X, 2);
                    if (Math.Round(p.Y, 2) == Math.Round((ml * p.X), 2) + bl)
                    {
                        C.Add(leftChild);
                    }
                }

                if (rightChild != null)
                {
                    double mr = (Math.Round(rightChild.Value.PointAtEnd.Y, 2) - Math.Round(rightChild.Value.PointAtStart.Y, 2)) /
                                (Math.Round(rightChild.Value.PointAtEnd.X, 2) - Math.Round(rightChild.Value.PointAtStart.X, 2));
                    double br = Math.Round(rightChild.Value.PointAtStart.Y, 2) - Math.Round(mr * rightChild.Value.PointAtStart.X, 2);
                    if (Math.Round(p.Y, 2) == Math.Round(mr * p.X, 2) + br)
                    {
                        C.Add(rightChild);
                    }
                }

                //if L(p) U U(p) U C(p) => more than one segment where there is an intersection
                if (L.Count > 0)
                {
                    if (C.Contains(L[0]))
                    {
                        // report an intersection and identify the itersection point, enqueue and replay the HandleEventPoint routine
                        if (leftChild != null)
                        {
                            intPointl = Intersection.CurveCurve(node.Value, leftChild.Value, 0.001, 0.01);
                            IEnumerator<IntersectionEvent> intPointsl =intPointl.GetEnumerator();
                            if (intPointsl != null)
                            {
                                while (intPointsl.MoveNext())
                                {
                                    IntersectionEvent pt = intPointsl.Current;
                                    Point3d intersectl = pt.PointA;
                                    pts.Add(intersectl);
                                }
                            }
                        }

                        if (rightChild != null)
                        {
                            intPointr = Intersection.CurveCurve(node.Value, rightChild.Value, 0.001, 0.01);
                            IEnumerator<IntersectionEvent> intPointsr = intPointl.GetEnumerator();
                            if (intPointsr != null)
                            {
                                while (intPointsr.MoveNext())
                                {
                                    IntersectionEvent pt = intPointsr.Current;
                                    Point3d intersectr = pt.PointA;
                                    pts.Add(intersectr);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //intPoint = new Point3d(2, 3, 0);
                }

                if (U.Count > 0)
                {
                    if (C.Contains(U[0]))
                    {

                    }
                }
                else
                {
                    //intPoint = new Point3d(5, 13, 0);
                }
            }

            return pts;
        }
    }
}
