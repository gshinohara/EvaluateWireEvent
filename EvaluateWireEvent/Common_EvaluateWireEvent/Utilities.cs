using Grasshopper;
using Grasshopper.GUI.Canvas.Interaction;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Reflection;

namespace EvaluateWireEvent
{
    public static partial class Utilities
    {
        public static void WireProperties(this GH_WireInteraction wireInteraction, out IGH_Param source, out IGH_Param target, out string mode)
        {
            Func<string, object> get_field = name => wireInteraction.GetType().GetField(name, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(wireInteraction);

            source = get_field("m_source") as IGH_Param;
            target = get_field("m_target") as IGH_Param;
            mode = get_field("m_mode").ToString();
        }

        public static IGH_Component GetParentComponent(this IGH_Param param)
        {
            if (param.Attributes is GH_LinkedParamAttributes att && att.Parent.DocObject is IGH_Component component)
                return component;
            else
                return null;
        }
    }
}
