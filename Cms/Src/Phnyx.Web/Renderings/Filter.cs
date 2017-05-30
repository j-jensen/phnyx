using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phnyx.Web.Renderings;

namespace Phnyx.Web.Renderings
{
    public class Filter
    {
        private Predicate<RenderingBase> _match;

        private Filter() { }

        private Filter(Predicate<RenderingBase> match)
        {
            _match = match;
        }

        public bool Pass(RenderingBase rendering)
        {
            return _match(rendering);
        }

        public Filter Or(Filter restriction)
        {
            return new Filter(rendering => this._match(rendering) || restriction._match(rendering));
        }

        public Filter And(Filter restriction)
        {
            return new Filter(rendering => this._match(rendering) && restriction._match(rendering));
        }

        #region Static Filters
        /// <summary>
        /// Returns a restriction on the css-class of the element
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static Filter CssClass(string name)
        {
            return new Filter(rendering => name.Equals(rendering.Attributes["class"], StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns a Restriction filtering on the element id-attribute
        /// </summary>
        public static Filter ElementId(string id)
        {
            return new Filter(rendering => id.Equals(rendering.Attributes["id"], StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns a filter for element-name
        /// </summary>
        public static Filter ElementName(string name)
        {
            return new Filter(rendering =>
            {
                var htmlTag = rendering as HtmlTag;
                return htmlTag!=null && htmlTag.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
            });
        }

        /// <summary>
        /// Returns a filter wich return true if, element has an attribute with that name
        /// </summary>
        public static Filter AttributeName(string name)
        {
            return new Filter(rendering => rendering.Attributes.AllKeys.Contains(name));
        }

        /// <summary>
        /// Returns a filter wich return true, if element has an attribute with that name and value
        /// </summary>
        public static Filter Attribute(string name, string value)
        {
            return new Filter(rendering => rendering.Attributes[name] == value);
        }

        public static Filter AlwaysTrue()
        {
            return new Filter(rendering => true);
        }
        #endregion

    }
}
