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
    class HandelEventPoint
    {
        //1.variables
        List<Curve> U = new List<Curve>();
        List<Curve> L = new List<Curve>();
        List<Curve> C = new List<Curve>();
        BinarySearchTreeKV<double, Curve> T = new BinarySearchTreeKV<double, Curve>();

        //2.properties

        //3.constructor
        public HandelEventPoint(Point3d p, BinarySearchTreeKV<double, Curve> t)
        {
            Point3d eventPoint = p;
            T = t;
        }
        //4.methods
        /// <summary>
        /// Find set of segments for which p is a start point
        /// </summary>
        /// <param name="S"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public List<Curve> UpperEndPointSet(List<Curve> S, Point3d p)
        {
            foreach (Curve crv in S)
            {
                // select segments at start point p
                if (Math.Round(crv.PointAtStart.Y, 2) == Math.Round(p.Y, 2))
                {
                    U.Add(crv);
                }
            }
            return U;
        }

        public List<Curve> LowerEndPointSet(List<Curve> U, Point3d p)
        {
            BinaryKeyValueNode<double, Curve> node = T.Search(p.X);
            if (Math.Round(node.Value.PointAtEnd.Y, 2) == Math.Round(p.Y, 2))
            {
                L.Add(node.Value);
            }
            return L;
        }

        /// <summary>
        /// find segments where p lies on the lines
        /// </summary>
        /// <returns></returns>
        public List<Curve> ContainsSet()
        {
            //proof of coincidence of a point on the line
            return C;
        }

        // if there is no segments in the union of sets U and C
        public Point3d FindNew(Curve curveA, Curve curveB, Queue<Point3d> Q, RhinoDoc doc)
        {
            FindNewEvent newEventPoint = new FindNewEvent();
            Point3d pt = newEventPoint.CalculateIntersection(curveA, curveB, Q, doc);
            return pt;
        }
    }
}
