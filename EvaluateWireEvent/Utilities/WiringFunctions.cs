using Grasshopper.Kernel;

namespace EvaluateWireEvent.Utilities
{
    public class WireLR
    {
        public string Mode { get; }
        public IGH_ActiveObject LeftObject { get; }
        public IGH_ActiveObject RightObject { get; }
        public IGH_Param LeftParam { get; }
        public IGH_Param RightParam { get; }

        internal WireLR(string mode, IGH_Param left, IGH_Param right)
        {
            Mode = mode;
            LeftParam = left;
            RightParam = right;
            LeftObject = WiringFunctions.GetParent(left);
            RightObject = WiringFunctions.GetParent(right);
        }
    }
    public static partial class WiringFunctions
    {
        public static void ObjectChangedAndWired(IGH_DocumentObject sender, GH_ObjectChangedEventArgs e)
        {
            Rhino.RhinoApp.WriteLine("");
            if (GetWire(sender, e) is WireLR wire)
            {
                //Check Access
                bool isEqualAccess = IsEqualAccess(wire.LeftParam, wire.RightParam);
                if (!isEqualAccess)
                    Rhino.RhinoApp.WriteLine("Grasshopper Solution : No equality of Access.");
            }
        }
    }
}
