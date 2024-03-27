using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;

namespace EvaluateWireEvent.Utilities
{
    public static partial class WiringFunctions
    {
        /// <summary>
        ///  If an attribute of param is linked-attribute, you get its parent. Otherwire returns child.
        /// </summary>
        /// <param name="child">What you want to check.</param>
        /// <returns>If param is null, this returns null. Otherwise this returns parent.</returns>
        public static IGH_ActiveObject GetParent(IGH_Param child)
        {
            IGH_ActiveObject activeObject = child;
            if (child.Attributes is GH_LinkedParamAttributes linkedAtt && linkedAtt.Parent.DocObject is IGH_Component component)
                activeObject = component;
            return activeObject;
        }

        public static bool IsEqualAccess(IGH_Param param1,IGH_Param param2)
        {
            return param1.Access == param2.Access;
        }
    }
}
