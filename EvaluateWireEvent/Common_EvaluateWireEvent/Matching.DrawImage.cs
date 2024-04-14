using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EvaluateWireEvent
{
    internal partial class PathChecker
    {
        /// <summary>
        /// The helper function to draw an image on this form.
        /// </summary>
        /// <param name="left">Left parameter in wiring.</param>
        /// <param name="right">Right parameter in wiring.</param>
        /// <returns></returns>
        private static Bitmap DrawImage(IGH_Param left,IGH_Param right)
        {
            Grasshopper.GUI.Canvas.GH_Canvas canvas = Grasshopper.Instances.ActiveCanvas;

            Bitmap bitmap = canvas.GetCanvasScreenBuffer(Grasshopper.GUI.Canvas.GH_CanvasMode.Export);
            Graphics graphics =Graphics.FromImage(bitmap);

            GraphicsPath path = GH_Painter.ConnectionPath(canvas.Viewport.ProjectPoint(left.Attributes.OutputGrip), canvas.Viewport.ProjectPoint(right.Attributes.InputGrip), GH_WireDirection.right, GH_WireDirection.left);
            graphics.DrawPath(new Pen(Color.OrangeRed, 20) { DashPattern = new float[] { 3, 1.5f } }, path);

            Pen pen = new Pen(Color.Red, 20);
            graphics.DrawEllipse(pen, GripRectangle(left.Attributes.OutputGrip));
            graphics.DrawEllipse(pen, GripRectangle(right.Attributes.InputGrip));

            graphics.Dispose();

            return bitmap;
        }

        /// <summary>
        /// The helper function to mark a grip.
        /// </summary>
        /// <param name="gripPt">The grip point of a parameter.</param>
        /// <returns></returns>
        private static RectangleF GripRectangle(System.Drawing.PointF gripPt)
        {
            PointF center = Grasshopper.Instances.ActiveCanvas.Viewport.ProjectPoint(gripPt);
            RectangleF rectangle = new RectangleF(center, new SizeF(100, 100));
            rectangle.Offset(-50, -50);

            return rectangle;
        }
    }
}
