using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Phnyx.Web.Renderings
{
    [SuppressMessage("Microsoft.Naming", "CA1710", Justification = "Seen as a Rendering with child renderings - not a collecttion of renderings")]
    [DebuggerDisplay("Name = {Name}")]
    public class HtmlTag : RenderingBase
    {
        private string _name;

        public HtmlTag() : this("div") { }

        public HtmlTag(string name)
        {
            this.IsFullTag = false;
            this._name = name;
        }

        public override void Process(System.IO.TextWriter writer)
        {
            writer.Write("<" + this.Name);
            foreach (var key in this.Attributes.AllKeys)
                writer.Write(" {0}=\"{1}\"", key, this.Attributes[key]);
            writer.Write(this.IsFullTag ? " />" : ">");

            if (!this.IsFullTag)
            {
                base.Process(writer); // A full tag should not have children to render
                writer.Write("</" + this.Name + ">");
            }
        }

        /// <summary>
        /// Indicates if the tag is full / closed ie. <br />
        /// </summary>
        public bool IsFullTag { get; set; }
        public string Name { get { return _name; } }
    }
}
