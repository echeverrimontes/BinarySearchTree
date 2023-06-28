using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using Rhino.Input.Custom;

namespace binaryTreeExample.Classes
{
    class UsefulFunctions
    {
        /// <summary>
        /// This function finds start and end points in a series of lines
        /// </summary>
        /// <param name="lineCurves"></param>
        /// <param name="doc"></param>
        public static Queue<Point3d> EventPointQueue(List<Curve> lineCurves, RhinoDoc doc)
        {
            //1. initialize an empty queue of event points according to y position => sweep line status
            Queue<Point3d> Q = new Queue<Point3d>();

            //2. list of points existing in the selection: select segment start and end points
            Point3d[] Qs = new Point3d[lineCurves.Count * 2];
            double[] QsY = new double[lineCurves.Count * 2];

            int i = 0;
            int j = lineCurves.Count;

            foreach (Curve crv in lineCurves) // generate the array of points by selecting and inserting the point from segment
            {
                Point3d ptStart = crv.PointAtStart;
                Point3d ptEnd = crv.PointAtEnd;
                Qs[i] = ptStart;
                QsY[i] = ptStart.Y;
                Qs[j] = ptEnd;
                QsY[j] = ptEnd.Y;

                j += 1;
                i += 1;
            }

            //4. sort lists based on the y coordinate value and Queue the points in order according to y coordinate
            Array.Sort(QsY, Qs);
            Array.Reverse(Qs);
            //IntArrayQuickSort(Qs); // sorted array according to the Y coordinate
            
            //5. enqueue the points in order of appearance from top to bottom
            //if two event points have the same y-coordinate, then the one with smallest x-coordinate will be the first
            foreach (Point3d pt in Qs)
            {
                Q.Enqueue(pt);
            }
            
            return Q;
        }

        /// <summary>
        /// select lines in a document
        /// </summary>
        /// <returns></returns>
        public static List<Curve> SelectCurve()
        {
            GetObject gc = new GetObject();
            gc.SetCommandPrompt("Select curves");
            gc.GeometryFilter = ObjectType.Curve;
            gc.GetMultiple(1, 0);

            int lineCount = 0;
            int nurbsCount = 0;
            int arcCount = 0;
            int circleCount = 0;
            int polylineCount = 0;

            //Create a collection of curves
            List<Curve> crvs = new List<Curve>(gc.ObjectCount);

            for (int i = 0; i < gc.ObjectCount; i++)
            {
                Curve crv = gc.Object(i).Curve();
                if (null != crv)
                {
                    crvs.Add(crv);

                    NurbsCurve nurbsCurve = crv as NurbsCurve;
                    if (nurbsCurve != null)
                        nurbsCount++;

                    //Curve curve = crv as Curve;
                    //if (curve != null)
                    //    lineCount++;

                    LineCurve line_curve = crv as LineCurve;
                    if (line_curve != null)
                        lineCount++;

                    ArcCurve arc_curve = crv as ArcCurve;
                    if (arc_curve != null)
                    {
                        if (arc_curve.IsCircle())
                            circleCount++;
                        else
                            arcCount++;
                    }

                    PolylineCurve poly_curve = crv as PolylineCurve;
                    if (poly_curve != null)
                        polylineCount++;
                }
            }

            string s = string.Format(
                "the user selected {0} lines, {1} circles, {2} arcs, {3} polylines, {4} nurbs and {5} in total",
                lineCount.ToString(), circleCount.ToString(), arcCount.ToString(), polylineCount.ToString(),
                nurbsCount.ToString(), crvs.Count.ToString());

            RhinoApp.WriteLine(s);

            return crvs;
        }

        /// <summary>
        /// Select StartPoints for line segments
        /// </summary>
        /// <returns></returns>
        public static Point3d[] PointsStart()
        {
            List<Curve> crvs = SelectCurve();


            //1. initialize an empty event queu Q, insert segment start and endpoints in Q
            Point3d[] ptSt = new Point3d[crvs.Count];

            Point3d ptStart;

            for (int i = 0; i < crvs.Count; i++)
            {
                Hashtable segs = new Hashtable();
                ptStart = crvs[i].PointAtStart;
                segs.Add(i, crvs[i]);
                ptSt[i] = ptStart;
            }

            return ptSt;
        }

        /// <summary>
        /// Select EndPoints for line segments
        /// </summary>
        /// <returns></returns>
        public static Point3d[] PointsEnd()
        {
            List<Curve> crvs = SelectCurve();

            //1. initialize an empty event queu Q, insert segment start and endpoints in Q
            Point3d[] ptE = new Point3d[crvs.Count];

            Point3d ptEnd;

            for (int i = 0; i < crvs.Count; i++)
            {
                ptEnd = crvs[i].PointAtEnd;
                ptE[i] = ptEnd;
            }

            return ptE;
        }

        /// <summary>
        /// intersections using the dictionary key/value pair
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static Point3d IntersectionPoint(Curve Sl, Curve Sr)
        {
            Point3d ptA = new Point3d(); //intersection point 
            //check for intersections of pairs of segments left and right according to the status structure (sorted dictionary)
            //take each segment and find left and right neighbors 
            //check for intersection
            CurveIntersections intersect = Intersection.CurveCurve(Sl, Sr, 0.001, 0.001);

            if (intersect.Count >= 1)
            {
                RhinoApp.WriteLine(intersect.Count.ToString());
                ptA = intersect[0].PointA;
            }
            else
            {
                RhinoApp.WriteLine("there are no curve intersections");
            }

            return ptA;
        }

        //a utility function to sort an array of points
        /// <summary>
        /// Swap two elements in an array
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        static void Swap(Point3d[] arr, int i, int j)
        {
            Point3d temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }

        /* This function takes last element as pivot, places
             the pivot element at its correct position in sorted
             array, and places all smaller (smaller than pivot)
             to left of pivot and all greater elements to right
             of pivot */
        static int Partition(Point3d[] arr, int low, int high)
        {

            // pivot
            double pivot = arr[high].Y;

            // Index of smaller element and indicates the right position
            // of pivot found so far
            int i = (low - 1);

            for (int j = low; j <= high - 1; j++)
            {

                // If current element is smaller than the pivot
                if (arr[j].Y < pivot)
                {
                    // Increment index of smaller element
                    i++;
                    Swap(arr, i, j);
                }
            }
            Swap(arr, i + 1, high);
            return (i + 1);
        }

        /* The main function that implements QuickSort
                    arr[] --> Array to be sorted,
                    low --> Starting index,
                    high --> Ending index
           */
        static void QuickSort(Point3d[] arr, int low, int high)
        {
            if (low < high)
            {

                // pi is partitioning index, arr[p] is now at right place
                int pi = Partition(arr, low, high);

                // Separately sort elements before partition and after partition
                QuickSort(arr, low, pi - 1);
                QuickSort(arr, pi + 1, high);
            }
        }

        public static Point3d[] SortedList(Point3d[] ptList)
        {
            int n = ptList.Length - 1;

            QuickSort(ptList, 0, n);

            return ptList;
        }

        // Function to print an array
        static void PrintArray(Point3d[] arr, int size)
        {
            for (int i = 0; i < size; i++)
                Console.Write(arr[i] + " ");

            Console.WriteLine();
        }

        /// <summary>
        /// find intersections between neighbor curves in the binary tree
        /// </summary>
        /// <param name="Q"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        public static Point3d IntersectionPt(Queue<Point3d> Q, BinaryTree T)
        {
            Point3d intPtQueue = new Point3d();
            while (Q.Count > 0)
            {
                //determine the next event point p in Q and dequeue it
                //HandleEventPoint(p)
                Point3d pt = Q.Dequeue();
                //Point3d pt = UsefulFunctions.HandleEventPoint(T, p);

                //U(p) segments whose upper end point is p
                List<Curve> U = new List<Curve>();
                TreeNode currentNodeU = T.Find(pt.X);
                if (currentNodeU != null)
                {
                    Point3d ptStart = currentNodeU.Data.Value.PointAtStart;
                    double ptStartX = Math.Round(ptStart.X, 2);
                    //Point3d ptEnd = currentNodeU.Data.Value.PointAtEnd;
                    if (pt.X == ptStartX)
                    {
                        U.Add(currentNodeU.Data.Value);
                    }
                }

                //L(p) segments whose lower end point is p
                List<Curve> L = new List<Curve>();
                TreeNode currentNodeL = T.Find(pt.X);
                if (currentNodeL != null)
                {
                    //Point3d ptStart = currentNodeL.Data.Value.PointAtStart;
                    Point3d ptEnd = currentNodeL.Data.Value.PointAtEnd;

                    if (currentNodeL.Data.Key < pt.X || currentNodeL.Data.Key > pt.X)
                    {
                        

                        L.Add(currentNodeL.Data.Value);
                    }
                }

                //C(p) segments that contain p in their interior
                List<Curve> C = new List<Curve>();
                TreeNode currentNodeC = T.Find(pt.X);
                if (currentNodeC != null)
                {
                    if (pt.X < pt.X || pt.X > pt.X)
                    {
                        Point3d ptStart = currentNodeC.Data.Value.PointAtStart;
                        Point3d ptEnd = currentNodeC.Data.Value.PointAtEnd;

                        C.Add(currentNodeC.Data.Value);
                    }
                }

                if (currentNodeU != null && currentNodeL != null && currentNodeC != null)
                {
                    if (U.Contains(currentNodeU.Data.Value) && L.Contains(currentNodeL.Data.Value) &&
                        C.Contains(currentNodeC.Data.Value))
                    {
                        intPtQueue = pt; // intersection points
                    }
                }

                //intPt = T.InOrderTraversal();
                //if there are intersections then 
                /*
                if (intPt != null)
                {
                    TreeNode currentNode = T.Find(j); //node being analyzed at the moment
                    if (intPt.X == ly)
                    {
                        ly = Math.Round(tempStY,
                            2); //the status of the sweep line is updated to the y location of point
                        intPtQueu.Enqueue(intPt);
                        //swap line segments in the binary tree after finding intersections
                        T.SwappNodes(currentNode);
                    }
                }*/
            }

            return intPtQueue;
        }

        /// <summary>
        /// Swap two numbers n and m, not necessarily consecutive numbers
        /// </summary>
        /// <param name="data"></param>
        /// <param name="m"></param>
        /// <param name="n"></param>
        public static void Exchange(Point3d[] data, int m, int n)
        {
            var temporary = data[m];
            data[m] = data[n];
            data[n] = temporary;
        }

        //http://anh.cs.luc.edu/170/notes/CSharpHtml/sorting.html sorting algorithms
        /// <summary>
        /// quick sort algorithm
        /// </summary>
        /// <param name="data"></param>
        /// <param name="l"></param>
        /// <param name="r"></param>
        public static void IntArrayQuickSort(Point3d[] data, int l, int r)
        {
            int i, j;
            Point3d x;

            i = l;
            j = r;

            x = data[(l + r) / 2]; /* find pivot item */
            while (true)
            {
                while (data[i].Y < x.Y)
                    i++;
                while (x.Y < data[j].Y)
                    j--;
                if (i <= j)
                {
                    Exchange(data, i, j);
                    i++;
                    j--;
                }
                if (i > j)
                    break;
            }
            if (l < j)
                IntArrayQuickSort(data, l, j);
            if (i < r)
                IntArrayQuickSort(data, i, r);
        }

        public static void IntArrayQuickSort(Point3d[] data)
        {
            IntArrayQuickSort(data, 0, data.Length - 1);
        }

        /// <summary>
        /// Order the data according to y coordinate from a disordered list of segments and start and end points
        /// </summary>
        /// <param name="data"></param>
        public static void IntArrayInsertionSort(Point3d[] data) // sort in y coordinate order
        {
            int i, j;
            int n = data.Length;

            for (j = 1; j < n; j++)
            {
                for (i = j; i > 0 && data[i].Y > data[i - 1].Y; i--)
                {
                    Exchange(data, i, i - 1);
                }
            }
        }

        public static Point3d NewEvent(Curve sl, Curve sr, Point3d p)
        {
            Point3d intPoint;
            if (true) // sl and sr intersect below sweep line, or on it and to the right of the current event point
            {
                intPoint = p;
                
            }

            return intPoint;
        }
    }
}
