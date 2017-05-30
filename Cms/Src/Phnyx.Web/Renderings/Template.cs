using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web.Hosting;

namespace Phnyx.Web.Renderings
{
    [SuppressMessage("Microsoft.Naming", "CA1710", Justification = "Seen as a Rendering with child renderings - not a collecttion of renderings")]
    public class Template : RenderingBase
    {
        private string virtualPath = null;

        public Template(string virtualPath, Filter filter)
        {
            this.virtualPath = virtualPath;
            this.Initialize(File.OpenText(HostingEnvironment.MapPath(virtualPath)), filter);
        }

        public Template(string virtualPath) : this(virtualPath, null) { }

        /// <summary>
        /// Should only be used for unittests to circumsize HostingEnvironment
        /// </summary>
        internal Template(TextReader reader, Filter filter)
        {
            Initialize(reader, filter);
        }

        private void Initialize(TextReader reader, Filter filter)
        {
            var template = HtmlParser.Parse(reader);
            if (filter != null)
            {
                var root = template.Find(filter);
                if (root != null)
                    this.Add(root);
            }
            else
            {
                foreach (var item in template)
                {
                    this.Add(item);
                }
            }

            if (virtualPath != null)
                this.Accept(new MapPathVisitor(HostingEnvironment.ApplicationVirtualPath, virtualPath));
        }

        public void Bind(object data)
        {
            foreach (var info in data.GetType().GetProperties())
            {
                object[] atr = info.GetCustomAttributes(typeof(MarkupAttribute), false);
                var value = info.GetValue(data, null);
                if (value == null)
                    continue;
                var visitor = new InnerTextVisitor(value.ToString());

                if (atr.Length > 0)
                    visitor.Filter = Filter.ElementId(((MarkupAttribute)atr[0]).Id);
                else // If no MarkupAttribute, search Dom for Tag with Name or id matching property
                    visitor.Filter = Filter.ElementName(info.Name)
                                           .Or(Filter.ElementId(info.Name));

                Accept(visitor);
            }
        }
    }
}
