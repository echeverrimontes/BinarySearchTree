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

        public static List<Point3d> IntersectionPt(Point3d p, BinarySearchTreeKV<double, Curve> T, RhinoDoc doc)
        {
            // an event point p determines the status of the sweep line, 
            // right below the sweepline there is an intersection point pInt where
            // Si and Sj become adjacent and thus are tested for intersection
            // only intersections below sweepline are important. They constitute a new event point
            // maintain the ordered sequence of segments
            List<Point3d> pts = new List<Point3d>();
            Curve leftCurveP = null;
            Curve rightCurveP = null;
            double x = Math.Round(p.X, 2);

            // extract the value (Curve at this node)
            BinaryKeyValueNode<double, Curve> nodeP = T.FindNode(x);
            Curve crvP = nodeP.Value;
            RhinoApp.WriteLine("(" + Math.Round(nodeP.Key, 2).ToString() + ", " + nodeP.Value.ToString() + ")");

            // 1. search in T for the segment immediately to the left and right
            // to some point p that lies on the sweep line:

            if (nodeP.LeftChild != null)
            {
                BinaryKeyValueNode<double, Curve> leftChildP = nodeP.LeftChild;
                leftCurveP = leftChildP.Value; // segment left to the event point p
            }
            if (nodeP.RightChild != null)
            {
                BinaryKeyValueNode<double, Curve> rightChildP = nodeP.RightChild;
                rightCurveP = rightChildP.Value; // segment right to the event point p
            }
            // at each internal node nodeKV test whether p lies left or right of the segment stored at nodeKV
            // depending on the outcome descend to left or right subtree of nodeKV, eventually ending up in a leaf
            // either this leaf or the leaf immediately to the left or right of it stores the segment being searched for
            IEnumerable InOrder = T.TraverseTree(BinarySearchTreeKV<double, Curve>.DepthFirstTraversalMethod.InOrder);
            foreach (double i in InOrder)
            {
                BinaryKeyValueNode<double, Curve> node = T.FindNode(i);
                if (x < Math.Round(node.Key, 2)) // event point p lies to the left of nodeKV
                {
                    if (leftCurveP != null)
                    {
                        LineCurve crv0 = leftCurveP as LineCurve;
                        LineCurve crv1 = node.Value as LineCurve;
                        if (crv0 == null || crv1 == null)
                            return null;
                        else if (crv0 != null || crv1 != null)
                        {
                            Line line0 = crv0.Line;
                            Line line1 = crv1.Line;
                            RhinoApp.WriteLine("(left: " + node.Value.ToString() + ", " + leftCurveP.ToString() + ")");
                            /*
                            CurveIntersections intPointl = Intersection.CurveCurve(node.Value, leftCurveP, 0.001, 0.01);
                            IEnumerator<IntersectionEvent> intPointsl = intPointl.GetEnumerator();
                            if (intPointsl != null)
                            {
                                while (intPointsl.MoveNext())
                                {
                                    IntersectionEvent pt = intPointsl.Current;
                                    Point3d l = pt.PointA;
                                    doc.Objects.AddPoint(l);
                                    pts.Add(l);
                                }
                            }
                            */
                            double a, b;
                            if (!Intersection.LineLine(line0, line1, out a, out b))
                            {
                                RhinoApp.WriteLine("No intersection found.");
                                //return Rhino.Commands.Result.Nothing;
                            }
                            Point3d pt0 = line0.PointAt(a);
                            Point3d pt1 = line1.PointAt(b);
                            // pt0 and pt1 should be equal, so we will only add pt0 to the document
                            doc.Objects.AddPoint(pt0);
                            //doc.Views.Redraw();
                        }
                    }
                }
                else if (x > Math.Round(node.Key, 2)) // event point p lies to the right of nodeKV
                {
                    if (rightCurveP != null)
                    {
                        LineCurve crv0 = rightCurveP as LineCurve;
                        LineCurve crv1 = node.Value as LineCurve;
                        if (crv0 == null || crv1 == null)
                            return null;
                        else if (crv0 != null || crv1 != null)
                        {
                            Line line0 = crv0.Line;
                            Line line1 = crv1.Line;
                            RhinoApp.WriteLine("(right: " + node.Value.ToString() + ", " + rightCurveP.ToString() + ")");
                            /*
                            CurveIntersections intPointr = Intersection.CurveCurve(node.Value, rightCurveP, 0.001, 0.01);
                            IEnumerator<IntersectionEvent> intPointsr = intPointr.GetEnumerator();
                            if (intPointsr != null)
                            {
                                while (intPointsr.MoveNext())
                                {
                                    IntersectionEvent pt = intPointsr.Current;
                                    Point3d r = pt.PointA;
                                    doc.Objects.AddPoint(r);
                                    pts.Add(r);
                                }
                            }
                            */
                            double a, b;
                            if (!Intersection.LineLine(line0, line1, out a, out b))
                            {
                                RhinoApp.WriteLine("No intersection found.");
                                //return Rhino.Commands.Result.Nothing;
                            }
                            Point3d pt0 = line0.PointAt(a);
                            Point3d pt1 = line1.PointAt(b);
                            // pt0 and pt1 should be equal, so we will only add pt0 to the document
                            doc.Objects.AddPoint(pt0);
                            //doc.Views.Redraw();
                        }
                    }
                }
            }
            return pts;
        }
    }
}
