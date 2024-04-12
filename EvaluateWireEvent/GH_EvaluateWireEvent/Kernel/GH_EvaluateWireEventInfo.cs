using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace GH_EvaluateWireEvent.Kernel
{
    public class GH_EvaluateWireEventInfo : GH_AssemblyInfo
    {
        public override string Name => "GH_EvaluateWireEvent";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("cea8fbe6-8a36-4f77-9534-69b0e4d74250");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}