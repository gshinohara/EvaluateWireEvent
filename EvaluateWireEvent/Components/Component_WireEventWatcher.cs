using EvaluateWireEvent.Utilities;
using Grasshopper.Kernel;
using System;
using System.Linq;

namespace EvaluateWireEvent.Components
{
    public class Component_WireEventWatcher : GH_Component
    {
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

        private static void ObjectChangedAndWired(IGH_DocumentObject sender, GH_ObjectChangedEventArgs e)
        {
            if (WiringFunctions.GetWire(sender, e) is WireLR wire)
            {
                if (wire.Mode is string mode && wire.LeftObject is IGH_ActiveObject left && wire.RightObject is IGH_ActiveObject right)
                    Rhino.RhinoApp.WriteLine($"Wire event changing object ({mode.ToString()}): {left.Name} -> {right.Name}");
            }
        }

        public override void AddedToDocument(GH_Document document)
        {
            foreach (IGH_ActiveObject activeObject in document.ActiveObjects())
                activeObject.ObjectChanged += Component_WireEventWatcher.ObjectChangedAndWired;

            document.ObjectsAdded += this.Document_ObjectsAdded;
            document.ObjectsDeleted += this.Document_ObjectsDeleted;

            Rhino.RhinoApp.WriteLine($"{Name} is loaded, {document.Objects.Count(o => o is Component_WireEventWatcher) - 1} already existing.");

            base.AddedToDocument(document);
        }

        public override void RemovedFromDocument(GH_Document document)
        {
            foreach (IGH_ActiveObject activeObject in document.ActiveObjects())
                activeObject.ObjectChanged -= Component_WireEventWatcher.ObjectChangedAndWired;

            document.ObjectsAdded -= this.Document_ObjectsAdded;
            document.ObjectsDeleted -= this.Document_ObjectsDeleted;

            Rhino.RhinoApp.WriteLine($"{Name} is destroyed, {document.Objects.Count(o => o is Component_WireEventWatcher)} remaining.");

            base.RemovedFromDocument(document);
        }

        private void Document_ObjectsAdded(object sender, GH_DocObjectEventArgs e)
        {
            foreach (IGH_DocumentObject obj in e.Objects)
            {
                if (obj is IGH_ActiveObject activeObject)
                    activeObject.ObjectChanged += Component_WireEventWatcher.ObjectChangedAndWired;
            }
        }

        private void Document_ObjectsDeleted(object sender, GH_DocObjectEventArgs e)
        {
            foreach (IGH_DocumentObject obj in e.Objects)
            {
                if (obj is IGH_ActiveObject activeObject)
                    activeObject.ObjectChanged -= Component_WireEventWatcher.ObjectChangedAndWired;
            }
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("429BCBB3-2825-44B0-81E6-311A9BC686AC");
    }
}