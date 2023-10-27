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
    class FindNewEvent
    {
        //1.Variables

        //2.Properties

        //3.Constructor
        public FindNewEvent()
        {

        }
        public FindNewEvent(BinaryKeyValueNode<double, Curve> leftchild, BinaryKeyValueNode<double, Curve> rightchild, 
            Point3d p, Queue<Point3d> Q, RhinoDoc doc)
        {
            if (leftchild != null && rightchild != null)
            {
                Curve curveA = leftchild.Value;
                Curve curveB = rightchild.Value;
            }
            else
            {
                RhinoApp.WriteLine("No intersection possible");
            }

        }

        //4.Methods
        /// <summary>
        /// test for intersections
        /// </summary>
        /// <param name="curveA"></param>
        /// <param name="curveB"></param>
        /// <param name="Q"></param>
        /// <param name="doc"></param>
        /// <returns></returns> the intersection point enqueued
        public Point3d CalculateIntersection(Curve curveA, Curve curveB, Queue<Point3d> Q, RhinoDoc doc)
        {
            Point3d newPt = new Point3d();
            // Calculate Intersection
            CurveIntersections events1 = Intersection.CurveCurve(curveA, curveB,
                    0.001, 0.00);

            // Process the results
            if (events1 != null)
            {
                for (int i = 0; i < events1.Count; i++)
                {
                    IntersectionEvent ccx_event = events1[i];
                    if (Q.Contains(ccx_event.PointA))
                    {
                        RhinoApp.WriteLine("Already an event point");
                    }
                    else
                    {
                        doc.Objects.AddPoint(ccx_event.PointA);
                        Q.Enqueue(ccx_event.PointA);
                    }

                    if (ccx_event.PointA.DistanceTo(ccx_event.PointB) > double.Epsilon)
                    {
                        doc.Objects.AddPoint(ccx_event.PointB);
                        Q.Enqueue(ccx_event.PointB);
                    }

                }
                doc.Views.Redraw();
            }
            return newPt;
        }

    }
}
