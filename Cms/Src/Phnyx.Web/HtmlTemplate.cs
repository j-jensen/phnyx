using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;

namespace Phnyx.Web
{
    [Obsolete("Use TemplateRendering")]
    public class HtmlTemplate
    {
        #region Fields
        private TextReader _reader;
        private Rendering _rendering;
        #endregion

        // Methods
        public HtmlTemplate(TextReader reader)
        {
            _reader = reader;
            this._rendering = Parse(reader);
        }
        public HtmlTemplate(string path) : this(File.OpenText(path)) { }

        public void Invoke(RenderingVisitor visitor)
        {
            _rendering.Accept(visitor);
        }

        private Rendering Parse(TextReader reader)
        {
            return HtmlParser.Parse(reader);
        }

        public void Process(TextWriter writer)
        {
            this._rendering.Process(writer);
        }

        public void SetData(object value)
        {
            foreach (var info in value.GetType().GetProperties())
            {
                object[] atr = info.GetCustomAttributes(typeof(MarkupAttribute), false);
                var visitor = new InnerTextVisitor(info.GetValue(value, null).ToString());
                
                if (atr.Length > 0)
                    visitor.SetFilter(Filter.ElementId(((MarkupAttribute)atr[0]).Id));
                else // If no MarkupAttribute, search Dom for Tag with Name
                    visitor.SetFilter(Filter.ElementName(info.Name));

                this._rendering.Accept(visitor);
            }
        }
    }
}