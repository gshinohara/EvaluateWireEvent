using Grasshopper;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI.Canvas.Interaction;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace EvaluateWireEvent.Components
{
    public class Component_WireEventWatcher : GH_Component
    {
        private GH_Canvas canvas;

        public Component_WireEventWatcher()
          : base("Wire Event Watcher", "Watcher",
              "",
              "Params", "Util")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
        }

        public override void AddedToDocument(GH_Document document)
        {
            foreach (IGH_ActiveObject activeObject in document.ActiveObjects())
                activeObject.ObjectChanged += ActiveObject_ObjectChanged;

            document.ObjectsAdded += Document_ObjectsAdded;
            document.ObjectsDeleted += Document_ObjectsDeleted;

            GH_Canvas canvas = Instances.ActiveCanvas;
            if (canvas.Document == document)
                this.canvas = canvas;

            base.AddedToDocument(document);
        }

        public override void RemovedFromDocument(GH_Document document)
        {
            foreach (IGH_ActiveObject activeObject in document.ActiveObjects())
                activeObject.ObjectChanged -= ActiveObject_ObjectChanged;

            document.ObjectsAdded -= Document_ObjectsAdded;
            document.ObjectsDeleted -= Document_ObjectsDeleted;

            base.RemovedFromDocument(document);
        }

        private void ActiveObject_ObjectChanged(IGH_DocumentObject sender, GH_ObjectChangedEventArgs e)
        {
            if (e.Type == GH_ObjectEventType.Sources && sender is IGH_Param param)
            {
                if (!(canvas is null) && canvas.Document.SolutionState == GH_ProcessStep.PreProcess && canvas.ActiveInteraction is GH_WireInteraction wireInteraction)
                {
                    //if param is null, get_parent returns null.
                    Func<IGH_Param, IGH_ActiveObject> get_parent = p =>
                    {
                        IGH_ActiveObject activeObject = p;
                        if (p.Attributes is GH_LinkedParamAttributes linkedAtt && linkedAtt.Parent.DocObject is IGH_Component component)
                            activeObject = component;
                        return activeObject;
                    };

                    //get_field is a function of wireInteraction.
                    Func<string, object> get_field = name => wireInteraction.GetType().GetField(name, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(wireInteraction);

                    if (get_field("m_mode") is object w_mode && get_field("m_source") is IGH_Param w_source && get_field("m_target") is IGH_Param w_target)
                    {
                        IGH_ActiveObject left;
                        IGH_ActiveObject right;
                        if (w_source == param)
                        {
                            left = get_parent(w_target);
                            right = get_parent(w_source);
                        }
                        else if (w_target == param)
                        {
                            left = get_parent(w_source);
                            right = get_parent(w_target);
                        }
                        else
                            return;

                        if (left != null && right != null)
                            Rhino.RhinoApp.WriteLine($"Wire event changing object ({w_mode.ToString()}): {left.Name} -> {right.Name}");
                    }
                }
            }
        }

        private void Document_ObjectsAdded(object sender, GH_DocObjectEventArgs e)
        {
            foreach (IGH_DocumentObject obj in e.Objects)
            {
                if (obj is IGH_ActiveObject activeObject)
                    activeObject.ObjectChanged += ActiveObject_ObjectChanged;
            }
        }

        private void Document_ObjectsDeleted(object sender, GH_DocObjectEventArgs e)
        {
            foreach (IGH_DocumentObject obj in e.Objects)
            {
                if (obj is IGH_ActiveObject activeObject)
                    activeObject.ObjectChanged -= ActiveObject_ObjectChanged;
            }
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("429BCBB3-2825-44B0-81E6-311A9BC686AC");
    }
}