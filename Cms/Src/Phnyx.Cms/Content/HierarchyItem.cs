using System;
using System.Globalization;

namespace Phnyx.Cms.Content
{
    public abstract class HierarchyItem : IFormattable
    {
        public HierarchyItem(string title)
            : this(title, null)
        {
        }

        public HierarchyItem(string title, HierarchyItem parent)
        {
            this._parent = parent;
            this._title = title;
        }

        private string _title = null;
        private HierarchyItem _parent;

        public HierarchyItem Parent
        {
            get { return _parent; }
        }

        public virtual string RessourceName
        {
            get
            {
                if (this._parent == null)
                    return "/";
                else
                    return System.IO.Path.Combine( this._parent.RessourceName, this.ToString("s"));
            }
        }

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null) format = "G";

            if (formatProvider != null)
            {
                ICustomFormatter formatter = formatProvider.GetFormat(
                    this.GetType())
                    as ICustomFormatter;

                if (formatter != null)
                    return formatter.Format(format, this, formatProvider);
            }

            switch (format)
            {
                case "s": return Uri.EscapeUriString(_title);
                case "G":
                default: return _title;
            }
        }
        public string ToString(string format)
        {
            return this.ToString(format, CultureInfo.InvariantCulture);
        }
        #endregion
    }
}
