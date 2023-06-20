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
            
            //U(p) segments whose upper end point is p
            List<Curve> U = new List<Curve>();
            //BinaryKeyValueNode<double, Curve> upper;
            //L(p) segments whose lower end point is p
            List<Curve> L = new List<Curve>();
            BinaryKeyValueNode<double, Curve> lower;
            //C(p) segments that contain p in their interior
            List<Curve> C = new List<Curve>();
            BinaryKeyValueNode<double, Curve> contain;

            //line function to determine if pt lays in the segment - y = mx + b, where m is equal to the slope and b i the intersection with y axis
            Point3d start = crv.PointAtStart;
            Point3d end = crv.PointAtEnd;
            double m = (end.Y - start.Y) / (end.X - start.X);
            double b = start.Y - m * start.X;
            
            if (crv != null)
            {
                // Find all segments stored in T that contain p
                // BinaryKeyValueNode<double, Curve> node = T.FindNode(p.X);

                if (crv.PointAtStart.Y == p.Y)
                {
                    //Curve u = T.FindFirst(p.X);
                    U.Add(crv);
                }
                else if (crv.PointAtEnd.Y == p.Y)
                {
                    //Curve l = T.FindFirst(p.X);
                    L.Add(crv);
                }
                else if (p.Y == m * p.X + b)
                {
                    //Curve c = T.FindFirst(p.X);
                    C.Add(crv);
                }
            }

            // if L(p) U U(p) U C(p) => more than one segment where there is an intersection
            // int segmentIntCount = U.Count + L.Count + C.Count;
            // in a list of objects (Curve) -not structs (or strings)- then intersect their keys first, 
            // and then select objects by those keys:

            //double keys = p.Select(x => x.Id).Intersect(list2.Select(x => x.Id));
            //Curve value = crv.Where(x => ids.Contains(x.Id));

            if (U.Contains(crv)) // revisar la condición para la intersección de las listas
            {
                // report p as an intersection
                intPoint = p;
                // report set of segments L(p) U(p) C(p)
                // delete segments in L(p) U C(p) due to their intersection
                lower = new BinaryKeyValueNode<double, Curve>(p.X, L[0]);
                contain = new BinaryKeyValueNode<double, Curve>(p.X, C[0]);
                T.DeleteNode(lower);
                T.DeleteNode(contain);

                // insert the segments U(p) U C(p) into T
                T.Insert(p.X, U[0]);
                T.Insert(p.X, C[0]);
            }

            return intPoint;
        }
    }
}
