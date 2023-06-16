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
            //Curve upper;
            //Curve lower;
            //Curve contain;
            
            //U(p) segments whose upper end point is p
            List<BinaryKeyValueNode<double, Curve>> U = new List<BinaryKeyValueNode<double, Curve>>();
            BinaryKeyValueNode<double, Curve> upper;
            //L(p) segments whose lower end point is p
            List<BinaryKeyValueNode<double, Curve>> L = new List<BinaryKeyValueNode<double, Curve>>();
            BinaryKeyValueNode<double, Curve> lower;
            //C(p) segments that contain p in their interior
            List<BinaryKeyValueNode<double, Curve>> C = new List<BinaryKeyValueNode<double, Curve>>();
            BinaryKeyValueNode<double, Curve> contain;

            //line function to determine if pt lays in the segment - y = mx + b, where m is equal to the slope and b i the intersection with y axis
            Point3d start = crv.PointAtStart;
            Point3d end = crv.PointAtEnd;
            double m = (end.Y - start.Y) / (end.X - start.X);
            double b = start.Y - m * start.X;
            
            if (crv != null)
            {
                if (crv.PointAtStart.Y == p.Y)
                {
                    BinaryKeyValueNode<double, Curve> u = T.FindNode(p.X);
                    upper = new BinaryKeyValueNode<double, Curve>(p.X, crv);
                    U.Add(upper);
                }
                else if (crv.PointAtEnd.Y == p.Y)
                {
                    BinaryKeyValueNode<double, Curve> l = T.FindNode(p.X);
                    lower = new BinaryKeyValueNode<double, Curve>(p.X, crv);
                    L.Add(lower);
                }
                else if (p.Y == m * p.X + b)
                {
                    BinaryKeyValueNode<double, Curve> c = T.FindNode(p.X);
                    contain = new BinaryKeyValueNode<double, Curve>(p.X, crv);
                    C.Add(contain);
                }
                
                //if L(p) U U(p) U C(p) => more than one segment where there is an intersection
                int segmentIntCount = U.Count + L.Count + C.Count;
                if (segmentIntCount > 1)
                {
                    // report p as an intersection
                    intPoint = p;
                    // report set of segments L(p) U(p) C(p)
                    // delete segments in L(p) and C(p) due to their intersection
                    //T.DeleteNode(lower);
                    //T.DeleteNode(contain);

                    /*
                    int segIntCount = U.Count + C.Count;
                    if (segIntCount == 0)
                    {
                        UsefulFunctions.NewEvent();
                    }
                    */
                }
            }

            return intPoint;
        }
    }
}
