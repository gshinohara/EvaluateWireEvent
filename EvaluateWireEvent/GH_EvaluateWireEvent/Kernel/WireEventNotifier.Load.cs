using Grasshopper;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;

namespace GH_EvaluateWireEvent.Kernel
{
    public class EventNotifier_Load : GH_AssemblyPriority
    {
        private WireEventNotifier m_notifier;

        public override GH_LoadingInstruction PriorityLoad()
        {
            Instances.CanvasCreated += Instances_CanvasCreated;
            return GH_LoadingInstruction.Proceed;
        }

        private void Instances_CanvasCreated(GH_Canvas canvas)
        {
            canvas.DocumentChanged += Canvas_DocumentChanged;
        }

        private void Canvas_DocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
        {
            m_notifier =  WireEventNotifier.Instantiate(sender, e.NewDocument);
        }
    }
}
