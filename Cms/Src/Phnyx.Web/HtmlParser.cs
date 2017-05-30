using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.RegularExpressions;
using Phnyx.Web.Renderings;
using System.Globalization;

namespace Phnyx.Web
{
    public sealed class HtmlParser
    {
        private static Regex _tagRegex = new TagRegex();
        private static Regex _endtagRegex = new EndTagRegex();
        private static Regex _textRegex = new TextRegex();
        private static Regex _doctypeRegex = new DoctypeRegex();
        private static Regex _noteRegex = new NoteRegex();

        private HtmlParser()
        {

        }
        public static RenderingBase Parse(TextReader reader)
        {
            string html = reader.ReadToEnd();
            reader.Dispose();
            return ParseString(html);
        }

        public static RenderingBase ParseString(string html)
        {
            var root = RenderingBase.CreateRoot();
            RenderingBase current = root;
            int pos = 0;
            Match m;

            while (pos < html.Length)
            {
                if ((m = _textRegex.Match(html, pos)).Success)
                {
                    current.Add(new Literal(m.Value));
                    pos = m.Index + m.Length;
                }
                else if ((m = _tagRegex.Match(html, pos)).Success)
                {
                    var tag = new HtmlTag(m.Groups["tagname"].Value);
                    tag.IsFullTag = m.Groups["empty"].Success || IsExceptionHtmlRule(tag.Name);
                    ParsAttributes(m, tag);
                    current.Add(tag);

                    if (!tag.IsFullTag)
                        current = tag;
                    pos = m.Index + m.Length;
                }
                else if ((m = _endtagRegex.Match(html, pos)).Success)
                {
                    var tag = current as HtmlTag;
                    if (tag!=null && tag.Name.EqualsIgnoreCase(m.Groups["tagname"].Value))
                        current = current.Parent;
                    pos = m.Index + m.Length;
                }
                else if ((m = _doctypeRegex.Match(html, pos)).Success)
                {
                    // We process doctype declarations as plain text...
                    current.Add(new Literal(m.Value));
                    pos = m.Index + m.Length;
                }
                else if ((m = _noteRegex.Match(html, pos)).Success)
                {
                    current.Add(new Literal(m.Value));
                    pos = m.Index + m.Length;
                }
                else
                {
                    throw new FormatException("Unexpected Html format: '" + html.Substring(pos) + "'");
                }
            }


            return root.Count == 1 ? root[0] : root;
        }

        private static void ParsAttributes(Match m, RenderingBase tag)
        {
            var names = m.Groups["attrname"].Captures;
            var values = m.Groups["attrval"].Captures;

            for (int i = 0; i < names.Count; i++)
            {
                tag.Attributes.Add(names[i].Value, values[i].Value);
            }
        }


        /// <summary>
        /// Some tags are ok, even if it is not Xhtml f.ex. &lt;br> or &lt;input>
        /// </summary>
        private static bool IsExceptionHtmlRule(string key)
        {
            switch (key.ToUpper(CultureInfo.InvariantCulture))
            {
                case "HR":
                case "IMG":
                case "BR":
                case "INPUT":
                    return true;
                default:
                    return false;
            }
        }
    }

    internal class DoctypeRegex : Regex
    {
        public DoctypeRegex()
            : base(@"<!DOCTYPE[^>]+>")
        {
        }
    }

    internal class NoteRegex : Regex
    {
        public NoteRegex()
            : base("<!--[^-->]*-->")
        {

        }
    }
}
