using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace Phnyx.Web.Renderings
{
    [SuppressMessage("Microsoft.Naming", "CA1710", Justification = "Seen as a Rendering with child renderings - not a collecttion of renderings")]
    public class Root : RenderingBase
    {
        public override RenderingBase Parent
        {
            get
            {
                return this;
            }
            protected set
            {
                throw new NotSupportedException("You can't set the Root");
            }
        }
    }
}
