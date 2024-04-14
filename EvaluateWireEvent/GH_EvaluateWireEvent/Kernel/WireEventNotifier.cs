using EvaluateWireEvent;
using Grasshopper;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI.Canvas.Interaction;
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
                activeObject.ObjectChanged -= OnAddedSource;
                activeObject.ObjectChanged += OnAddedSource;
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
                if (obj is IGH_Param param)
                    param.ObjectChanged += OnAddedSource;
                if (obj is IGH_Component component)
                {
                    foreach (IGH_Param param_component in component.Params.Input)
                        param_component.ObjectChanged += OnAddedSource;
                }
            }
        }

        private static void Document_ObjectsDeleted(object sender, GH_DocObjectEventArgs e)
        {
            foreach (IGH_DocumentObject obj in e.Objects)
            {
                if (obj is IGH_Param param)
                    param.ObjectChanged -= OnAddedSource;
                if (obj is IGH_Component component)
                {
                    foreach (IGH_Param param_component in component.Params.Input)
                        param_component.ObjectChanged -= OnAddedSource;
                }
            }
        }

        private static void OnAddedSource(IGH_DocumentObject sender, GH_ObjectChangedEventArgs e)
        {
            if (sender is IGH_Param param && Instances.ActiveCanvas.ActiveInteraction is GH_WireInteraction wireInteraction)
            {
                wireInteraction.WireProperties(out IGH_Param source, out IGH_Param target, out string mode);

                if (param == source || param == target)
                {
                    param.CheckMatching();
                }
            }
        }
    }
}
