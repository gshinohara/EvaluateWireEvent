using Grasshopper;
using Grasshopper.GUI.Canvas.Interaction;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Reflection;

namespace EvaluateWireEvent
{
    public static class WireFunctions
    {
        public static void OnAddSource(IGH_DocumentObject sender, GH_ObjectChangedEventArgs e)
        {
            if (sender is IGH_Param param && Instances.ActiveCanvas.ActiveInteraction is GH_WireInteraction wireInteraction)
            {
                Func<string, object> get_field = name => wireInteraction.GetType().GetField(name, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(wireInteraction);

                IGH_Param source = get_field("m_source") as IGH_Param;
                IGH_Param target = get_field("m_target") as IGH_Param;
                string mode = get_field("m_mode").ToString();

                if (param == source || param == target)
                    Rhino.RhinoApp.WriteLine($"Wire Event ({mode}) : {param.Name}" + ((param.Attributes is GH_LinkedParamAttributes att) ? $"{att.Parent.DocObject.Name}" : ""));
            }
        }
    }
}
