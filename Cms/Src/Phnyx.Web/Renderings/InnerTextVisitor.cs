using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phnyx.Web.Renderings;

namespace Phnyx.Web.Renderings
{
    public class InnerTextVisitor : VisitorBase
    {
        public InnerTextVisitor(string text)
        {
            this.Text = text;
        }
        public override void OnVisit(RenderingBase rendering)
        {
            rendering.Clear();
            rendering.Add(new Literal(this.Text));
            Register(rendering[0]);
        }

        public string Text { get; set; }
    }
}
