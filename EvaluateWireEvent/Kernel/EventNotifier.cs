using EvaluateWireEvent.Components;
using EvaluateWireEvent.Utilities;
using Grasshopper;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace EvaluateWireEvent.Kernel
{
    public class EventNotifier : GH_DocumentObject
    {
        public EventNotifier() : base("Event Notifier", "Notifier", "", "Params", "Util")
        {
        }

        public override void AddedToDocument(GH_Document document)
        {
            Rhino.RhinoApp.WriteLine($"{this.GetType().Name} is loaded.");

            foreach (IGH_ActiveObject activeObject in document.ActiveObjects())
                activeObject.ObjectChanged += WiringFunctions.ObjectChangedAndWired;

            document.ObjectsAdded += this.Document_ObjectsAdded;
            document.ObjectsDeleted += this.Document_ObjectsDeleted;

            base.AddedToDocument(document);
        }

        public override void RemovedFromDocument(GH_Document document)
        {
            Rhino.RhinoApp.WriteLine($"{this.GetType().Name} is destroyed.");

            foreach (IGH_ActiveObject activeObject in document.ActiveObjects())
                activeObject.ObjectChanged -= WiringFunctions.ObjectChangedAndWired;

            document.ObjectsAdded -= this.Document_ObjectsAdded;
            document.ObjectsDeleted -= this.Document_ObjectsDeleted;

            base.RemovedFromDocument(document);
        }

        private void Document_ObjectsAdded(object sender, GH_DocObjectEventArgs e)
        {
            foreach (IGH_DocumentObject obj in e.Objects)
            {
                if (obj is IGH_ActiveObject activeObject)
                    activeObject.ObjectChanged += WiringFunctions.ObjectChangedAndWired;
            }
        }

        private void Document_ObjectsDeleted(object sender, GH_DocObjectEventArgs e)
        {
            foreach (IGH_DocumentObject obj in e.Objects)
            {
                if (obj is IGH_ActiveObject activeObject)
                    activeObject.ObjectChanged -= WiringFunctions.ObjectChangedAndWired;
            }
        }

        public override void CreateAttributes()
        {
            this.Attributes = new EventNotifierAttribute(this);
        }

        public override Guid ComponentGuid => new Guid("3E3690FC-E7AB-450A-83AB-7DB60DBA15B8");

        public override GH_Exposure Exposure => GH_Exposure.hidden;
    }

    internal class EventNotifierAttribute : GH_Attributes<EventNotifier>
    {
        public EventNotifierAttribute(EventNotifier owner) : base(owner)
        {
        }

        public override bool Selected => false;

        protected override void Layout()
        {
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
        }
    }

    public class EventNotifierLoad : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            Instances.CanvasCreated -= Instances_CanvasCreated;
            Instances.CanvasCreated += Instances_CanvasCreated;
            Instances.CanvasDestroyed -= Instances_CanvasDestroyed;
            Instances.CanvasDestroyed += Instances_CanvasDestroyed;
            return GH_LoadingInstruction.Proceed;
        }

        private void Instances_CanvasCreated(GH_Canvas canvas)
        {
            canvas.CanvasPaintBegin += AddNotifier;
        }

        private void Instances_CanvasDestroyed(GH_Canvas canvas)
        {
            canvas.CanvasPaintBegin -= AddNotifier;
            RemoveAllNotifier(canvas);
        }

        private void AddNotifier(GH_Canvas canvas)
        {
            if (canvas.IsDocument && canvas.Document is GH_Document document && !document.Objects.Any(o => o is EventNotifier))
            {
                canvas.CanvasPaintBegin -= AddNotifier;
                document.AddObject(new EventNotifier(), update: false, document.ObjectCount);
            }
        }

        private void RemoveAllNotifier(GH_Canvas canvas)
        {
            if (canvas.IsDocument && canvas.Document is GH_Document document)
            {
                List<EventNotifier> list = new List<EventNotifier>();
                foreach (IGH_DocumentObject o in document.Objects)
                {
                    if (o is EventNotifier notifier)
                        list.Add(notifier);
                }
                document.RemoveObjects(list, false);
            }
        }
    }
}
