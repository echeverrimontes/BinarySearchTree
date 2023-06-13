using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

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

        public static Point3d IntersectionPt(Point3d p, BinarySearchTreeKV<double, Curve> T, Curve crv)
        {
            Point3d intPoint = new Point3d();

            double tempPtY = Math.Round(p.Y, 2);
            T.Insert(tempPtY, crv);

            //U(p) segments whose upper end point is p
            List<Curve> U = new List<Curve>();
            //L(p) segments whose lower end point is p
            List<Curve> L = new List<Curve>();
            //C(p) segments that contain p in their interior
            List<Curve> C = new List<Curve>();

            //line function to determine if pt lays in the segment - y = mx + b, where m is equal to the slope and b i the intersection with y axis
            Point3d start = crv.PointAtStart;
            Point3d end = crv.PointAtEnd;
            double m = (end.Y - start.Y) / (end.X - start.X);
            double b = start.Y - m * start.X;
            
            if (crv != null)
            {
                if (crv.PointAtStart.Y == p.Y)
                {
                    U.Add(crv);
                }
                else if (crv.PointAtEnd.Y == p.Y)
                {
                    L.Add(crv);
                }
                else if (p.Y == m * p.X + b)
                {
                    C.Add(crv);
                }

                if (U.Count > 1 && L.Count > 1 && C.Count > 1)
                {
                    intPoint = p;
                    // borrar del binaryTree las curvas
                }
            }

            return intPoint;
        }
    }
}
