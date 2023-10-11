using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Commands;
using Rhino;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using Rhino.Input.Custom;
using Rhino.DocObjects;
using Rhino.Input;
using Rhino.UI;
using Command = Rhino.Commands.Command;

namespace binaryTreeExample.Classes
{
    class ObjectAtt
    {
        //variables

        //properties

        //methods

        public static Rhino.Commands.Result ObjectColor(Rhino.RhinoDoc doc)
        {
            Rhino.DocObjects.ObjRef[] objRefs;
            Rhino.Commands.Result cmdResult = Rhino.Input.RhinoGet.GetMultipleObjects("Select objects to change color", 
                false, Rhino.DocObjects.ObjectType.AnyObject, out objRefs);
            if (cmdResult != Rhino.Commands.Result.Success)
                return cmdResult;

            System.Drawing.Color color = System.Drawing.Color.Black;
            bool rc = Rhino.UI.Dialogs.ShowColorDialog(ref color);
            if (!rc)
                return Rhino.Commands.Result.Cancel;

            for (int i = 0; i < objRefs.Length; i++)
            {
                Rhino.DocObjects.RhinoObject obj = objRefs[i].Object();
                if (null == obj || obj.IsReference)
                    continue;

                if (color != obj.Attributes.ObjectColor)
                {
                    obj.Attributes.ObjectColor = color;
                    obj.Attributes.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject;
                    obj.CommitChanges();
                }
            }

            doc.Views.Redraw();

            return Rhino.Commands.Result.Success;
        }

        /// <summary>
        /// modify object color
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public Result ModifyObjectColor(RhinoDoc doc)
        {
            ObjRef obj_ref;
            Result rc = RhinoGet.GetOneObject("Select object", false, ObjectType.AnyObject, out obj_ref);
            if (rc != Result.Success)
                return rc;
            RhinoObject rhino_object = obj_ref.Object();
            Color color = rhino_object.Attributes.ObjectColor;
            bool b = Dialogs.ShowColorDialog(ref color);
            if (!b) return Result.Cancel;

            rhino_object.Attributes.ObjectColor = color;
            rhino_object.Attributes.ColorSource = ObjectColorSource.ColorFromObject;
            rhino_object.CommitChanges();

            /*
            // an object's color attributes can also be specified
            // when the object is added to Rhino
            Sphere sphere = new Sphere(Point3d.Origin, 5.0);
            ObjectAttributes attributes = new ObjectAttributes();
            attributes.ObjectColor = Color.CadetBlue;
            attributes.ColorSource = ObjectColorSource.ColorFromObject;
            doc.Objects.AddSphere(sphere, attributes);
            */
            doc.Views.Redraw();
            return Result.Success;
        }




    }
}
