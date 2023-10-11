using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace binaryTreeExample.Classes
{
    public class calculateIntersections
    {
        //variables
        RhinoDoc doc;
        private Point3d intPt;
        BinaryKeyValueNode<double, Curve> node;
        BinaryKeyValueNode<double, Curve> leftNode;
        BinaryKeyValueNode<double, Curve> rightNode;
        Curve curveA;
        Curve curveL;
        private Curve curveR;
        Point3d pt = new Point3d();
        const double intersection_tolerance = 0.001;
        const double overlap_tolerance = 0.0;

        //properties



        //constructor
        public calculateIntersections(RhinoDoc Doc)
        {
            
            doc = Doc;

        }

        //methods
        public Point3d interPoint(BinarySearchTreeKV<double, Curve> T, double key)
        {
            BinaryKeyValueNode<double, Curve> node = T.Search(key);
            if (node != null)
            {
                if (node.LeftChild != null || node.RightChild != null)
                {
                    BinaryKeyValueNode<double, Curve> leftNode = node.LeftChild;
                    BinaryKeyValueNode<double, Curve> rightNode = node.RightChild;

                    if (leftNode.Value != null || rightNode.Value != null)
                    {
                        Curve curveA = node.Value;
                        if (leftNode.Value != null)
                        {
                            // Take curves from the tree
                            Curve curveL = leftNode.Value;

                            // Calculate the intersection
                            
                            CurveIntersections events1 = Intersection.CurveCurve(curveA, curveL,
                                intersection_tolerance, overlap_tolerance);

                            // Process the results
                            if (events1 != null)
                            {
                                for (int i = 0; i < events1.Count; i++)
                                {
                                    var ccx_event = events1[i];
                                    doc.Objects.AddPoint(ccx_event.PointA);
                                    if (ccx_event.PointA.DistanceTo(ccx_event.PointB) > double.Epsilon)
                                    {
                                        doc.Objects.AddPoint(ccx_event.PointB);
                                        doc.Objects.AddLine(ccx_event.PointA, ccx_event.PointB);
                                    }
                                }

                                doc.Views.Redraw();
                            }

                            if (rightNode.Value != null)
                            {
                                // Take curves from the tree
                                Curve curveR = rightNode.Value;

                                // Calculate the intersection
                                const double intersection_tolerance = 0.001;
                                const double overlap_tolerance = 0.0;
                                CurveIntersections events2 = Intersection.CurveCurve(curveA, curveR,
                                    intersection_tolerance, overlap_tolerance);

                                // Process the results
                                if (events2 != null)
                                {
                                    for (int i = 0; i < events2.Count; i++)
                                    {
                                        var ccx_event = events2[i];
                                        doc.Objects.AddPoint(ccx_event.PointA);
                                        if (ccx_event.PointA.DistanceTo(ccx_event.PointB) > double.Epsilon)
                                        {
                                            doc.Objects.AddPoint(ccx_event.PointB);
                                            doc.Objects.AddLine(ccx_event.PointA, ccx_event.PointB);
                                        }
                                    }

                                    doc.Views.Redraw();
                                }
                            }
                        }
                    }
                }
            }

            return pt;
        }

    }
}
