using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Phnyx.Web.Renderings
{
    [SuppressMessage("Microsoft.Naming", "CA1710", Justification = "Seen as a Rendering with child renderings - not a collecttion of renderings")]
    [DebuggerDisplay("Content = {Content}")]
    public class Literal : RenderingBase
    {
        string _content;
        public Literal(string content)
        {
            _content = content;
        }

        public override void Process(System.IO.TextWriter writer)
        {
            writer.Write(this._content);
            base.Process(writer);
        }

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }
    }
}
