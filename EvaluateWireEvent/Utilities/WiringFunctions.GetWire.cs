using Grasshopper;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI.Canvas.Interaction;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Reflection;

namespace EvaluateWireEvent.Utilities
{
    public static partial class WiringFunctions
    {
        public static WireLR GetWire(IGH_DocumentObject sender, GH_ObjectChangedEventArgs e)
        {
            if (e.Type == GH_ObjectEventType.Sources && sender is IGH_Param param)
            {
                GH_Canvas canvas = Instances.ActiveCanvas;
                if (canvas.IsDocument && canvas.Document.SolutionState == GH_ProcessStep.PostProcess && canvas.ActiveInteraction is GH_WireInteraction wireInteraction)
                {
                    //get_field is a function of wireInteraction.
                    Func<string, object> get_field = name => wireInteraction.GetType().GetField(name, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(wireInteraction);

                    if (get_field("m_mode") is object w_mode)
                    {
                        IGH_Param w_source = get_field("m_source") as IGH_Param;
                        IGH_Param w_target = get_field("m_target") as IGH_Param;

                        IGH_Param left;
                        IGH_Param right;
                        if (w_source == param)
                        {
                            left = w_target;
                            right = w_source;
                        }
                        else if (w_target == param)
                        {
                            left = w_source;
                            right = w_target;
                        }
                        else
                            return null;

                        return new WireLR(w_mode.ToString(), left, right);
                    }
                }
            }
            return null;
        }
    }
}
