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

            // initialize intersection point
            CurveIntersections intPointl = null;
            CurveIntersections intPointr = null;
            List<Point3d> pts = null;

            if (T != null)
            {
                // identify adjacency by way of the left and right nodes of the binary tree T
                BinaryKeyValueNode<double, Curve> node = T.FindNode(p.X);
                BinaryKeyValueNode<double, Curve> leftChild = node.LeftChild;
                BinaryKeyValueNode<double, Curve> rightChild = node.RightChild;

                if (leftChild != null)
                {
                    intPointl = Intersection.CurveCurve(node.Value, leftChild.Value, 0.001, 0.01);
                    IEnumerator<IntersectionEvent> intPointsl = intPointl.GetEnumerator();
                    if (intPointsl != null)
                    {
                        while (intPointsl.MoveNext())
                        {
                            IntersectionEvent pt = intPointsl.Current;
                            Point3d intersectl = pt.PointA;
                            doc.Objects.AddPoint(intersectl);
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
                            doc.Objects.AddPoint(intersectr);
                            pts.Add(intersectr);
                        }
                    }
                }
            }

            return pts;
        }
    }
}
