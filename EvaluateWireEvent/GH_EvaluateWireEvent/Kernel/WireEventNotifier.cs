using EvaluateWireEvent;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;

namespace GH_EvaluateWireEvent.Kernel
{
    public class WireEventNotifier
    {
        private GH_Canvas m_canvas;
        private GH_Document m_document;

        private WireEventNotifier(GH_Canvas canvas, GH_Document document)
        {
            m_canvas = canvas;
            m_document = document;
        }

        public static WireEventNotifier Instantiate(GH_Canvas canvas, GH_Document document)
        {
            WireEventNotifier notifier = new WireEventNotifier(canvas,document);
            Rhino.RhinoApp.WriteLine($"{nameof(WireEventNotifier)} is loaded.");

            foreach (IGH_ActiveObject activeObject in document.ActiveObjects())
            {
                activeObject.ObjectChanged -= WireFunctions.OnAddSource;
                activeObject.ObjectChanged += WireFunctions.OnAddSource;
            }

            document.ObjectsAdded -= Document_ObjectsAdded;
            document.ObjectsAdded += Document_ObjectsAdded;
            document.ObjectsDeleted -= Document_ObjectsDeleted;
            document.ObjectsDeleted += Document_ObjectsDeleted;

            return notifier;
        }

        private static void Document_ObjectsAdded(object sender, GH_DocObjectEventArgs e)
        {
            foreach (IGH_DocumentObject obj in e.Objects)
            {
                if (obj is IGH_ActiveObject activeObject)
                    activeObject.
            }
        }

        private static void Document_ObjectsDeleted(object sender, GH_DocObjectEventArgs e)
        {
            foreach (IGH_DocumentObject obj in e.Objects)
            {
                if (obj is IGH_ActiveObject activeObject)
                    activeObject.ObjectChanged -= WireFunctions.OnAddSource;
            }
        }
    }
}
